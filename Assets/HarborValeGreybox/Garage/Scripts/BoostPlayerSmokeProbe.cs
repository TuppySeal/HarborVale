using System;
using System.IO;
using System.Collections;
using UnityEngine;
using HarborValeGreybox.Garage;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Boost {
public class BoostPlayerSmokeProbe:MonoBehaviour {
 static bool active,reload;static string folder;FinchGarage garage;double deadline;float pedal;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Prepare(){var args=Environment.GetCommandLineArgs();active=Debug.isDebugBuild&&Array.IndexOf(args,"--boost-smoke")>=0;if(!active)return;int i=Array.IndexOf(args,"--boost-smoke-dir");if(i<0||i+1>=args.Length)throw new ArgumentException("Isolated boost directory required");folder=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(folder);reload=Array.IndexOf(args,"--boost-reload")>=0;DocksideTrialDirector.SavePathOverrideForTests=Path.Combine(folder,"profile-v3.json");}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Install(){if(active)new GameObject("Isolated boost player probe").AddComponent<BoostPlayerSmokeProbe>();}
 void Update(){if(garage!=null&&garage.Trial!=null){var car=garage.Trial.vehicle;car.externalInput=true;car.commandedPedal=pedal;car.commandedSteer=0;car.commandedHandbrake=false;}if(deadline>0&&Time.realtimeSinceStartupAsDouble>deadline)Fail("Timeout");}
 static TrialSession Finish(TrialDefinition d){var s=new TrialSession(d);s.Tick(4,Vector3.zero,Vector3.zero);s.Tick(90,Vector3.zero,Vector3.zero);foreach(var g in d.checkpoints)s.Tick(.02f,g.Position-g.Forward,g.Position+g.Forward);return s;}
 IEnumerator Start(){deadline=Time.realtimeSinceStartupAsDouble+60;while(garage==null||garage.Catalog==null){garage=UnityEngine.Object.FindAnyObjectByType<FinchGarage>();yield return null;}yield return new WaitForSecondsRealtime(1);var trial=garage.Trial;if(trial.Store==null){Fail("Store missing");yield break;}
  if(reload){var p=trial.Store.Profile;if(p.engineStage!=2||p.gearboxStage!=1||p.intercoolerStage!=1||p.availableCredits!=250||trial.vehicle.Engine.definition.SampleTorque(4500)!=162||Mathf.Abs(trial.vehicle.Engine.definition.shiftSeconds-.396f)>.0001f){Fail("Restart did not restore complete build");yield break;}if(!garage.Open()){Fail("Reload shop unavailable");yield break;}garage.Select(3);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Boost_Reload_UI.png"));yield return new WaitForSecondsRealtime(.5f);garage.Close();File.WriteAllText(Path.Combine(folder,"player-reload.txt"),"PASS separate Windows process restored turbo Stage 2, gearbox 1, street intercooler 1, 162 Nm curve, 0.396 s shifts and 250 credits.\n");Application.Quit(0);yield break;}
  if(trial.Store.Profile.earnings!=0){Fail("Expected fresh isolated profile");yield break;}for(int i=0;i<4;i++)trial.Store.Settle(Finish(trial.Definition),out _);if(!garage.Open()){Fail("Could not open parts shop");yield break;}
  garage.Select(2);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Turbo_Prerequisites_UI.png"));yield return new WaitForSecondsRealtime(.3f);
  for(int i=0;i<3;i++){garage.Select(i);if(!garage.BuySelected()){Fail("Part purchase "+i+" failed");yield break;}}
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Turbo_Installed_UI.png"));yield return new WaitForSecondsRealtime(.3f);garage.Select(3);if(!garage.BuySelected()){Fail("Street intercooler failed");yield break;}
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Intercooler_Installed_UI.png"));yield return new WaitForSecondsRealtime(.3f);garage.Close();pedal=1;yield return new WaitForSecondsRealtime(3);
  if(trial.vehicle.Engine.TurboAvailability<.05f||trial.vehicle.Engine.BoostBar<=0){Fail("Driving did not produce loaded boost");yield break;}
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,"Loaded_Boost_HUD.png"));pedal=0;yield return new WaitForSecondsRealtime(.5f);
  var state=trial.Store.Profile;if(state.availableCredits!=250||state.spending.Count!=4){Fail("Incorrect paid receipt balance");yield break;}File.WriteAllText(Path.Combine(folder,"player-purchase.txt"),"PASS Windows player: four isolated valid-finish reward fixtures fund Stage 1 (400), gearbox (250), turbo kit (1900), street cooler (400); all installed with 250 credits remaining. Loaded driving produced real per-car boost; actual prerequisite/turbo/cooler/driving UI captures. No user progress seeded.\n");Application.Quit(0);
 }
 void Fail(string text){if(!active)return;active=false;File.WriteAllText(Path.Combine(folder,reload?"player-reload.txt":"player-purchase.txt"),"FAIL "+text);Debug.LogError(text);Application.Quit(1);}
}
}
