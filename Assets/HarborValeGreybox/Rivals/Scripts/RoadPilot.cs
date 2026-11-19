using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing {
// Uses the same steering, powertrain, suspension and tires as the player. No velocity assignment/rubber banding.
public sealed class RoadPilot:MonoBehaviour {
 static int nextPriority;public int Priority{get;private set;}void Awake(){Priority=++nextPriority;}public FinchVehicle Car;public Vector3[] Path;public int Index;public float CruiseKph=36;public bool Held;public bool IsRival;public float Lane=1.75f;public int Recoveries{get;private set;}public float DistanceMoved{get;private set;}Vector3 previous;float stuck;readonly RaycastHit[] hits=new RaycastHit[20];
 public Vector3 Point(int i){i=(i%Path.Length+Path.Length)%Path.Length;var tangent=(Path[(i+1)%Path.Length]-Path[(i+Path.Length-1)%Path.Length]);tangent.y=0;tangent.Normalize();return Path[i]+new Vector3(tangent.z,0,-tangent.x)*Lane;}
 public void Place(int i){Index=i%Path.Length;Car.ResetAt(Point(Index)+Vector3.up*.35f,Quaternion.LookRotation(Point(Index+2)-Point(Index)));previous=Car.transform.position;stuck=0;}
 public void RecoverAt(TrialGate gate){float best=float.MaxValue;for(int i=0;i<Path.Length;i++){float d=(Path[i]-gate.Position).sqrMagnitude;if(d<best){best=d;Index=i;}}Recoveries++;Car.ResetAt(gate.Position-gate.Forward*5+Vector3.Cross(Vector3.up,gate.Forward)*Lane+Vector3.up*.35f,Quaternion.LookRotation(gate.Forward));previous=Car.transform.position;stuck=0;}
 void Start(){previous=transform.position;}
 void FixedUpdate(){Drive(Time.fixedDeltaTime);}
 public void Drive(float dt){if(Car==null||Path==null||Path.Length<4)return;Car.externalInput=true;Car.commandedHandbrake=false;Car.commandedServiceBrake=Held?1:0;Car.commandedPedal=0;if(Held)return;var pos=Car.transform.position;DistanceMoved+=Vector3.Distance(pos,previous);previous=pos;
  for(int tries=0;tries<20;tries++){Vector3 next=Point(Index+1)-pos;next.y=0;if(next.magnitude<4||Vector3.Dot(Point(Index)-pos,Point(Index+1)-Point(Index))<0)Index=(Index+1)%Path.Length;else break;}
  float look=Mathf.Clamp(4+Car.SpeedKph*.14f,4,14),distance=0;int goal=Index;for(int n=0;n<30&&distance<look;n++){distance+=Vector3.Distance(Point(goal),Point(goal+1));goal=(goal+1)%Path.Length;}
  Vector3 local=Car.transform.InverseTransformPoint(Point(goal));float angle=Mathf.Atan2(local.x,local.z);float steeringAngle=Mathf.Atan2(2*2.45f*Mathf.Sin(angle),Mathf.Max(3,local.magnitude))*Mathf.Rad2Deg;Car.commandedSteer=Mathf.Clamp(steeringAngle/Mathf.Lerp(Car.lowSpeedSteer,Car.highSpeedSteer,Mathf.Clamp01(Car.SpeedKph/130)), -1,1);
  int ahead=goal;float bend=0;for(int n=0;n<16;n++){bend=Mathf.Max(bend,Vector3.Angle(Point(ahead+1)-Point(ahead),Point(ahead+3)-Point(ahead+1)));ahead=(ahead+2)%Path.Length;}
  float target=Mathf.Min(CruiseKph,Mathf.Lerp(CruiseKph,14,Mathf.Clamp01(bend/55)));if(local.z<0)target=10;Vector3 origin=pos+Car.transform.forward*2.2f+Vector3.up*.65f;float safe=4+Car.SpeedKph*.45f;int count=Physics.SphereCastNonAlloc(origin,.65f,Car.transform.forward,hits,safe,~((1<<28)|(1<<29)|(1<<31)),QueryTriggerInteraction.Ignore);
  for(int i=0;i<count;i++){var hit=hits[i];if(hit.collider.transform.IsChildOf(Car.transform)||hit.normal.y>.7f)continue;target=Mathf.Min(target,Mathf.Max(0,(hit.distance-3)*2));}
  // Cars approaching crossing paths yield to the closer car; deterministic tie-break prevents mutual priority.
  foreach(var other in TrafficWorld.Agents){if(other==null||other==this||other.Car==null)continue;var relative=other.transform.position-pos;if(relative.sqrMagnitude>18*18||Vector3.Dot(Car.transform.forward,relative)<0)continue;if(Mathf.Abs(Vector3.Dot(Car.transform.forward,other.transform.forward))>.7f)continue;Vector3 mine=pos+Car.transform.forward*8,theirs=other.transform.position+other.transform.forward*8;if(Vector3.Distance(mine,theirs)<10&&(other.Car.SpeedKph>2||other.Priority<Priority))target=0;}
  if(Car.SpeedKph>target+2){Car.commandedServiceBrake=Mathf.Clamp01((Car.SpeedKph-target)/12);Car.commandedPedal=0;}else{Car.commandedServiceBrake=0;Car.commandedPedal=Mathf.Clamp((target-Car.SpeedKph)/14,.08f,IsRival?.95f:.65f);}
  if(Car.SpeedKph<2&&target>8)stuck+=dt;else stuck=0;if(stuck>12&&!IsRival&&TrafficWorld.CanReposition(Car.transform.position)){Recoveries++;Place(Index);}
 }
}
}
