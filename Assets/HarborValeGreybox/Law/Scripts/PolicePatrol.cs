using System;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
public sealed class PolicePatrol : MonoBehaviour {
 public static PolicePatrol Instance {get;private set;}
 public bool Wanted=>evidence!=null;
 public bool RadarAlert{get;private set;}
 readonly List<PoliceContact> officers=new List<PoliceContact>();
 DocksideTrialDirector trial;
 SpeedFineEvidence evidence;
 PoliceContact pursuing;
 string caseId,message;
 float observedAt,contactAt=-100,escapedFor,queryAt;
 float limit;
 bool highway;
 readonly RaycastHit[] sight=new RaycastHit[24];
 public static void Hook(){
  if(HighwayTrafficWorld.Instance!=null&&Instance==null)
   Instance=new GameObject("Harbor Vale patrols").AddComponent<PolicePatrol>();
 }
 void OnDestroy(){if(Instance==this)Instance=null;}
 void Start(){
  if(TrafficWorld.DisabledForTests)return;
  trial=FindAnyObjectByType<DocksideTrialDirector>();
  var world=HighwayTrafficWorld.Instance;
  Add(world.Spawn("Highway patrol 01",new WorkshopBuild(),"car_bastion",false,680,1,false,true,106).Car,"officer_highway_01",4);
  Add(world.Spawn("Highway patrol 02",new WorkshopBuild(),"car_bastion",true,1500,1,false,true,103).Car,"officer_highway_02",28);
  var routes=RivalsData.Load().routes;
  for(int i=0;i<2;i++){
   var car=TrafficWorld.CreateCar("City patrol "+(i+1),new WorkshopBuild(),4,"car_finch");
   var pilot=car.gameObject.AddComponent<RoadPilot>();
   pilot.Car=car;pilot.CruiseKph=45;
   var points=routes[(i*17+9)%routes.Length].points;
   pilot.Path=Array.ConvertAll(points,p=>p.Position);
   Add(car,"officer_city_0"+(i+1),i*17+12);
   pilot.Place(points.Length/3);
   TrafficWorld.Agents.Add(pilot);
  }
 }
 void Add(FinchVehicle car,string id,int phase){
  var officer=car.gameObject.AddComponent<PoliceContact>();
  officer.Id=id;officer.Phase=phase;officer.Owner=this;officer.Car=car;officers.Add(officer);
  var roof=GameObject.CreatePrimitive(PrimitiveType.Cube);
  roof.name="Patrol light bar";roof.transform.SetParent(car.transform,false);
  roof.transform.localPosition=new Vector3(0,1.49f,-.15f);
  roof.transform.localScale=new Vector3(.88f,.12f,.24f);
  Destroy(roof.GetComponent<Collider>());
  officer.Light=roof.GetComponent<Renderer>();
 }
 static void CheckPath(RoutePoint[] points,bool closed,Vector3 position,float width,float posted,bool expressway,ref float best,ref float limit,ref bool highway){
  int count=closed?points.Length:points.Length-1;
  for(int i=0;i<count;i++){
   var a=points[i].Position;var edge=points[(i+1)%points.Length].Position-a;
   float fraction=Mathf.Clamp01(Vector3.Dot(position-a,edge)/Mathf.Max(.01f,edge.sqrMagnitude));
   var delta=position-(a+edge*fraction);
   if(Mathf.Abs(delta.y)>3.5f||delta.x*delta.x+delta.z*delta.z>width*width||delta.sqrMagnitude>=best)continue;
   best=delta.sqrMagnitude;limit=posted;highway=expressway;
  }
 }
 public static float PostedLimit(Vector3 p,out bool highway){
  var n=HighwayNetwork.Load();float best=float.PositiveInfinity,posted=0;highway=false;
  CheckPath(n.center,true,p,n.roadWidth/2,n.rules.highwayLimitMph,true,ref best,ref posted,ref highway);
  foreach(var r in n.ramps)CheckPath(r.points,false,p,r.width/2,r.speedLimitMph,false,ref best,ref posted,ref highway);
  foreach(var r in n.cityRoads)CheckPath(r.points,false,p,r.width/2,r.speedLimitMph,false,ref best,ref posted,ref highway);
  return posted;
 }
 bool CanSee(PoliceContact officer,FinchVehicle player,float range){
  var origin=officer.transform.position+Vector3.up;
  var delta=player.transform.position+Vector3.up-origin;
  if(delta.sqrMagnitude>range*range||Vector3.Dot(delta.normalized,officer.transform.forward)<.15f)return false;
  int count=Physics.RaycastNonAlloc(origin,delta.normalized,sight,delta.magnitude,~0,QueryTriggerInteraction.Ignore);
  if(count==sight.Length)return false;
  for(int i=0;i<count;i++)
   if(!sight[i].collider.transform.IsChildOf(officer.transform)&&!sight[i].collider.transform.IsChildOf(player.transform))return false;
  return true;
 }
 void Update(){
  if(trial==null||trial.vehicle==null||trial.Store==null)return;
  var player=trial.vehicle;
  float now=Time.time;
  if(now>=queryAt){
   queryAt=now+.25f;
   limit=PostedLimit(player.transform.position,out highway);
   RadarAlert=false;
   var rules=HighwayNetwork.Load().rules;
   string carId=trial.Store.Profile.fleet.selectedId;
   foreach(var officer in officers){
    if(officer==null||!officer.RadarOn(now))continue;
    if(trial.Store.HasRadarDetector(carId)&&CanSee(officer,player,rules.detectorRangeM))RadarAlert=true;
    float tolerance=highway?rules.highwayToleranceMph:rules.cityToleranceMph;
    if(limit<=0||player.SpeedKph<=DrivingUnits.FromMph(limit+tolerance)+.0001f||!CanSee(officer,player,rules.radarRangeM))continue;
    if(Wanted&&(pursuing!=officer||player.SpeedKph<=evidence.measuredKph))continue;
    if(!Wanted){caseId=Guid.NewGuid().ToString("N");pursuing=officer;contactAt=-100;escapedFor=0;}
    evidence=new SpeedFineEvidence{officerId=officer.Id,capturedCarId=carId,roadClass=highway?"highway":"city",postedMph=limit,measuredKph=player.SpeedKph};
    observedAt=now;
   }
  }
  foreach(var officer in officers)if(officer!=null)officer.ShowLights(officer==pursuing);
  if(!Wanted)return;if(pursuing==null){evidence=null;return;}
  var distance=Vector3.Distance(player.transform.position,pursuing.transform.position);
  escapedFor=distance>HighwayNetwork.Load().rules.escapeDistanceM?escapedFor+Time.deltaTime:0;
  if(escapedFor>=HighwayNetwork.Load().rules.escapeSeconds){EndPursuit("Patrol lost sight of you.");return;}
  if(now-contactAt<12&&distance<28&&player.SpeedKph<5&&now-observedAt>.5f){
   try{
    if(CircuitRaceDirector.Instance?.BeforeLeave()==false||RivalRaceDirector.Instance?.BeforeLeave()==false)return;
    if(trial.Active)trial.Abandon();
    trial.Store.IssueSpeedFine(caseId,evidence);
    EndPursuit("Speeding citation recorded. Outstanding fines "+trial.Store.OutstandingFines+" cr.");
   }catch(Exception ex){message=ex.Message;}
  }
 }
 void EndPursuit(string status){pursuing.Resume();pursuing=null;evidence=null;message=status;contactAt=-100;}
 internal bool IsPursuing(PoliceContact officer)=>pursuing==officer;
 internal FinchVehicle Player=>trial?.vehicle;
 internal void Contact(PoliceContact officer,Collision collision){
  if(!IsPursuing(officer)||Player==null||collision.relativeVelocity.magnitude<1.5f)return;
  if(collision.collider.GetComponentInParent<FinchVehicle>()==Player)contactAt=Time.time;
 }
 void OnGUI(){
  var car=Player;
  if(trial?.Store==null||car==null||FindAnyObjectByType<ExpandedWorkshop>()?.IsOpen==true||DrivingSettingsView.Instance?.IsOpen==true)return;
  if(trial.ExternalModalOpen&&!Wanted)return; if(Vector3.Distance(car.transform.position,HighwayNetwork.Load().ramps[0].points[0].Position)<110)GUI.Box(new Rect(16,200,402,30),"North Gate ramp ahead · Stop and press H for the Tempest race");
  var area=new Rect(Screen.width-310,100,294,130);
  GUILayout.BeginArea(area,GUI.skin.box);
  if(limit>0)GUILayout.Label("Posted limit "+DrivingUnits.Speed(DrivingUnits.FromMph(limit)));
  if(Wanted)GUILayout.Label("POLICE PURSUIT · "+DrivingUnits.Speed(evidence.measuredKph)+" recorded");
  if(RadarAlert)GUILayout.Label("ACTIVE RADAR NEARBY");
  if(!string.IsNullOrEmpty(message))GUILayout.Label(message);
  if(car.SpeedKph<3&&!Wanted&&!trial.Active){
   string id=trial.Store.Profile.fleet.selectedId;
   int debt=trial.Store.OutstandingFines;
   if(debt>0&&GUILayout.Button("Pay fines · "+debt+" cr"))Transact(()=>trial.Store.PaySpeedFines(Guid.NewGuid().ToString("N")));
   if(debt==0&&id!=FleetLedger.Loaner&&!trial.Store.HasRadarDetector(id)&&GUILayout.Button("Radar detector · 1,200 cr"))Transact(()=>trial.Store.BuyRadarDetector(Guid.NewGuid().ToString("N"),id,1200));
  }
  GUILayout.EndArea();
 }
 void Transact(Action action){try{action();message="Saved to your driving account.";}catch(Exception ex){message=ex.Message;}}
}
}
