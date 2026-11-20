using System;
using UnityEngine;
namespace HarborValeGreybox.Racing {
public sealed partial class RaceProgressStore {
 public int OutstandingFines=>TrafficLedger.Debt(Profile);
 public bool HasRadarDetector(string carId)=>carId!=FleetLedger.Loaner&&FleetLedger.Find(Profile.fleet,carId).radarDetector;
 public void IssueSpeedFine(string request,SpeedFineEvidence evidence)=>TrafficTransaction(request,"speed_fine",evidence,null,0);
 public void PaySpeedFines(string request)=>TrafficTransaction(request,"fine_payment",null,null,0);
 public void BuyRadarDetector(string request,string carId,int quotedPrice)=>TrafficTransaction(request,"radar_detector",null,carId,quotedPrice);
 void TrafficTransaction(string request,string kind,SpeedFineEvidence evidence,string carId,int quote){
  if(!Guid.TryParse(request,out _))throw new ArgumentException("Invalid traffic transaction identity");
  // Detach caller-owned mutable evidence before acquiring the save lock.
  string evidenceJson=evidence==null?null:JsonUtility.ToJson(evidence);
  using(Lock()){
   var latest=Load();Profile=latest;
   foreach(var old in latest.spending)if(old.transactionId==request){
    if(old.kind!=kind)throw new InvalidOperationException("Traffic request conflicts with a committed transaction");
    if(kind=="radar_detector"){if(old.partId!=carId||old.credits!=quote)throw new InvalidOperationException("Detector request changed");}
    else if(kind=="speed_fine"){var previous=JsonUtility.FromJson<TrafficPayment>(old.fleetJson);if(previous==null||JsonUtility.ToJson(previous.evidence)!=evidenceJson)throw new InvalidOperationException("Captured fine evidence changed");}
    return;
   }
   if(latest.fleet.HasPending)throw new InvalidOperationException("Resolve or forfeit the active event before processing police fines");
   var receipt=new TuneReceipt{transactionId=request,prototypeCarId=latest.prototypeCarId,kind=kind,pathId="traffic_v11",stage=0,paidUtc=DateTime.UtcNow.ToString("o")};
   int debt=TrafficLedger.Debt(latest);
   if(kind=="radar_detector"){
    TrafficLedger.CanSpend(latest,TrafficLedger.DetectorPrice);
    if(quote!=TrafficLedger.DetectorPrice)throw new InvalidOperationException("Radar detector quote changed");
    if(carId==FleetLedger.Loaner)throw new InvalidOperationException("Recovery loaner cannot be modified");
    var car=FleetLedger.Find(latest.fleet,carId);if(car.owner!="player"||car.radarDetector)throw new InvalidOperationException("Choose an owned chassis without a detector");
    if(latest.availableCredits<quote)throw new InvalidOperationException("Not enough credits for radar detector");
    receipt.partId=carId;receipt.configId="radar_detector_v1";receipt.credits=quote;
   }else{
    var move=new TrafficPayment{debtBefore=debt};
    if(kind=="speed_fine"){
     move.evidence=evidenceJson==null?null:JsonUtility.FromJson<SpeedFineEvidence>(evidenceJson);move.fullFine=TrafficLedger.Fine(move.evidence);
     if(!TrafficLedger.KnownCar(latest.fleet,move.evidence.capturedCarId))throw new InvalidOperationException("Unknown captured vehicle");
     move.paid=Math.Min(latest.availableCredits,move.fullFine);receipt.configId=move.evidence.capturedCarId;receipt.partId=move.evidence.officerId;
    }else{
     if(debt<=0)throw new InvalidOperationException("No police fines are outstanding");
     move.paid=Math.Min(latest.availableCredits,debt);if(move.paid<=0)throw new InvalidOperationException("Earn credits before paying outstanding fines");
     receipt.configId="traffic_account_v1";receipt.partId="outstanding_fines";
    }
    receipt.credits=move.paid;receipt.fleetJson=JsonUtility.ToJson(move);
   }
   var candidate=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(latest));candidate.spending.Add(receipt);candidate.availableCredits=checked(candidate.availableCredits-receipt.credits);Validate(candidate,true);Write(candidate);Profile=candidate;
  }
 }
}
}
