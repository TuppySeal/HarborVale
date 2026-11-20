using System;
using System.Collections.Generic;
using UnityEngine;
namespace HarborValeGreybox.Racing {
[Serializable] public class SpeedFineEvidence {
 public int rules=1;
 public string officerId,capturedCarId,roadClass;
 public float postedMph,measuredKph;
}
[Serializable] public class SpeedTicket {
 public string id,issuedUtc;
 public SpeedFineEvidence evidence;
 public int fullFine,paidAtIssue;
}
[Serializable] public class TrafficAccount {
 public bool initialized;
 public int assessed,paid,outstanding;
 public List<SpeedTicket> tickets=new List<SpeedTicket>();
}
[Serializable] public class TrafficPayment {public int rules=1,fullFine,paid,debtBefore;public SpeedFineEvidence evidence;}
public static class TrafficLedger {
 public const int DetectorPrice=1200;
 static void Need(bool value,string message){if(!value)throw new InvalidOperationException(message);}
 public static bool IsReceipt(TuneReceipt r)=>r.kind=="speed_fine"||r.kind=="fine_payment"||r.kind=="radar_detector";
 public static void Ensure(RaceProgress p){if(p.traffic==null)p.traffic=new TrafficAccount();p.traffic.initialized=true;}
 public static int Debt(RaceProgress p)=>p.traffic==null?0:p.traffic.outstanding;
 public static void CanSpend(RaceProgress p,int amount){if(amount>0)Need(Debt(p)==0,"Pay outstanding police fines before paid purchases, garage labor or circuit entry");}
 public static void ValidateLegacy(RaceProgress p){Need(p.traffic==null||!p.traffic.initialized&&p.traffic.assessed==0&&p.traffic.paid==0&&p.traffic.outstanding==0&&(p.traffic.tickets==null||p.traffic.tickets.Count==0),"Legacy profile contains unsupported police account data");if(p.fleet?.cars!=null)foreach(var car in p.fleet.cars)Need(!car.radarDetector,"Legacy profile contains an unsupported radar detector");}
 public static int Fine(SpeedFineEvidence e){
  Need(e!=null&&e.rules==1,"Unsupported radar evidence rules");
  bool city=e.roadClass=="city",highway=e.roadClass=="highway";
  Need(city||highway,"Unknown road class");
  Need((e.officerId=="officer_city_01"||e.officerId=="officer_city_02"||e.officerId=="officer_highway_01"||e.officerId=="officer_highway_02"),"Unknown officer");
  Need(!string.IsNullOrEmpty(e.capturedCarId),"Captured vehicle identity is required");
  Need(!float.IsNaN(e.postedMph)&&!float.IsInfinity(e.postedMph)&&!float.IsNaN(e.measuredKph)&&!float.IsInfinity(e.measuredKph)&&e.postedMph>=5&&e.postedMph<=120&&e.measuredKph>=0&&e.measuredKph<=1500,"Invalid finite radar evidence");
  double overspeed=(double)e.measuredKph/1.609344-e.postedMph;
  // Compare in the original float kph domain, so an exactly generated boundary
  // is exempt without widening the permitted speed by a rounding tolerance.
  Need(e.measuredKph>(e.postedMph+(city?10:20))*1.609344f+.0001f,"Speed is within this road's police tolerance");
  return checked(100+25*(int)Math.Ceiling(overspeed-.001));
 }
 public static bool KnownCar(FleetState fleet,string id){if(id==FleetLedger.Loaner)return true;return fleet.cars.Exists(c=>c.id==id);}
 public static void Apply(RaceProgress p,FleetState fleet,TuneReceipt r){
  Ensure(p);Need(r.pathId=="traffic_v11"&&r.stage==0&&r.credits>=0&&r.prototypeCarId==p.prototypeCarId,"Invalid traffic receipt");
  Need(!fleet.HasPending,"Resolve the committed race before police processing");
  if(r.kind=="radar_detector"){
   Need(r.credits==DetectorPrice&&r.configId=="radar_detector_v1"&&r.partId!=FleetLedger.Loaner,"Invalid detector price or chassis");
   Need(Debt(p)==0,"Outstanding fine debt blocks detector purchase");
   var car=FleetLedger.Find(fleet,r.partId);Need(car.owner=="player"&&!car.radarDetector,"Detector needs an owned chassis without an installed detector");car.radarDetector=true;return;
  }
  var move=JsonUtility.FromJson<TrafficPayment>(r.fleetJson);
  Need(move!=null&&move.rules==1&&move.paid==r.credits&&move.debtBefore==Debt(p),"Traffic payment history does not match debt");
  if(r.kind=="speed_fine"){
   Need(move.fullFine==Fine(move.evidence)&&KnownCar(fleet,move.evidence.capturedCarId)&&r.configId==move.evidence.capturedCarId&&r.partId==move.evidence.officerId,"Fine does not match captured evidence");
   // Trial earnings are aggregated before receipt replay. Never infer historical
   // available cash here: a frozen partial payment remains valid after later wins.
   Need(move.paid>=0&&move.paid<=move.fullFine,"Invalid frozen fine payment");
   p.traffic.tickets.Add(new SpeedTicket{id=r.transactionId,issuedUtc=r.paidUtc,evidence=move.evidence,fullFine=move.fullFine,paidAtIssue=move.paid});
   p.traffic.assessed=checked(p.traffic.assessed+move.fullFine);
   p.traffic.paid=checked(p.traffic.paid+move.paid);
   p.traffic.outstanding=checked(p.traffic.outstanding+move.fullFine-move.paid);return;
  }
  // JsonUtility can expand a null serializable child into an empty object.
  bool emptyEvidence=move.evidence==null||(string.IsNullOrEmpty(move.evidence.officerId)&&string.IsNullOrEmpty(move.evidence.capturedCarId)&&string.IsNullOrEmpty(move.evidence.roadClass)&&move.evidence.postedMph==0&&move.evidence.measuredKph==0);
  Need(r.kind=="fine_payment"&&r.configId=="traffic_account_v1"&&r.partId=="outstanding_fines"&&emptyEvidence&&move.fullFine==0&&move.paid>0&&move.paid<=Debt(p),"Invalid manual fine payment");
  p.traffic.paid=checked(p.traffic.paid+move.paid);p.traffic.outstanding-=move.paid;
 }
 public static void Validate(RaceProgress p){Ensure(p);Need(p.traffic.tickets!=null&&p.traffic.assessed>=0&&p.traffic.paid>=0&&p.traffic.paid<=p.traffic.assessed&&p.traffic.outstanding==p.traffic.assessed-p.traffic.paid,"Traffic account totals disagree");}
}
}
