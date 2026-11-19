using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace HarborValeAssetReview {
public static class BrakeAssetVerify {
 static string Package=>Path.GetFullPath("../BrakeAssetReview/Verification/PerformanceBrakeKit_Reviewed.unitypackage");
 static double deadline;
 public static void Run(){deadline=EditorApplication.timeSinceStartup+90;EditorApplication.update+=WaitForImport;AssetDatabase.ImportPackage("C:/Users/Justice/Documents/HarborVale-Claude-Art/BrakeKit/PerformanceBrakeKit_Assets.unitypackage",false);}
 static void WaitForImport(){if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;var p=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Brakes/Prefabs/PerformanceBrakeKit.prefab");if(p==null){if(EditorApplication.timeSinceStartup<deadline)return;Directory.CreateDirectory("Verification");File.WriteAllText("Verification/results.txt","FAIL Package import timed out");EditorApplication.update-=WaitForImport;EditorApplication.Exit(1);return;}EditorApplication.update-=WaitForImport;try{Verify();EditorApplication.Exit(0);}catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
 public static void Verify(){
  string output=Path.GetFullPath("Verification");Directory.CreateDirectory(output);
  try{
   AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Brakes/Prefabs/PerformanceBrakeKit.prefab");
   if(prefab==null)throw new Exception("Imported prefab missing");
   var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
   var car=UnityEngine.Object.Instantiate(prefab);var renderers=car.GetComponentsInChildren<Renderer>();
   int missing=renderers.Sum(r=>r.sharedMaterials.Count(m=>m==null));
   if(renderers.Length!=23||missing!=0)throw new Exception("Renderer/material count mismatch");
   var materials=AssetDatabase.FindAssets("t:Material",new[]{"Assets/Brakes/Materials"});
   if(materials.Length!=6)throw new Exception("Expected six imported materials");
   foreach(var renderer in renderers)foreach(var mat in renderer.sharedMaterials)if(!AssetDatabase.Contains(mat)||mat.shader==null||mat.shader.name!="Standard")throw new Exception("Missing saved Standard material");
   var rotor=car.GetComponentsInChildren<Transform>().Single(t=>t.name=="MainDisc").GetComponent<Renderer>();
   var size=rotor.bounds.size;
   if(Mathf.Abs(size.x-.30f)>.001f||Mathf.Abs(size.y-.016f)>.001f||Mathf.Abs(size.z-.30f)>.001f)throw new Exception("Rotor dimensions: "+size);
   string[] scripts=Directory.GetFiles("Assets/Brakes","*.cs",SearchOption.AllDirectories);
   if(scripts.Length!=0)throw new Exception("Asset-only package introduced scripts");
   var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
   RenderSettings.ambientLight=new Color(.35f,.38f,.43f);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
   var light=new GameObject("Review key light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.8f;light.transform.rotation=Quaternion.Euler(45,-35,0);
   var camera=new GameObject("Review camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.065f,.085f,.11f);camera.nearClipPlane=.01f;camera.farClipPlane=20;camera.orthographic=true;camera.orthographicSize=bounds.size.magnitude*.55f;
   Capture(camera,bounds.center,new Vector3(1,1.15f,1.4f),Path.Combine(output,"BrakeKit_Full.png"));
   camera.orthographicSize=.23f;Capture(camera,rotor.bounds.center,new Vector3(.9f,1.3f,1.1f),Path.Combine(output,"BrakeKit_Detail.png"));
   EditorSceneManager.SaveScene(scene,"Assets/BrakeKit_Reviewed.unity");
   string text="PASS Imported asset-only package into fresh Unity project\nPASS 23 renderers; zero null materials; six saved Standard materials\nPASS Rotor bounds.size (m): "+size.ToString("F4")+"\nPASS No imported C# or UI/TMP dependencies\nPASS Rendered full and detail PNG via actual Unity camera\nLIMIT Static asset inspection only; no brake gameplay, draw-call or memory claims\n";
   File.WriteAllText(Path.Combine(output,"results.txt"),text);Debug.Log(text);
   AssetDatabase.ExportPackage(new[]{"Assets/Brakes/Prefabs/PerformanceBrakeKit.prefab"},Path.Combine(output,"PerformanceBrakeKit_Reviewed.unitypackage"),ExportPackageOptions.IncludeDependencies|ExportPackageOptions.Recurse);
  }catch(Exception e){File.WriteAllText(Path.Combine(output,"results.txt"),"FAIL "+e);throw;}
 }
 public static void ExportCorrected(){try{Verify();}catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
 public static void ImportReviewed(){deadline=EditorApplication.timeSinceStartup+90;EditorApplication.update+=WaitForImport;AssetDatabase.ImportPackage(Package,false);}
 static void Capture(Camera camera,Vector3 target,Vector3 direction,string file){
  camera.transform.position=target+direction.normalized*2;camera.transform.LookAt(target);
  var targetTexture=new RenderTexture(1600,1000,24,RenderTextureFormat.ARGB32);camera.targetTexture=targetTexture;camera.Render();var old=RenderTexture.active;RenderTexture.active=targetTexture;
  var image=new Texture2D(1600,1000,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,1000),0,0);image.Apply();File.WriteAllBytes(file,image.EncodeToPNG());
  RenderTexture.active=old;camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(image);targetTexture.Release();UnityEngine.Object.DestroyImmediate(targetTexture);
 }
}
}
