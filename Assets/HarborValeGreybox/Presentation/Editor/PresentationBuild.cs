using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
namespace HarborValeGreybox.Presentation.Editor {
public static class PresentationBuild {
 static void Prepare(){HarborValeGreybox.Launch.Editor.LaunchBuild.Prepare();string path="Assets/HarborValeGreybox/Presentation/Resources/CityUnitCube.asset";if(AssetDatabase.LoadAssetAtPath<Mesh>(path)==null){var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);var original=cube.GetComponent<MeshFilter>().sharedMesh;var mesh=new Mesh{name="HarborVale readable unit cube",vertices=original.vertices,normals=original.normals,uv=original.uv,triangles=original.triangles,bounds=original.bounds};AssetDatabase.CreateAsset(mesh,path);UnityEngine.Object.DestroyImmediate(cube);AssetDatabase.SaveAssets();}PlayerSettings.productName="Harbor Vale";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;}
 public static void Both(){Baseline();Enhanced();}
 public static void Baseline(){Prepare();PlayerSettings.colorSpace=ColorSpace.Gamma;Build("Baseline");}
 public static void Enhanced(){Prepare();PlayerSettings.colorSpace=ColorSpace.Linear;Build("Windows");}
 static void Build(string folder){string path="Verification/Presentation/"+folder;Directory.CreateDirectory(path);var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=HarborValeGreybox.Launch.Editor.LaunchBuild.Scenes,locationPathName=path+"/HarborVale.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});File.WriteAllText(path+"/build.txt",result.summary.result+" | errors "+result.summary.totalErrors);if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Rendering build failed");}
}
}
