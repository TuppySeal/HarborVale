using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
public sealed class TrafficWorld:MonoBehaviour {
 public static readonly List<RoadPilot> Agents=new List<RoadPilot>();public static FinchVehicle Player;public int Spawned=>Agents.Count(a=>a!=null&&!a.IsRival);public bool RaceReserved;public static bool DisabledForTests;RivalsData data;float timer;int serial;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Clear(){Agents.Clear();Player=null;DisabledForTests=false;}
 public static void Hook(){if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_CityMap")new GameObject("Traffic | connected right-hand road loops").AddComponent<TrafficWorld>();}
 IEnumerator Start(){while(FindAnyObjectByType<DocksideTrialDirector>()?.vehicle==null)yield return null;Player=FindAnyObjectByType<DocksideTrialDirector>().vehicle;data=RivalsData.Load();yield return new WaitForSeconds(1);}
 public static bool CanReposition(Vector3 point){if(Player==null||Vector3.Distance(point,Player.transform.position)<160)return false;var camera=Camera.main;if(camera==null)return true;var p=camera.WorldToViewportPoint(point+Vector3.up);return p.z<0||p.x<-.1f||p.x>1.1f||p.y<-.1f||p.y>1.1f;}
 void Update(){if(data==null||Player==null||DisabledForTests)return;timer-=Time.deltaTime;if(timer>0)return;timer=1;Agents.RemoveAll(a=>a==null);foreach(var a in Agents.ToArray())if(!a.IsRival&&Vector3.Distance(a.transform.position,Player.transform.position)>700&&CanReposition(a.transform.position)){Agents.Remove(a);Destroy(a.gameObject);}if(!RaceReserved&&Spawned<data.trafficCount)SpawnTraffic();}
 void SpawnTraffic(){var choices=new List<(TrafficRoute,int)>();foreach(var r in data.routes)for(int i=0;i<r.points.Length;i+=12){float distance=Vector3.Distance(r.points[i].Position,Player.transform.position);if(distance>110&&distance<350&&Agents.All(a=>Vector3.Distance(a.transform.position,r.points[i].Position)>35))choices.Add((r,i));}if(choices.Count==0)return;var pick=choices[(serial++*137)%choices.Count];var car=CreateCar("Traffic car "+serial,new WorkshopBuild(),serial%5,serial%4==0?"car_finch":VehicleLineup.All[(serial-1)%3].id);var driver=car.gameObject.AddComponent<RoadPilot>();driver.Car=car;driver.Path=pick.Item1.points.Select(p=>p.Position).ToArray();driver.Index=pick.Item2;driver.CruiseKph=30+serial%4*4;driver.Place(pick.Item2);Agents.Add(driver);}
 public static FinchVehicle CreateCar(string name,WorkshopBuild build,int paint,string model="car_finch"){var g=Instantiate(Resources.Load<GameObject>("RivalsFinch"));g.name=name;var car=g.AddComponent<FinchVehicle>();car.Initialize();car.externalInput=true;car.playerInput=false;car.commandedHandbrake=false;car.Body.interpolation=RigidbodyInterpolation.Interpolate;VehicleBody.Apply(car,model);var catalog=WorkshopCatalog.ForBuild(model,build);car.EngineModelId=string.IsNullOrEmpty(build.engineModelId)?model:build.engineModelId;car.InstallPowertrain(catalog.Resolve(build));ChassisTuning.ApplyVehicle(catalog,build,car);FitmentTuning.ApplyVehicle(catalog,build,car);Paint(car,paint);g.AddComponent<RoadEngineAudio>().Car=car;return car;}
 public static void Paint(FinchVehicle car,int paint){var colors=new[]{new Color(.7f,.24f,.13f),new Color(.23f,.48f,.28f),new Color(.48f,.25f,.63f),new Color(.78f,.73f,.54f),new Color(.75f,.76f,.78f)};var block=new MaterialPropertyBlock();foreach(var r in car.GetComponentsInChildren<Renderer>()){if(r.name.Contains("Body shell")||r.name.Contains("Roof")||r.name.Contains("Pillar")||r.name.Contains("hood")){r.GetPropertyBlock(block);block.SetColor("_Color",paint<0?(VehicleLineup.IsNew(car.PowertrainCarId)?new Color(VehicleLineup.Find(car.PowertrainCarId).color[0],VehicleLineup.Find(car.PowertrainCarId).color[1],VehicleLineup.Find(car.PowertrainCarId).color[2]):new Color(.3f,.49f,.58f)):colors[paint%colors.Length]);r.SetPropertyBlock(block);}}}
 public void ReserveCircuit(Vector3[] course){RaceReserved=true;foreach(var p in Agents.ToArray())if(p!=null&&!p.IsRival&&course.Any(t=>Vector3.Distance(t,p.transform.position)<28)){Agents.Remove(p);Destroy(p.gameObject);}}
 public void ClearGrid(Vector3 start){foreach(var a in Agents.ToArray())if(a!=null&&!a.IsRival&&Vector3.Distance(a.transform.position,start)<70){Agents.Remove(a);Destroy(a.gameObject);}}
 void OnDestroy(){Agents.RemoveAll(a=>a==null||a.gameObject.scene==gameObject.scene);Player=null;}
}
}
