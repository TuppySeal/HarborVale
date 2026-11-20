using System;
using UnityEngine;
namespace HarborValeGreybox.Racing {
public sealed partial class RaceProgressStore {
 public CircuitContract BeginCircuit(string request){Refresh();if(CircuitLedger.Pending(Profile.fleet)){if(Profile.fleet.circuit.id==request)return Profile.fleet.circuit;throw new InvalidOperationException("A paid circuit is already committed");}if(Profile.fleet.HasPending)throw new InvalidOperationException("Finish the other race first");if(Profile.spending.Exists(r=>r.transactionId==request))throw new InvalidOperationException("Entry request was already completed");if(!HasEngine(Profile.fleet.selectedId))throw new InvalidOperationException("Install an engine before entering");var c=new CircuitContract{id=request,eventId=CircuitRules.EventId,playerId=Profile.fleet.selectedId,playerModelId=ActiveModelId,playerName=ActiveVehicleName,playerBuild=FleetLedger.Clone(ActiveWorkshop),rivals=CircuitRules.Field(request),entryFee=CircuitRules.Entry,perRival=CircuitRules.PerRival};var receipt=Receipt(request,"circuit_open",c.eventId,c.playerId,c.entryFee,JsonUtility.ToJson(c));receipt.pathId="circuit_v10";FleetTransaction(receipt);return Profile.fleet.circuit;}
 public void SettleCircuit(CircuitSession session){if(session==null||!session.Resolved)throw new InvalidOperationException("A resolved physical circuit is required");FinishCircuit(session.Contract,session.Result);}
 public void ForfeitCircuit(){Refresh();if(!CircuitLedger.Pending(Profile.fleet))return;var c=Profile.fleet.circuit;FinishCircuit(c,new CircuitResult{contractId=c.id,completed=false,beaten=0,playerMilliseconds=0,rivalMilliseconds=new[]{-1,-1,-1,-1,-1,-1,-1}});}
 void FinishCircuit(CircuitContract c,CircuitResult result){var r=Receipt(CircuitRules.ResultId(c.id),"circuit_result",result.completed?"finish":"forfeit",c.id,-checked(result.beaten*c.perRival),JsonUtility.ToJson(result));r.pathId="circuit_v10";FleetTransaction(r);}
}
}
