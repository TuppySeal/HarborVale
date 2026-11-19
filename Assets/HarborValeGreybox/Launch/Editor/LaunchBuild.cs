using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace HarborValeGreybox.Launch.Editor {
public static class LaunchBuild {
 public static void Regression(){HarborValeGreybox.Boost.Editor.BoostVerification.Run();HarborValeGreybox.SportsTest.Editor.ApexVerification.Run();HarborValeGreybox.Racing.Editor.TrialVerification.Run();}
 public const string MenuPath="Assets/HarborValeGreybox/Launch/Scenes/HarborVale_Hub.unity";
 public static string[] Scenes=>new[]{MenuPath,"Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity","Assets/HarborValeGreybox/SportsTest/Scenes/HarborVale_SportsTest.unity"};
 public static void Prepare(){
  if(!File.Exists(MenuPath)){var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var g=new GameObject("Menu camera");g.tag="MainCamera";var cam=g.AddComponent<Camera>();cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.035f,.065f,.095f);g.AddComponent<AudioListener>();EditorSceneManager.SaveScene(scene,MenuPath);}
  var entries=new EditorBuildSettingsScene[Scenes.Length];for(int i=0;i<Scenes.Length;i++)entries[i]=new EditorBuildSettingsScene(Scenes[i],true);EditorBuildSettings.scenes=entries;AssetDatabase.SaveAssets();AssetDatabase.Refresh();
 }
 public static void Verify(){Directory.CreateDirectory("Verification/Launch");var run=new AccelerationRun();if(run.Arm(1))throw new Exception("Moving start accepted");if(!run.Arm(0))throw new Exception("Stopped start denied");run.Tick(.02f,0,1);run.Tick(2,50,1);run.Tick(2,110,1);if(run.State!=AccelerationState.Finished||Mathf.Abs(run.Seconds-(2+2*50f/60))>.0001f)throw new Exception("100 km/h crossing interpolation incorrect");run.Clear();run.Arm(0);run.Tick(.1f,3,0);if(run.State!=AccelerationState.Cancelled)throw new Exception("Unpowered rolling start accepted");run.Clear();run.Arm(0);run.Tick(.02f,0,1);run.Cancel("Recovery");run.Tick(5,110,1);if(run.State!=AccelerationState.Cancelled)throw new Exception("Cancelled run completed");File.WriteAllText("Verification/Launch/timer-rules.txt","PASS moving-start rejection, stopped arming, launch trigger, interpolated 100 km/h crossing, unpowered roll rejection and cancellation persistence.\n");Prepare();}
 public static void Build(){Verify();PlayerSettings.productName="Harbor Vale";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;Directory.CreateDirectory("Verification/Launch/Windows");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=Scenes,locationPathName="Verification/Launch/Windows/HarborVale.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});File.WriteAllText("Verification/Launch/player-build.txt",report.summary.result+" | "+report.summary.totalSize+" bytes | errors "+report.summary.totalErrors);if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Unified export failed");}
}
}
