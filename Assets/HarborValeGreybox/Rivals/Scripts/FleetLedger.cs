using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
[Serializable] public class FleetCar {public string id,name,owner,origin;public WorkshopBuild build;public int paint;}
[Serializable] public class DuelContract {public string id,mode,playerId,opponentId,opponentName,opponentKey;public WorkshopBuild playerBuild,opponentBuild;public float speedKph;public int winCredits,finishCredits,paint;}
[Serializable] public class FleetState {public string selectedId;public List<FleetCar> cars=new List<FleetCar>();public DuelContract pending;public bool HasPending=>pending!=null&&!string.IsNullOrEmpty(pending.id);}
[Serializable] public class RivalOption {public string id,name;public float speedKph;public int winCredits,finishCredits;public WorkshopBuild build;}
[Serializable] public class TrafficRoute {public string id;public RoutePoint[] points;}
[Serializable] public class RivalsData {public int schema,trafficCount,replacementPrice;public RivalOption[] opponents;public TrafficRoute[] routes;public static RivalsData Load(){var d=JsonUtility.FromJson<RivalsData>(Resources.Load<TextAsset>("RivalsTraffic").text);foreach(var o in d.opponents){o.build.drive="stock";o.build.tireCompound="road";o.build.tireOwned=1;o.build.sound="road";o.build.soundStage=1;if(string.IsNullOrEmpty(o.build.induction))o.build.induction="none";if(o.build.setup==null)o.build.setup=new WorkshopSetup();}return d;}}
public static class FleetLedger {
 public const string Loaner="recovery_loaner";public static WorkshopBuild Clone(WorkshopBuild b)=>JsonUtility.FromJson<WorkshopBuild>(JsonUtility.ToJson(b));
 public static FleetState Create(string id,WorkshopBuild original){return new FleetState{selectedId=id,cars=new List<FleetCar>{new FleetCar{id=id,name="Aster Finch / original",owner="player",origin="Retained prototype; no fabricated purchase or service history",build=original,paint=-1}}};}
 public static FleetCar Find(FleetState f,string id)=>f.cars.SingleOrDefault(c=>c.id==id)??throw new InvalidOperationException("Unknown vehicle identity");
 public static WorkshopBuild Active(FleetState f)=>f.selectedId==Loaner?new WorkshopBuild():Find(f,f.selectedId).build;
 public static void CanModify(FleetState f){if(f.HasPending)throw new InvalidOperationException("Finish the committed race first");if(f.selectedId==Loaner)throw new InvalidOperationException("Recovery loaner cannot be upgraded or staked");if(Find(f,f.selectedId).owner!="player")throw new InvalidOperationException("This car is no longer owned");}
 public static bool IsFleet(TuneReceipt r)=>r.kind=="duel_open"||r.kind=="duel_result"||r.kind=="fleet_select"||r.kind=="fleet_buy";
 static void Need(bool yes,string why){if(!yes)throw new InvalidOperationException(why);}
 public static void Replay(FleetState f,TuneReceipt r,string profileId){Need(r.pathId=="fleet_v7"&&r.stage==0,"Invalid fleet receipt");
  if(r.kind=="fleet_select"){Need(r.credits==0&&!f.HasPending,"Cannot switch during race");Need(r.partId==Loaner?f.cars.All(c=>c.owner!="player"):Find(f,r.partId).owner=="player","Only an owned car can be selected");f.selectedId=r.partId;return;}
  if(r.kind=="fleet_buy"){Need(r.credits>0&&!f.HasPending&&r.partId==profileId+":replacement:"+r.transactionId&&!f.cars.Any(c=>c.id==r.partId),"Invalid replacement purchase");f.cars.Add(new FleetCar{id=r.partId,name="Aster Finch / replacement",owner="player",origin="Purchased replacement / "+r.paidUtc,build=new WorkshopBuild(),paint=3});f.selectedId=r.partId;return;}
  if(r.kind=="duel_open"){Need(r.credits==0&&!f.HasPending,"Race contract already open");var d=JsonUtility.FromJson<DuelContract>(r.fleetJson);Need(d!=null&&d.id==r.transactionId&&d.opponentKey==r.partId&&d.playerId==f.selectedId&&(d.mode=="cash"||d.mode=="pinks")&&d.opponentId==profileId+":rival:"+d.id&&!f.cars.Any(c=>c.id==d.opponentId),"Invalid race terms");Need(d.playerId==Loaner?d.mode=="cash":Find(f,d.playerId).owner=="player","Cannot stake unowned/loaned car");Need(JsonUtility.ToJson(d.playerBuild)==JsonUtility.ToJson(Active(f)),"Player build changed after agreement");Need(d.speedKph>=20&&d.speedKph<=100&&d.winCredits>0&&d.finishCredits>=0&&d.finishCredits<=d.winCredits,"Invalid race reward terms");WorkshopCatalog.Load("car_finch").Resolve(d.opponentBuild);f.cars.Add(new FleetCar{id=d.opponentId,name="Aster Finch / "+d.opponentName,owner="rival",origin="Rival race stake / "+r.paidUtc,build=Clone(d.opponentBuild),paint=d.paint});f.pending=d;return;}
  var open=f.pending;Need(f.HasPending&&r.configId==open.id&&(r.partId=="win"||r.partId=="finish"||r.partId=="draw"||r.partId=="forfeit"),"Result does not match committed race");int reward=open.mode=="cash"?(r.partId=="win"?open.winCredits:(r.partId=="finish"||r.partId=="draw")?open.finishCredits:0):0;Need(r.credits==-reward,"Result payout disagrees with frozen terms");if(open.mode=="pinks"){if(r.partId=="win")Find(f,open.opponentId).owner="player";else if(r.partId!="draw"){Find(f,open.playerId).owner="rival";f.selectedId=f.cars.FirstOrDefault(c=>c.owner=="player")?.id??Loaner;}}f.pending=null;
 }
}
}
