using System;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing {
public sealed class VehicleBody:MonoBehaviour {
 string model;GameObject visible;Transform[] original;FinchVehicle car;
 public static void Apply(FinchVehicle target,string id){var component=target.GetComponent<VehicleBody>();if(component==null)component=target.gameObject.AddComponent<VehicleBody>();component.Switch(target,id);}
 void Switch(FinchVehicle target,string id){if(model==id)return;car=target;if(original==null){string[] names={"Front left wheel pivot","Front right wheel pivot","Rear left wheel pivot","Rear right wheel pivot"};var renderers=car.GetComponentsInChildren<MeshFilter>();var roots=new System.Collections.Generic.HashSet<Transform>();foreach(var f in renderers){if(f.name.Contains("Exhaust flame")||f.name.Contains("Chassis"))continue;var t=f.transform;while(t.parent!=null&&t.parent!=car.transform)t=t.parent;if(t.parent==car.transform&&!t.name.Contains("audio")&&!t.name.Contains("Exhaust")&&!t.name.Contains("Aero"))roots.Add(t);}original=new Transform[roots.Count];roots.CopyTo(original);}
  if(visible!=null){visible.SetActive(false);if(Application.isPlaying)Destroy(visible);else DestroyImmediate(visible);}foreach(var t in original)if(t!=null)t.gameObject.SetActive(id=="car_finch");Transform binding=car.transform;
  if(VehicleLineup.IsNew(id)){visible=Instantiate(Resources.Load<GameObject>(id),car.transform,false);visible.name=VehicleLineup.Find(id).name+" | active visual";binding=visible.transform;}car.PowertrainCarId=id;var s=VehicleLineup.IsNew(id)?VehicleLineup.Find(id):null;car.ConfigureGeometry(binding,s==null?2.45f:s.wheelBase,s==null?1.554f:s.track,s==null?.295f:s.wheelRadius,s==null?new Vector3(1.58f,1.18f,3.90f):new Vector3(s.width-.12f,s.height-.28f,s.length-.12f),s==null?new Vector3(0,.84f,0):new Vector3(0,s.height*.5f+.08f,0));car.lowSpeedSteer=s==null?32:s.steer;car.highSpeedSteer=7.5f;car.Body.centerOfMass=s==null?new Vector3(0,.43f,.08f):new Vector3(0,s.height*.28f,s.stockDrive=="rwd"?-.04f:.04f);model=id;
 }
}
}
