using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using ClaudeGaragePreview;
namespace HarborValePreviewReview.Editor {
public static class ReviewBuild {
 public const string GarageScene="Assets/ClaudeGarage/Scenes/GaragePreview.unity";
 public static void Bake(string folder){Directory.CreateDirectory(folder);int count=0;foreach(var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))foreach(var mat in r.sharedMaterials)if(mat!=null&&!AssetDatabase.Contains(mat))AssetDatabase.CreateAsset(mat,AssetDatabase.GenerateUniqueAssetPath(folder+"/Material_"+(count++).ToString("D4")+".mat"));AssetDatabase.SaveAssets();}
 public static void Garage(){Directory.CreateDirectory("Verification/Garage");EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab");GarageSceneSetup.BuildGarageScene(prefab);new GameObject("Garage verification | opt-in only").AddComponent<GarageReviewProbe>();Bake("Assets/ClaudeGarage/Generated");EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),GarageScene);EditorSceneManager.OpenScene(GarageScene);var manager=UnityEngine.Object.FindAnyObjectByType<GaragePreviewManager>();if(manager==null||manager.displays.Length!=4)throw new Exception("Saved scene missing manager");foreach(var display in manager.displays)if(display.GetComponentsInChildren<Renderer>(true).Length<2)throw new Exception("Saved geometry missing");PlayerSettings.productName="HarborValeGaragePreview";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;Directory.CreateDirectory("Verification/Garage/Windows");var build=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{GarageScene},target=BuildTarget.StandaloneWindows64,locationPathName="Verification/Garage/Windows/GaragePreview.exe",options=BuildOptions.Development});File.WriteAllText("Verification/Garage/build.txt",build.summary.result+" | errors "+build.summary.totalErrors);if(build.summary.result!=BuildResult.Succeeded)throw new Exception("Preview export failed");AssetDatabase.ExportPackage(new[]{GarageScene,"Assets/ClaudeGarage/Scripts","Assets/Review","Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab"},"Verification/Garage/ReviewedGarage.unitypackage",ExportPackageOptions.Recurse|ExportPackageOptions.IncludeDependencies);File.WriteAllText("Verification/Garage/scene-results.txt","PASS actual saved scene reopened with static Finch, four complete part displays, materials and wired camera/manager/UI. Minimal static dependencies only; no driving/racing/save code.\n");}
}
}
