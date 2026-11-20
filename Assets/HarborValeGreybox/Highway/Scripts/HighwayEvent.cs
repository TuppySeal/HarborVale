using System.Collections.Generic;
using UnityEngine;
namespace HarborValeGreybox.Racing {
public static class HighwayEvent {
 public const string Id="evt_highway_01";
 public const int StartIndex=505;
 public static TrialDefinition Build(){
  var path=HighwayTrafficWorld.Instance.ForwardPath;
  var gates=new List<TrialGate>();
  var route=new RoutePoint[path.Length+1];
  float length=0;
  for(int i=0;i<=path.Length;i++){
   int index=(StartIndex+i)%path.Length;
   var p=path[index];
   route[i]=new RoutePoint{x=p.x,y=p.y,z=p.z};
   if(i>0)length+=Vector3.Distance(path[(index+path.Length-1)%path.Length],p);
   if(i%32==0||i==path.Length){
    var forward=path[(index+1)%path.Length]-path[(index+path.Length-1)%path.Length];
    gates.Add(new TrialGate{x=p.x,y=p.y,z=p.z,forwardX=forward.x,forwardZ=forward.z,halfWidth=5.4f});
   }
  }
  return new TrialDefinition{id=Id,title="Expressway Showdown",lengthM=length,route=route,checkpoints=gates.ToArray(),
   countdownSeconds=3,recoveryPenaltySeconds=6,goldSeconds=210,silverSeconds=300,bronzeSeconds=420,creditBps=new int[4],repBps=new int[4]};
 }
 public static int FindIndex(Vector3 point){
  var path=HighwayTrafficWorld.Instance.ForwardPath;
  int index=0;float best=float.PositiveInfinity;
  for(int i=0;i<path.Length;i++){
   float distance=(point-path[i]).sqrMagnitude;
   if(distance<best){index=i;best=distance;}
  }
  return index;
 }
}
}
