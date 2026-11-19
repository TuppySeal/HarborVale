using System;
using System.IO;
using System.Collections;
using UnityEngine;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Garage {
// Explicit, isolated development-only checks. Never seeds the real user profile.
public class GaragePlayerSmokeProbe:MonoBehaviour {
 static bool active,reload;static string folder;FinchGarage garage;double deadline;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Prepare(){var args=Environment.GetCommandLineArgs();active=Debug.isDebugBuild&&Array.IndexOf(args,"--garage-smoke")>=0;if(!active)return;int i=Array.IndexOf(args,"--garage-smoke-dir");if(i<0||i+1>=args.Length)throw new ArgumentException("Isolated garage output required");folder=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(folder);reload=Array.IndexOf(args,"--garage-reload")>=0;DocksideTrialDirector.SavePathOverrideForTests=Path.Combine(folder,"profile-v2.json");}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(active)new GameObject("Garage player smoke probe").AddComponent<GaragePlayerSmokeProbe>();}
 void Update(){if(garage!=null&&garage.Trial!=null){var car=garage.Trial.vehicle;car.externalInput=true;car.commandedPedal=car.commandedSteer=0;car.commandedHandbrake=false;}if(deadline>0&&Time.realtimeSinceStartupAsDouble>deadline)Fail("Timeout");}
 static TrialSession Finish(TrialDefinition d){var s=new TrialSession(d);s.Tick(4,Vector3.zero,Vector3.zero);s.Tick(90,Vector3.zero,Vector3.zero);foreach(var g in d.checkpoints)s.Tick(.02f,g.Position-g.Forward,g.Position+g.Forward);return s;}
 IEnumerator Start(){
  deadline=Time.realtimeSinceStartupAsDouble+60;while(garage==null||garage.Catalog==null){garage=UnityEngine.Object.FindAnyObjectByType<FinchGarage>();yield return null;}yield return new WaitForSecondsRealtime(1);
  var store=garage.Trial.Store;if(store==null){Fail("Store missing");yield break;}
  if(reload){
   if(store.Profile.availableCredits!=700||store.Profile.engineStage!=1||garage.Trial.vehicle.Engine.definition.SampleTorque(4500)!=125){Fail("Paid tune or available credits did not survive process restart");yield break;}
   if(!garage.Open()){Fail("Reloaded garage did not open");yield break;}yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Garage_Reload_UI.png"));yield return new WaitForSecondsRealtime(.5f);garage.Close();File.WriteAllText(Path.Combine(folder,"player-reload.txt"),"PASS separate Windows process reloaded Stage 1 curve (125 Nm at 4500 RPM), installed HUD stage and 700 available credits; no repeat charge.\n");Application.Quit(0);yield break;
  }
  if(store.Profile.earnings!=0){Fail("Initial smoke profile was not empty");yield break;}store.Settle(Finish(garage.Trial.Definition),out _);if(!garage.Open()){Fail("Garage did not open");yield break;}
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Garage_Before_UI.png"));yield return new WaitForSecondsRealtime(.4f);
  if(!garage.Purchase()||store.Profile.availableCredits!=700||garage.Trial.vehicle.Engine.definition.SampleTorque(4500)!=125){Fail("Purchase failed");yield break;}
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Garage_Installed_UI.png"));yield return new WaitForSecondsRealtime(.4f);garage.Close();
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Stage1_Driving_UI.png"));yield return new WaitForSecondsRealtime(.5f);
  File.WriteAllText(Path.Combine(folder,"player-purchase.txt"),"PASS Windows player: isolated valid trial reward fixture funded 1100 credits, garage installed authored Stage 1 for 400, 700 credits remained, actual before/installed/driving UI screenshots. Real player profile untouched.\n");Application.Quit(0);
 }
 void Fail(string reason){if(!active)return;active=false;File.WriteAllText(Path.Combine(folder,reload?"player-reload.txt":"player-purchase.txt"),"FAIL "+reason);Debug.LogError(reason);Application.Quit(1);}
}
}
