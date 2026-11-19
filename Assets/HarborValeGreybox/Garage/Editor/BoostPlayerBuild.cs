using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
namespace HarborValeGreybox.Boost.Editor {
public static class BoostPlayerBuild {
 public static void Build(){if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64))throw new Exception("Windows support unavailable");PlayerSettings.productName="Harbor Vale — Turbo Parts";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=UnityEngine.FullScreenMode.Windowed;PlayerSettings.runInBackground=true;Directory.CreateDirectory("Verification/Boost/Windows");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity"},locationPathName="Verification/Boost/Windows/HarborVale.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});File.WriteAllText("Verification/Boost/player-build.txt",report.summary.result+" | "+report.summary.totalSize+" bytes | errors "+report.summary.totalErrors);if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Boost export failed");}
}
}
