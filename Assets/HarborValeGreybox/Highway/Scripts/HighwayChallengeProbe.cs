using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarborValeGreybox.Launch;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
public sealed class HighwayChallengeProbe:MonoBehaviour {
 static string folder;
 static bool failed;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
 static void Prepare(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--highway-challenge-check");
  if(!Debug.isDebugBuild||i<0||i+1>=args.Length)return;
  folder=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(folder);
  DocksideTrialDirector.SavePathOverrideForTests=Path.Combine(folder,"isolated.json");
 }
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 static void Hook(){if(folder==null)return;var root=new GameObject("Expressway challenge verification");DontDestroyOnLoad(root);root.AddComponent<HighwayChallengeProbe>();}
 void Check(bool good,string label){
  File.AppendAllText(Path.Combine(folder,"checks.txt"),(good?"PASS ":"FAIL ")+label+Environment.NewLine);
  if(!good){failed=true;Application.Quit(1);throw new Exception(label);}
 }
 void OnEnable(){Application.logMessageReceived+=Log;}
 void OnDisable(){Application.logMessageReceived-=Log;}
 void Log(string text,string trace,LogType kind){if(!failed&&(kind==LogType.Error||kind==LogType.Exception))Check(false,text);}
 IEnumerator Start(){
  Application.runInBackground=true;
  yield return SceneManager.LoadSceneAsync(HarborValeLaunch.CityScene);
  while(RivalRaceDirector.Instance?.Ready!=true)yield return null;
  var race=RivalRaceDirector.Instance;
  var world=HighwayTrafficWorld.Instance;
  var patrols=FindObjectsByType<PoliceContact>(FindObjectsSortMode.None);
  Check(patrols.Length==4&&patrols.All(p=>p.Car.Wheels.Length==4),"Four physical patrol cars");
  Check(patrols.All(p=>p.RadarOn(-p.Phase)&&!p.RadarOn(16-p.Phase)),"Patrol radar alternates on/off");
  var store=race.Trial.Store;
  string carId=store.Profile.fleet.selectedId;
  string ticket=Guid.NewGuid().ToString("N");
  var evidence=new SpeedFineEvidence{officerId="officer_highway_01",capturedCarId=carId,roadClass="highway",postedMph=70,measuredKph=DrivingUnits.FromMph(95)};
  store.IssueSpeedFine(ticket,evidence);int debt=store.OutstandingFines;store.IssueSpeedFine(ticket,evidence);
  Check(debt==725&&store.OutstandingFines==debt,"Speed citation records debt exactly once");
  foreach(var officer in patrols)Destroy(officer.gameObject);
  race.Trial.vehicle.controlsLocked=false;
  Check(race.OpenPanel(),"Rivals menu available");
  race.SelectedRival="expressway";
  Check(race.StartRace()&&race.Race.Contract.routeId==HighwayEvent.Id,"Tempest highway duel starts through normal race menu");
  var driver=race.Opponent.GetComponent<HighwayDriver>();
  Check(driver!=null&&driver.IsRacer&&driver.Car.PowertrainCarId=="car_tempest"&&race.Race.Player.Definition.lengthM>10000,"Full expressway route and authored Tempest");
  for(int i=0;i<12;i++)world.Spawn("Traffic fixture "+i,new WorkshopBuild(),"car_finch",false,HighwayEvent.StartIndex+27+i*19,i%3);
  Time.timeScale=12;Time.maximumDeltaTime=1.4f;Application.targetFrameRate=20;
  float peak=0;int changes=0,lastLane=driver.Lane;float deadline=Time.realtimeSinceStartup+190,nextSample=0;
  while(!race.Race.Rival.ValidFinish&&!race.Race.Resolved&&Time.realtimeSinceStartup<deadline){
   peak=Mathf.Max(peak,driver.Car.SpeedKph);if(Time.realtimeSinceStartup>=nextSample){nextSample=Time.realtimeSinceStartup+5;File.AppendAllText(Path.Combine(folder,"telemetry.txt"),$"gate={race.Race.Rival.NextGate} index={driver.Index} kph={driver.Car.SpeedKph:F1} target={driver.TargetKph:F1} time={race.Race.Clock:F1} cars={HighwayDriver.Active.Count}\n");}
   if(lastLane!=driver.Lane){changes++;lastLane=driver.Lane;}
   yield return null;
  }
  Time.timeScale=1;
  Check(race.Race.Rival.ValidFinish,"Tempest completed physical traffic lap: gates="+race.Race.Rival.NextGate+" peak="+peak.ToString("F1")+" lane changes="+changes);
  Check(peak>145&&changes>0,"Tempest reaches race pace and changes lanes");
  Check(race.BeforeLeave()&&store.Profile.fleet.pending==null,"Abandoned player run settles through normal ledger");
  Application.Quit(0);
 }
}
}
