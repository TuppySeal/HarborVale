using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing {
public sealed class PoliceContact : MonoBehaviour {
 public string Id;
 public int Phase;
 public FinchVehicle Car;
 public PolicePatrol Owner;
 public Renderer Light;
 public bool RadarOn(float time)=>Mathf.Repeat(time+Phase,39)<10;
 public void ShowLights(bool active){
  if(Light==null)return;
  var block=new MaterialPropertyBlock();
  Light.GetPropertyBlock(block);
  block.SetColor("_Color",active?(Mathf.Repeat(Time.time*4,1)<.5f?Color.red:Color.blue):new Color(.12f,.22f,.4f));
  Light.SetPropertyBlock(block);
 }
 void FixedUpdate(){
  if(!Owner.IsPursuing(this)||Owner.Player==null)return;
  var highway=GetComponent<HighwayDriver>();
  var road=GetComponent<RoadPilot>();
  if(highway!=null){
   highway.CruiseKph=175;
   if(Vector3.Distance(transform.position,Owner.Player.transform.position)>34){highway.enabled=true;return;}
   highway.enabled=false;
  }
  if(road!=null)road.enabled=false;
  var target=Owner.Player;
  var direction=target.transform.position+target.Body.linearVelocity*.18f-transform.position;
  var local=transform.InverseTransformDirection(direction);
  Car.commandedSteer=Mathf.Clamp(Mathf.Atan2(local.x,local.z)*2,-1,1);
  float desired=Mathf.Clamp(target.SpeedKph+24,24,145);
  if(local.z<0)desired=18;
  Car.commandedServiceBrake=Mathf.Clamp01((Car.SpeedKph-desired)/12);
  Car.commandedPedal=Mathf.Clamp01((desired-Car.SpeedKph)/13);
  Car.commandedHandbrake=false;
 }
 public void Resume(){
  var highway=GetComponent<HighwayDriver>();
  if(highway!=null){highway.CruiseKph=106;highway.enabled=true;return;}
  var road=GetComponent<RoadPilot>();
  if(road!=null)road.enabled=true;
 }
 void OnCollisionEnter(Collision collision){Owner.Contact(this,collision);}
}
}
