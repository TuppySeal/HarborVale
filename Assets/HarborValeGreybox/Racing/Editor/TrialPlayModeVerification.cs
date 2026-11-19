using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing.Editor {
[InitializeOnLoad]public static class TrialPlayModeVerification {
 const string Armed="HarborVale.Trial.PlayCheck";static double started;static int phase;static bool failed;static string report="";static int crossingPhase;static int gateBefore;static float phaseTime;
 static TrialPlayModeVerification(){if(SessionState.GetBool(Armed,false)){DocksideTrialDirector.SavePathOverrideForTests=SessionState.GetString(Armed+".Path","");EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;Application.logMessageReceived+=Log;started=EditorApplication.timeSinceStartup;}}
 public static void Begin(){Directory.CreateDirectory("Verification/Trial");SessionState.SetString(Armed+".Path",Path.GetFullPath("Verification/Trial/Play_"+Guid.NewGuid().ToString("N")+".json"));SessionState.SetBool(Armed,true);EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");EditorApplication.isPlaying=true;}
 static void Log(string message,string stack,LogType type){if(type!=LogType.Exception&&type!=LogType.Error)return;if(stack.Contains("UnityEditor.Search.SearchDatabase")){report+="EDITOR-ONLY ISSUE: known Unity search indexing exception (K001).\n";return;}failed=true;report+="ERROR "+message+"\n";}
 static void Check(bool ok,string text){report+=(ok?"PASS ":"FAIL ")+text+"\n";if(!ok)failed=true;}
 static void State(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredEditMode)return;SessionState.SetBool(Armed,false);File.WriteAllText("Verification/Trial/playmode-results.txt",report);EditorApplication.Exit(failed?1:0);}
 static void Tick(){
  if(!EditorApplication.isPlaying){if(EditorApplication.timeSinceStartup-started>150){failed=true;report+="FAIL Play startup timeout\n";State(PlayModeStateChange.EnteredEditMode);}return;}
  EditorApplication.QueuePlayerLoopUpdate();var director=UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>();if(director==null||director.Definition==null)return;var car=director.vehicle;car.externalInput=true;car.commandedPedal=0;car.commandedSteer=0;car.commandedHandbrake=false;float t=Time.timeSinceLevelLoad;
  if(phase==0&&t>2){Check(director.Store!=null&&director.Store.Profile.earnings==0,"Isolated race profile loads; real user progress untouched");Check(director.BeginRun()&&car.controlsLocked,"Start creates countdown and locks driving");phase=1;phaseTime=t;}
  if(phase==1&&t-phaseTime>3.5f){Check(director.Session.State==TrialState.Running&&!car.controlsLocked,"Countdown reaches GO and unlocks vehicle");float elapsed=director.Session.Elapsed;int gate=director.Session.NextGate;car.Recover();Check(director.Session.NextGate==gate&&Mathf.Abs(director.Session.Elapsed-elapsed-3)<.002f,"Vehicle R recovery routes through race penalty policy (delta "+(director.Session.Elapsed-elapsed).ToString("F6")+", gates "+gate+"/"+director.Session.NextGate+")");var hud=car.GetComponent<FinchDrivingHUD>();hud.SetInspection(true);phase=2;phaseTime=t;}
  if(phase==2&&t-phaseTime>1){Check(director.Session.Elapsed>3.5f,"Race clock continues while map inspection is active");car.GetComponent<FinchDrivingHUD>().SetInspection(false);phase=3;}
  if(phase==3){
   // Scripted gate crossings verify runtime wiring; this is not a driven full-lap playtest.
   if(director.Session.State==TrialState.Running){var gate=director.Definition.checkpoints[director.Session.NextGate];if(crossingPhase==0){car.ResetAt(gate.Position-gate.Forward*2+Vector3.up*.4f,Quaternion.LookRotation(gate.Forward));gateBefore=director.Session.NextGate;crossingPhase=1;}else if(crossingPhase==1){car.ResetAt(gate.Position+gate.Forward*2+Vector3.up*.4f,Quaternion.LookRotation(gate.Forward));crossingPhase=2;}else{Check(director.Session.NextGate>gateBefore,"Runtime checkpoint "+(gateBefore+1)+" advances in order");crossingPhase=0;}}
   else{Check(director.ShowingResult&&director.Receipt!=null&&director.Store.Profile.earnings==1100,"Finish displays results and commits first gold payout once");director.TrySettlement();Check(director.Store.Profile.earnings==1100&&director.Store.Profile.receipts.Count==1,"Repeated runtime settlement cannot duplicate reward");Render("Verification/Trial/Trial_Result.png");phase=4;phaseTime=t;}
  }
  if(phase==4&&t-phaseTime>.5f){director.DismissResult();Check(!car.controlsLocked&&!director.ShowingResult,"Free-roam return releases car controls");director.BeginRun();car.GetComponent<FinchDrivingHUD>().SetInspection(true);director.Abandon();Check(director.Receipt==null&&director.Store.Profile.earnings==1100,"Abandon awards nothing");Check(car.playerInput&&!Camera.main.GetComponent<MapExplorer>().enabled,"Abandon from inspection restores result keyboard/camera controls");director.DismissResult();director.BeginRun();phase=5;phaseTime=t;}
  if(phase==5&&t-phaseTime>3.4f){Render("Verification/Trial/Trial_Route.png");Check(!failed,"No gameplay runtime exceptions");EditorApplication.isPlaying=false;}
  if(t>120){failed=true;report+="FAIL runtime timeout\n";EditorApplication.isPlaying=false;}
 }
 static void Render(string file){var cam=Camera.main;var rt=new RenderTexture(1600,1000,24);cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var tex=new Texture2D(1600,1000,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,1000),0,0);tex.Apply();File.WriteAllBytes(file,tex.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);}
}
}
