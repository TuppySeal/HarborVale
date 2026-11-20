using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
public sealed class HighwayTrafficWorld : MonoBehaviour {
 public static HighwayTrafficWorld Instance {get;private set;}
 public Vector3[] ForwardPath {get;private set;}
 public Vector3[] ReversePath {get;private set;}
 readonly List<HighwayDriver> traffic=new List<HighwayDriver>();
 float timer;
 int serial;
 public static void Hook(){
  if(HighwayRuntime.Instance==null||Instance!=null)return;
  Instance=new GameObject("Expressway traffic").AddComponent<HighwayTrafficWorld>();
  var network=HighwayNetwork.Load();
  Instance.ForwardPath=network.Carriageway(false);
  Instance.ReversePath=network.Carriageway(true);
 }
 void OnDestroy(){if(Instance==this)Instance=null;}
 public HighwayDriver Spawn(string label,WorkshopBuild build,string model,bool reverse,int index,int lane,bool racer=false,bool police=false,float speed=0){
  var car=TrafficWorld.CreateCar(label,build,serial%5,model);
  var driver=car.gameObject.AddComponent<HighwayDriver>();
  driver.Car=car;
  driver.Path=reverse?ReversePath:ForwardPath;
  driver.IsRacer=racer;
  driver.IsPolice=police;
  driver.CruiseKph=speed;
  if(!racer&&!police){
   driver.Persona=car.gameObject.AddComponent<TrafficPersona>();
   driver.Persona.Configure(++serial);
   lane=serial%4==0?lane:driver.Persona.PreferredLane;
   traffic.Add(driver);
  }
  driver.SetProxy(car.gameObject.AddComponent<RoadPilot>());
  driver.Place(index,lane);
  car.gameObject.AddComponent<TrafficIndicators>();
  return driver;
 }
 void Update(){
  timer-=Time.deltaTime;
  var player=TrafficWorld.Player;
  if(timer>0||player==null||TrafficWorld.DisabledForTests)return;
  timer=.35f;
  var playerCenter=player.transform.position;var racer=RivalRaceDirector.Instance?.Opponent?.GetComponent<HighwayDriver>();var rivalCenter=racer!=null?racer.transform.position:playerCenter;var center=serial%3==0?rivalCenter:playerCenter;
  for(int i=traffic.Count-1;i>=0;i--){
   var driver=traffic[i];
   if(driver==null){traffic.RemoveAt(i);continue;}
   var position=driver.transform.position;
   if((Mathf.Min((position-playerCenter).sqrMagnitude,(position-rivalCenter).sqrMagnitude)>950*950||driver.NeedsAssistedReset)&&TrafficWorld.CanReposition(position)){
    traffic.RemoveAt(i);Destroy(driver.gameObject);
   }
  }
  if(traffic.Count>=HighwayNetwork.Load().rules.highwayTrafficCount)return;
  bool reverse=serial%3==1;
  var path=reverse?ReversePath:ForwardPath;
  int lane=serial%3;
  for(int n=0;n<path.Length;n+=9){
   int i=(n+serial*157)%path.Length;
   var p=path[i];
   float distance=(p-center).sqrMagnitude;
   if(distance<170*170||distance>810*810||!TrafficWorld.CanReposition(p))continue;
   bool occupied=false;
   foreach(var driver in HighwayDriver.Active)
    if(driver!=null&&(driver.transform.position-p).sqrMagnitude<36*36){occupied=true;break;}
   if(occupied)continue;
   Spawn("Highway traffic "+(serial+1),new WorkshopBuild(),serial%4==0?"car_finch":VehicleLineup.All[serial%3].id,reverse,i,lane);
   return;
  }
  serial++;
 }
}
}
