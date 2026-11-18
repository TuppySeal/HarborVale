using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
namespace HarborValeGreybox.Driving.Editor {
public static class FinchPlayerBuild {
 public static void Build(){
  if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64))throw new Exception("Windows standalone support not installed");
  PlayerSettings.productName="Harbor Vale Driving Prototype";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=UnityEngine.FullScreenMode.Windowed;PlayerSettings.runInBackground=true;
  Directory.CreateDirectory("Verification/Driving/Windows");
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity"},locationPathName="Verification/Driving/Windows/HarborVale.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
  File.WriteAllText("Verification/Driving/player-build.txt",report.summary.result+" | "+report.summary.totalSize+" bytes | errors "+report.summary.totalErrors);
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Windows driving build failed");
 }
}
}

