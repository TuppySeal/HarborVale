using System;
using System.IO;
using System.Collections;
using UnityEngine;
namespace HarborValeGreybox.Racing {
// Explicit development-build test mode. Never runs in ordinary play and never
// uses the real race profile. Captures the actual player UI after scripted gates.
public class TrialPlayerSmokeProbe:MonoBehaviour {
 static bool enabledForRun;static string folder;DocksideTrialDirector director;double deadline;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Prepare(){
  var args=Environment.GetCommandLineArgs();enabledForRun=Debug.isDebugBuild&&Array.IndexOf(args,"--trial-smoke")>=0;if(!enabledForRun)return;
  int index=Array.IndexOf(args,"--trial-smoke-dir");if(index<0||index+1>=args.Length)throw new ArgumentException("Smoke test requires isolated output directory");folder=Path.GetFullPath(args[index+1]);Directory.CreateDirectory(folder);DocksideTrialDirector.SavePathOverrideForTests=Path.Combine(folder,"profile_"+Guid.NewGuid().ToString("N")+".json");
 }
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(enabledForRun)new GameObject("Isolated player smoke probe").AddComponent<TrialPlayerSmokeProbe>();}
 void Update(){if(director!=null&&director.vehicle!=null){director.vehicle.externalInput=true;director.vehicle.commandedPedal=director.vehicle.commandedSteer=0;director.vehicle.commandedHandbrake=false;}if(deadline>0&&Time.realtimeSinceStartupAsDouble>deadline)Fail("Player test timeout");}
 IEnumerator Start(){
  deadline=Time.realtimeSinceStartupAsDouble+60;
  while(director==null||director.Definition==null){director=UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>();yield return null;}
  yield return new WaitForSecondsRealtime(1);if(director.Store==null||director.Store.Profile.earnings!=0){Fail("Isolated profile missing");yield break;}
  yield return new WaitForEndOfFrame();Capture("Trial_Offer_UI.png");yield return new WaitForSecondsRealtime(.3f);
  if(!director.BeginRun()){Fail("Could not begin trial");yield break;}
  while(director.Session.State==TrialState.Countdown)yield return null;
  yield return new WaitForEndOfFrame();Capture("Trial_Racing_UI.png");yield return new WaitForSecondsRealtime(.3f);
  for(int i=0;i<director.Definition.checkpoints.Length;i++){
   var gate=director.Definition.checkpoints[i];director.vehicle.ResetAt(gate.Position-gate.Forward*2+Vector3.up*.4f,Quaternion.LookRotation(gate.Forward));yield return null;yield return null;
   director.vehicle.ResetAt(gate.Position+gate.Forward*2+Vector3.up*.4f,Quaternion.LookRotation(gate.Forward));yield return null;yield return null;
   if(director.Session.NextGate!=i+1){Fail("Player gate "+(i+1)+" did not advance");yield break;}
  }
  if(!director.ShowingResult||director.Receipt==null||director.Store.Profile.earnings!=1100){Fail("Player finish/payout failed");yield break;}
  yield return new WaitForEndOfFrame();Capture("Trial_Results_UI.png");yield return new WaitForSecondsRealtime(1);
  var reload=new RaceProgressStore(director.Store.PathName,director.Definition);if(reload.Profile.earnings!=1100||reload.Profile.receipts.Count!=1){Fail("Player saved reward did not reload");yield break;}
  File.WriteAllText(Path.Combine(folder,"player-smoke.txt"),"PASS Windows player: isolated profile, countdown, 37 scripted ordered gate crossings, results, 1100-credit first gold payout, disk reload, actual offer/race/results UI PNGs. Scripted crossings do not measure a physically driven lap or medal pacing.\n");Application.Quit(0);
 }
 void Fail(string text){if(!enabledForRun)return;enabledForRun=false;File.WriteAllText(Path.Combine(folder,"player-smoke.txt"),"FAIL "+text);Debug.LogError(text);Application.Quit(1);}
 static void Capture(string name){ScreenCapture.CaptureScreenshot(Path.Combine(folder,name));}
}
}
