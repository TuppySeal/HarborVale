using System;
using System.IO;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing {
public static class DrivingUnits {
 [Serializable]class Preference {public int version=1;public bool mph=true;}
 public const float KphPerMph=1.609344f;static Preference current;public static string FileOverrideForTests;public static string Error{get;private set;}
 public static string FilePath=>FileOverrideForTests??Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"HarborVale","Settings","driving-units-v1.json");
 public static bool Mph {get{if(current==null)Reload();return current.mph;}}
 public static float FromMph(float mph)=>mph*KphPerMph;public static float Display(float kph)=>Mph?kph/KphPerMph:kph;public static string Suffix=>Mph?"mph":"km/h";public static string Speed(float kph)=>Display(kph).ToString("F0")+" "+Suffix;
 public static void Reload(){current=new Preference();Error=null;try{if(!File.Exists(FilePath))return;var data=JsonUtility.FromJson<Preference>(File.ReadAllText(FilePath));if(data==null||data.version!=1)throw new IOException("Unrecognized speed-unit settings; existing file preserved.");current=data;}catch(Exception e){Error=e.Message;}}
 public static bool Save(bool mph){Reload();if(Error!=null)return false;try{var data=new Preference{mph=mph};Directory.CreateDirectory(Path.GetDirectoryName(FilePath));string temp=FilePath+"."+Guid.NewGuid().ToString("N")+".tmp";var bytes=System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(data,true));using(var stream=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}if(File.Exists(FilePath))File.Replace(temp,FilePath,FilePath+".bak");else File.Move(temp,FilePath);current=data;return true;}catch(Exception e){Error=e.Message;return false;}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){current=null;Error=null;}
}
public sealed class DrivingSettingsView:MonoBehaviour {
 public static DrivingSettingsView Instance;public bool IsOpen{get;private set;}string message;FinchVehicle stoppedCar;DocksideTrialDirector stoppedTrial;
 public static void Hook(){if(Instance!=null)return;var g=new GameObject("Driving settings | speed display units");DontDestroyOnLoad(g);g.AddComponent<DrivingSettingsView>();}
 void Awake(){Instance=this;}
 public bool Open(){var trial=FindAnyObjectByType<DocksideTrialDirector>();var car=FindAnyObjectByType<FinchVehicle>();if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name!=HarborValeGreybox.Launch.HarborValeLaunch.MenuScene&&(car==null||car.SpeedKph>=2||car.controlsLocked||trial?.Store?.Profile.fleet.HasPending==true||trial?.ExternalModalOpen==true)){message="Stop safely outside a race to open settings.";return false;}stoppedTrial=trial;if(trial!=null)trial.ExternalModalOpen=true;stoppedCar=car;if(car!=null)car.controlsLocked=true;DrivingUnits.Reload();IsOpen=true;message=DrivingUnits.Error;return true;}
 public void Close(){IsOpen=false;if(stoppedTrial!=null)stoppedTrial.ExternalModalOpen=false;stoppedTrial=null;if(stoppedCar!=null)stoppedCar.controlsLocked=false;stoppedCar=null;}
 void Update(){if(FinchInput.Shortcut(KeyCode.F10)){if(IsOpen)Close();else Open();}}
 void OnGUI(){if(!IsOpen)return;var matrix=GUI.matrix;GUI.matrix=Matrix4x4.Scale(new Vector3(Screen.width/1280f,Screen.height/800f,1));GUI.depth=-1800;var old=GUI.color;GUI.color=new Color(.035f,.06f,.085f);GUI.DrawTexture(new Rect(0,0,1280,800),Texture2D.whiteTexture);GUI.color=old;var title=new GUIStyle(GUI.skin.label){fontSize=32,fontStyle=FontStyle.Bold};title.normal.textColor=Color.white;GUI.Label(new Rect(80,85,1120,65),"SETTINGS / SPEED UNITS",title);var text=new GUIStyle(GUI.skin.label){fontSize=22,wordWrap=true};text.normal.textColor=Color.white;GUI.Label(new Rect(84,189,1110,110),"Choose speedometer and speed-limit display units. Physics, posted limits, radar tolerance and ticket evidence stay identical.\n20mph = 32.2km/h. 10mph = 16.1km/h.",text);var button=new GUIStyle(GUI.skin.button){fontSize=25};if(GUI.Button(new Rect(84,352,530,75),(DrivingUnits.Mph?"SELECTED · ":"")+"Miles per hour / mph",button))message=DrivingUnits.Save(true)?"Speed display saved.":DrivingUnits.Error;if(GUI.Button(new Rect(644,352,530,75),(!DrivingUnits.Mph?"SELECTED · ":"")+"Kilometres per hour / km/h",button))message=DrivingUnits.Save(false)?"Speed display saved.":DrivingUnits.Error;GUI.Label(new Rect(84,475,1110,80),message??"mph is the default. Existing0–100km/h acceleration benchmarks keep their labelled metric target.",text);if(GUI.Button(new Rect(84,652,1090,60),"Back / F10",button))Close();GUI.matrix=matrix;}
 void OnDestroy(){if(Instance==this)Instance=null;}
}
}
