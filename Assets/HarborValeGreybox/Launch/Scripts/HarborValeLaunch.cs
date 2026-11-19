using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Garage;
using HarborValeGreybox.Racing;
using HarborValeGreybox.SportsTest;
namespace HarborValeGreybox.Launch {
public sealed class HarborValeLaunch:MonoBehaviour {
 public const string MenuScene="HarborVale_Hub",CityScene="HarborVale_CityMap",SportsScene="HarborVale_SportsTest";
 public static HarborValeLaunch Instance{get;private set;}public bool Busy{get;private set;}public string Message{get;private set;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){Instance=null;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Register(){SceneManager.sceneLoaded-=Loaded;SceneManager.sceneLoaded+=Loaded;}
 static void Loaded(Scene scene,LoadSceneMode mode){
  if(scene.name!=MenuScene&&scene.name!=CityScene&&scene.name!=SportsScene&&scene.name!="Finch_HandlingTest")return;
  // All scene-specific systems initialize here, including when returning from the hub.
  FinchDrivingBootstrap.StartDriving();DocksideTrialDirector.Hook();FinchGarage.Hook();ApexTestDrive.Hook();HarborValeGreybox.Workshop.ExpandedWorkshop.Hook();TrafficWorld.Hook();RivalRaceDirector.Hook();
  if(Instance==null){var root=new GameObject("HarborVale | launch and mode selection");Instance=root.AddComponent<HarborValeLaunch>();DontDestroyOnLoad(root);}
  Instance.Message=null;
  if(scene.name==SportsScene&&!UnityEngine.Object.FindAnyObjectByType<SportsAccelerationHUD>())new GameObject("Sports | live acceleration tests").AddComponent<SportsAccelerationHUD>();
 }
 public bool Open(string scene){if(Busy||!(scene==MenuScene||scene==CityScene||scene==SportsScene)||!Application.CanStreamedLevelBeLoaded(scene))return false;
  var trial=UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>();
  if(trial!=null&&trial.Session!=null&&trial.Session.ValidFinish&&trial.SaveError!=null){Message="Finish saving this race result before leaving.";return false;}
  if(RivalRaceDirector.Instance!=null&&!RivalRaceDirector.Instance.BeforeLeave()){Message="Save the rival race result before leaving.";return false;}if(trial!=null&&trial.Active)trial.Abandon();
  Busy=true;StartCoroutine(Change(scene));return true;
 }
 IEnumerator Change(string scene){foreach(var car in UnityEngine.Object.FindObjectsByType<FinchVehicle>(FindObjectsSortMode.None))car.controlsLocked=true;foreach(var fx in UnityEngine.Object.FindObjectsByType<HarborValeGreybox.Workshop.WorkshopEffects>(FindObjectsSortMode.None))fx.BeginSceneExit();yield return new WaitForSecondsRealtime(.10f);var load=SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);yield return load;Busy=false;}
 void OnGUI(){string scene=SceneManager.GetActiveScene().name;if(scene!=MenuScene&&scene!=CityScene&&scene!=SportsScene)return;if(scene!=MenuScene){if(UnityEngine.Object.FindAnyObjectByType<HarborValeGreybox.Workshop.ExpandedWorkshop>()?.IsOpen==true)return;if(UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>()?.ExternalModalOpen==true)return;if(Application.CanStreamedLevelBeLoaded(MenuScene)){var trial=UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>();GUI.enabled=!Busy;if(GUI.Button(new Rect(Screen.width-172,Screen.height-57,156,39),trial!=null&&trial.Active?"End run / main menu":"Main menu"))Open(MenuScene);GUI.enabled=true;if(Message!=null)GUI.Box(new Rect(Screen.width-380,Screen.height-100,364,38),Message);}return;}
  var old=GUI.matrix;GUI.matrix=Matrix4x4.Scale(new Vector3(Screen.width/1280f,Screen.height/800f,1));var oldColor=GUI.color;GUI.color=new Color(.035f,.065f,.095f);GUI.DrawTexture(new Rect(0,0,1280,800),Texture2D.whiteTexture);GUI.color=oldColor;
  var title=new GUIStyle(GUI.skin.label){fontSize=54,fontStyle=FontStyle.Bold};title.normal.textColor=new Color(.28f,.89f,.83f);var heading=new GUIStyle(GUI.skin.label){fontSize=25,fontStyle=FontStyle.Bold};var text=new GUIStyle(GUI.skin.label){fontSize=18,wordWrap=true};var button=new GUIStyle(GUI.skin.button){fontSize=21,fontStyle=FontStyle.Bold};
  GUI.Label(new Rect(80,63,1120,80),"HARBOR VALE",title);GUI.Label(new Rect(84,146,1120,44),"Choose your drive",heading);
  GUI.Box(new Rect(80,226,540,330),"");GUI.Box(new Rect(660,226,540,330),"");GUI.Label(new Rect(108,252,480,40),"ASTER FINCH",heading);GUI.Label(new Rect(688,252,480,40),"ASTER APEX",heading);
  GUI.Label(new Rect(108,309,480,140),"Race First Lap, earn credits and buy parts at the player garage.\n\nYour paid upgrades and race progress carry over.",text);GUI.Label(new Rect(688,309,480,140),"AWD sports car with extra grip and downforce.\n\nTry stronger upgrades for free and compare live 0–100 times.",text);
  GUI.enabled=!Busy;if(GUI.Button(new Rect(108,468,484,56),"Drive Finch · saved progress",button))Open(CityScene);if(GUI.Button(new Rect(688,468,484,56),"Test Apex · free upgrades",button))Open(SportsScene);if(GUI.Button(new Rect(1030,677,170,46),"Quit"))Application.Quit();GUI.enabled=true;
  GUI.Label(new Rect(84,588,1100,83),"W / S — accelerate, brake, reverse     A / D — steer     R — recover     C — camera\nRMB + mouse look / J-L side views / K rear view. Release to center.\nFinch: G opens the nearby garage. Apex: stop and press G for free test parts.",text);GUI.Label(new Rect(84,714,920,38),Busy?"Loading your drive…":"Apex test parts reset each visit. Your Finch credits stay separate.",text);GUI.matrix=old;
 }
}
}
