using System;
using System.IO;
using System.Linq;
using UnityEngine;
namespace HarborValeGreybox.Racing.Editor {
public static class HighwayChallengeVerification {
 public static void Run(){
  Directory.CreateDirectory("Verification/Highway");
  var report="Verification/Highway/traffic-rules.txt";
  File.WriteAllText(report,"");
  void Check(bool ok,string label){
   File.AppendAllText(report,(ok?"PASS ":"FAIL ")+label+Environment.NewLine);
   if(!ok)throw new InvalidOperationException(label);
  }
  var n=HighwayNetwork.Load();
  var deck=n.Carriageway(false)[320];
  Check(PolicePatrol.PostedLimit(deck,out var highway)==70&&highway,"Elevated road uses 70 mph limit");
  PolicePatrol.PostedLimit(new Vector3(deck.x,HighwayNetwork.GroundHeight(deck.z),deck.z),out highway);
  Check(!highway,"Road beneath deck does not inherit highway limit");
  var id=Guid.NewGuid().ToString("N");
  var fixture=Environment.GetEnvironmentVariable("HARBORVALE_LEGACY_FIXTURE");
  string bytes=File.ReadAllText(fixture);
  var old=JsonUtility.FromJson<RaceProgress>(bytes);
  Check(old.version==10,"Fixture was written by released v10 player");
  var route=JsonUtility.FromJson<TrialDefinition>(Resources.Load<TextAsset>("FirstDocksideTrial").text);
  var store=new RaceProgressStore(Path.GetFullPath("Verification/Highway/"+id+".json"),route,trafficLegacyFile:fixture);
  Check(store.Profile.version==11&&store.Profile.profileId==old.profileId&&store.Profile.availableCredits==old.availableCredits&&store.Profile.spending.Count==old.spending.Count&&File.ReadAllText(fixture)==bytes,"Migration preserves original wallet, receipts and v10 bytes");
  var evidence=new SpeedFineEvidence{officerId="officer_city_01",capturedCarId=store.Profile.fleet.selectedId,roadClass="city",postedMph=30,measuredKph=DrivingUnits.FromMph(40)};
  bool rejected=false;
  try{TrafficLedger.Fine(evidence);}catch(InvalidOperationException){rejected=true;}catch(ArgumentException){rejected=true;}
  Check(rejected,"City tolerance includes exactly ten mph over");
  evidence.measuredKph=DrivingUnits.FromMph(50);
  store.IssueSpeedFine(id,evidence);
  int debt=store.OutstandingFines;
  store.IssueSpeedFine(id,evidence);
  var loaded=new RaceProgressStore(store.PathName,route);
  Check(loaded.OutstandingFines==debt&&loaded.Profile.traffic.tickets.Count==1,"Citation retries and reload preserve a single frozen fine");
  var option=RivalsData.Load().opponents.Single(o=>o.id=="expressway");
  Check(option.routeId==HighwayEvent.Id&&option.modelId=="car_tempest","Expressway event uses the original Tempest");
 }
 public static void Release(){Run();HarborValeGreybox.Presentation.Editor.CityArtVerification.Release();}
}
}
