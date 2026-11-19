using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
namespace HarborValeGreybox.Racing.Editor {
public static class TrialPlayerBuild {
 public static void Build(){
  if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64))throw new Exception("Windows build support unavailable");
  PlayerSettings.productName="Harbor Vale — First Lap";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=UnityEngine.FullScreenMode.Windowed;PlayerSettings.runInBackground=true;Directory.CreateDirectory("Verification/Trial/Windows");
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity"},locationPathName="Verification/Trial/Windows/HarborVale.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
  File.WriteAllText("Verification/Trial/player-build.txt",report.summary.result+" | "+report.summary.totalSize+" bytes | errors "+report.summary.totalErrors);if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Time trial player build failed");
 }
}
}
