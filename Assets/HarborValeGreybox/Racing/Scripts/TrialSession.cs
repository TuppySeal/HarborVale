using System;
using UnityEngine;
namespace HarborValeGreybox.Racing {
[Serializable] public class RoutePoint {public float x,y,z;public Vector3 Position=>new Vector3(x,y,z);}
[Serializable] public class TrialGate:RoutePoint {public float forwardX,forwardZ,halfWidth;public Vector3 Forward=>new Vector3(forwardX,0,forwardZ).normalized;}
[Serializable] public class TrialDefinition {
 public string id,title,description;public float lengthM,countdownSeconds,recoveryPenaltySeconds,goldSeconds,silverSeconds,bronzeSeconds;public int baseReward,firstClearBonus,maxReputation;public int[] creditBps,repBps;public RoutePoint[] route;public TrialGate[] checkpoints;
 public void Validate(){if((id!="evt_dock_01"&&id!=HighwayEvent.Id)||checkpoints==null||checkpoints.Length<3||route==null||route.Length<3||baseReward<0||firstClearBonus<0||maxReputation<0||countdownSeconds<=0||recoveryPenaltySeconds<0||goldSeconds<=0||silverSeconds<=goldSeconds||bronzeSeconds<=silverSeconds||creditBps==null||creditBps.Length!=4||repBps==null||repBps.Length!=4)throw new ArgumentException("Invalid trial definition");foreach(var gate in checkpoints)if(gate.halfWidth<=0||gate.Forward.sqrMagnitude<.9f)throw new ArgumentException("Invalid checkpoint");foreach(var bps in creditBps)if(bps<0||bps>10000)throw new ArgumentException("Invalid credit multiplier");foreach(var bps in repBps)if(bps<0||bps>10000)throw new ArgumentException("Invalid reputation multiplier");}
 public int Credits(int rank)=>(int)((long)baseReward*creditBps[rank]/10000);
 public int Reputation(int rank)=>(int)((long)maxReputation*repBps[rank]/10000);
 public int Rank(float seconds)=>seconds<=goldSeconds?0:seconds<=silverSeconds?1:seconds<=bronzeSeconds?2:3;
 public static string Medal(int rank)=>rank==0?"GOLD":rank==1?"SILVER":rank==2?"BRONZE":"FINISH";
}
public enum TrialState {Countdown,Running,Finished,Abandoned}
// Pure race rules: no input, scene objects, currency IO or vehicle mutation.
public sealed class TrialSession {
 public readonly TrialDefinition Definition;public readonly string RunId;
 public TrialState State{get;private set;}=TrialState.Countdown;public int NextGate{get;private set;}public float Elapsed{get;private set;}public float Penalty{get;private set;}public float Countdown{get;private set;}
 public bool ValidFinish=>State==TrialState.Finished&&NextGate==Definition.checkpoints.Length;
 public TrialSession(TrialDefinition definition){definition.Validate();Definition=definition;RunId=Guid.NewGuid().ToString("N");Countdown=definition.countdownSeconds;}
 public void Tick(float dt,Vector3 from,Vector3 to){
  if(float.IsNaN(dt)||float.IsInfinity(dt)||dt<0)throw new ArgumentOutOfRangeException(nameof(dt));
  if(State==TrialState.Countdown){Countdown=Mathf.Max(0,Countdown-dt);if(Countdown<=0)State=TrialState.Running;return;}
  if(State!=TrialState.Running)return;Elapsed+=dt;
  if(!Finite(from)||!Finite(to)){State=TrialState.Abandoned;return;}
  // A teleport never counts as driving through a gate. Recovery uses its own policy.
  if((to-from).sqrMagnitude>60*60)return;
  var gate=Definition.checkpoints[NextGate];Vector3 normal=gate.Forward,right=new Vector3(normal.z,0,-normal.x);float a=Vector3.Dot(from-gate.Position,normal),b=Vector3.Dot(to-gate.Position,normal);
  if(a>0||b<=0||b-a<.00001f)return;Vector3 crossing=Vector3.Lerp(from,to,-a/(b-a));
  if(Mathf.Abs(Vector3.Dot(crossing-gate.Position,right))>gate.halfWidth||Mathf.Abs(crossing.y-gate.y)>3)return;
  NextGate++;if(NextGate==Definition.checkpoints.Length)State=TrialState.Finished;
 }
 public TrialGate Recover(){if(State!=TrialState.Running)throw new InvalidOperationException("Recovery requires running trial");Penalty+=Definition.recoveryPenaltySeconds;Elapsed+=Definition.recoveryPenaltySeconds;return Definition.checkpoints[Mathf.Max(0,NextGate-1)];}
 public void Abandon(){if(State==TrialState.Countdown||State==TrialState.Running)State=TrialState.Abandoned;}
 static bool Finite(Vector3 p)=>!float.IsNaN(p.x)&&!float.IsNaN(p.y)&&!float.IsNaN(p.z)&&!float.IsInfinity(p.x)&&!float.IsInfinity(p.y)&&!float.IsInfinity(p.z);
}
}
