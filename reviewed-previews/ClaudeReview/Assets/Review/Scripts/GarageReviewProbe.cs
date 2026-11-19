using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClaudeGaragePreview;
namespace HarborValePreviewReview {
public class GarageReviewProbe:MonoBehaviour {
 string folder;bool failed;List<string> lines=new List<string>();double deadline;
 void Check(bool yes,string text){lines.Add((yes?"PASS ":"FAIL ")+text);if(!yes){failed=true;Write();Application.Quit(1);throw new Exception(text);}}
 void Write(){if(folder!=null)File.WriteAllLines(Path.Combine(folder,"native-results.txt"),lines);}
 void Log(string m,string stack,LogType t){if(t==LogType.Error||t==LogType.Exception){failed=true;lines.Add("ERROR "+m);Write();Application.Quit(1);}}
 IEnumerator Still(string name){PreviewRecorder.SuppressNextFrame=true;yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,name+".png"));yield return new WaitForSecondsRealtime(.2f);}
 void Update(){if(folder!=null&&!failed&&Time.realtimeSinceStartupAsDouble>deadline){failed=true;lines.Add("FAIL timeout");Write();Application.Quit(1);}}
 IEnumerator Start(){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--review-dir");if(i<0||i+1>=args.Length)yield break;folder=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(folder);deadline=Time.realtimeSinceStartupAsDouble+60;Application.logMessageReceived+=Log;yield return new WaitForSecondsRealtime(1);var m=UnityEngine.Object.FindAnyObjectByType<GaragePreviewManager>();Check(m!=null&&m.displays.Length==4,"Scene opens with all four wired displays");Check(GameObject.Find("Aster Finch | static original visual")!=null,"Original Finch visual loaded; no cube substitute");Check(UnityEngine.Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None).Length==0,"Preview creates no car physics");var record=gameObject.AddComponent<PreviewRecorder>();record.Begin(Path.Combine(folder,"recording"));yield return Still("Garage_Full");yield return new WaitForSecondsRealtime(1);
  for(int part=0;part<4;part++){m.Select(part);yield return null;Check(m.SelectedId==GaragePreviewManager.Parts[part].id&&m.displays[part].activeSelf,"Selection shows "+m.SelectedId);Check(m.displays[part].GetComponentsInChildren<Renderer>().Length>1,"Actual part geometry exists");m.ToggleFitment();Check(m.fitted[part],"Mock fitment toggles on");m.ToggleFitment();Check(!m.fitted[part],"Mock fitment toggles off");if(part==2)Check(m.displays[part].transform.Find("Included basic intercooler")!=null,"Turbo package includes distinct basic cooler");Vector3 focus=m.cameraOrbit.target.position;m.cameraOrbit.target.position=m.displays[part].transform.position;m.cameraOrbit.distance=2.7f;m.cameraOrbit.Apply();yield return Still("Part_"+m.SelectedId);m.cameraOrbit.target.position=focus;m.cameraOrbit.ResetView();yield return new WaitForSecondsRealtime(1.2f);}
  var cam=m.cameraOrbit;Vector3 before=cam.transform.position;cam.Orbit(25,8);cam.Zoom(-2);Check(Vector3.Distance(before,cam.transform.position)>1,"Orbit and zoom action changes real camera");yield return Still("Garage_Zoom");yield return new WaitForSecondsRealtime(1);cam.ResetView();Check(Mathf.Abs(cam.distance-10)<.001f,"Reset view restores camera distance");m.MenuOpen=false;yield return Still("Garage_MenuClosed");m.MenuOpen=true;Check(m.MenuOpen,"Menu reopens");yield return new WaitForSecondsRealtime(.5f);record.Finish();yield return new WaitForSecondsRealtime(.3f);Check(!failed,"No native runtime exceptions");Write();Application.Quit(0);
 }
}
}
