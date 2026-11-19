using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace HarborValeGreybox.Presentation.Editor {
public static class PresentationVerification {
 public static void Run(){Directory.CreateDirectory("Verification/Presentation");var lines=new List<string>();Action<bool,string> check=(good,text)=>{lines.Add((good?"PASS ":"FAIL ")+text);if(!good)throw new Exception(text);};try{
  foreach(float distance in new[]{1f,5f,20f,100f})foreach(int pixels in new[]{720,1080}){float height=12;float scale=DrivingBillboard.ScreenScale(height,distance,65,pixels);float projected=height/Math.Max(1,distance)*pixels/(2*Mathf.Tan(65*Mathf.Deg2Rad*.5f))*scale;check(projected<=42.01f,"Driving marker height bounded at distance "+distance+" / "+pixels+"p");}
  check(DrivingBillboard.ScreenScale(0,0,65,720)==1,"Zero-size marker cannot divide by zero");
  var scene=EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");var roots=scene.GetRootGameObjects();var root=roots.First(g=>g.name.StartsWith("HARBOR VALE | scaled"));check(root.GetComponentsInChildren<MeshRenderer>().Length>1000,"Saved source city retains authored geometry");check(root.GetComponentsInChildren<Collider>().Length>500,"Saved source city retains physical map colliders");check(root.GetComponentsInChildren<StarterCarSpecification>().Length==1,"Authored starter exists and is identified for exclusion from static batches");check(LayerMask.LayerToName(28)==""&&LayerMask.LayerToName(29)=="","Presentation detail layers do not overwrite named project layers");
  File.WriteAllLines("Verification/Presentation/rules.txt",lines);
 }catch(Exception e){lines.Add(e.ToString());File.WriteAllLines("Verification/Presentation/rules.txt",lines);throw;}}
 public static void CompareRelease(){Run();PresentationBuild.Both();}
 public static void Release(){Run();PresentationBuild.Enhanced();}
}
}
