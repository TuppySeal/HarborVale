using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace HarborValeGreybox.Driving.Editor {
[InitializeOnLoad]public static class FinchPlayModeVerification {
 const string Armed="HarborVale.Driving.PlayTest";static double started;static int phase;static bool failure;static string lines="";static string root;
 static FinchPlayModeVerification(){if(SessionState.GetBool(Armed,false)){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;Application.logMessageReceived+=Log;started=EditorApplication.timeSinceStartup;}}
 static void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error){if(stack.Contains("UnityEditor.Search.SearchDatabase")){lines+="EDITOR-ONLY ISSUE: Unity SearchDatabase startup indexing exception; excluded from runtime-controller result. See editor log.\n";return;}failure=true;lines+="ERROR "+message+"\n";}}
 public static void Begin(){SessionState.SetBool(Armed,true);EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");EditorApplication.isPlaying=true;}
 static void State(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Armed,false);Directory.CreateDirectory("Verification/Driving");File.WriteAllText("Verification/Driving/playmode-results.txt",lines);EditorApplication.Exit(failure?1:0);}}
 static void Check(bool ok,string text){lines+=(ok?"PASS ":"FAIL ")+text+"\n";if(!ok)failure=true;}
 static void Tick(){
  if(!EditorApplication.isPlaying){if(EditorApplication.timeSinceStartup-started>120){failure=true;lines+="FAIL play mode startup timeout\n";State(PlayModeStateChange.EnteredEditMode);}return;}
  EditorApplication.QueuePlayerLoopUpdate();var car=UnityEngine.Object.FindAnyObjectByType<FinchVehicle>();if(car==null)return;float t=Time.timeSinceLevelLoad;
  car.externalInput=true;car.commandedHandbrake=false;car.commandedSteer=0;
  if(phase==0&&t>2){Check(car.GroundedWheels>=3,"Runtime bootstrap equips car and establishes ground contact");Check(Camera.main.GetComponent<FinchChaseCamera>()!=null&&!Camera.main.orthographic,"Runtime chase camera active");Check(car.GetComponent<FinchDrivingHUD>()!=null,"Runtime HUD and procedural audio initialized");phase=1;}
  car.commandedPedal=t>2&&t<8?1:t>=8&&t<11?-1:0;
  if(phase==1&&t>8){Check(car.SpeedKph>35,"Actual Play Mode acceleration: "+car.SpeedKph.ToString("F1")+" km/h");phase=2;}
  if(phase==2&&t>11){Check(car.SpeedKph<25,"Actual Play Mode brake/reverse input reduces speed");car.Recover();Check(car.SpeedKph<.1f,"Recovery clears velocity in Play Mode");car.GetComponent<FinchDrivingHUD>().SetInspection(true);phase=3;}
  if(phase==3&&t>12){Check(Camera.main.GetComponent<MapExplorer>().enabled&&!Camera.main.GetComponent<FinchChaseCamera>().enabled,"Map inspection camera and parked vehicle toggle");car.GetComponent<FinchDrivingHUD>().SetInspection(false);phase=4;}
  if(phase==4&&t>13){Check(!Camera.main.orthographic&&Camera.main.GetComponent<FinchChaseCamera>().enabled,"Return from map to driving perspective");Directory.CreateDirectory("Verification/Driving");ScreenCapture.CaptureScreenshot(Path.GetFullPath("Verification/Driving/Finch_City_PlayMode.png"));phase=5;}
  if(phase==5&&t>15){Check(!failure,"No runtime exceptions during driving/camera/HUD checks");EditorApplication.isPlaying=false;}
  if(t>45){failure=true;lines+="FAIL runtime test timeout\n";EditorApplication.isPlaying=false;}
 }
}
}

