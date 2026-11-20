using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing.Editor {
public static class HighwayGeometryVerification {
 static List<string> lines=new List<string>();static int failures;
 static void Check(bool ok,string s){lines.Add((ok?"PASS ":"FAIL ")+s);if(!ok)failures++;}
 public static void Fresh(){HighwayAssets.Build();Run();}public static void JoinsOnly(){HighwayAssets.Build();joinsOnly=true;Run();}static bool joinsOnly; public static void Run(){Directory.CreateDirectory("Verification/Highway");lines.Clear();failures=0;EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");foreach(var s in UnityEngine.Object.FindObjectsByType<StarterCarSpecification>())UnityEngine.Object.DestroyImmediate(s.gameObject);var root=(GameObject)PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("HarborExpressway"));Physics.SyncTransforms();var n=HighwayNetwork.Load();try {
 foreach(float lane in new[]{-10.2f,-6.6f,-3f,3f,6.6f,10.2f})Scan("Mainline lane "+lane,n.center.Select((p,i)=>HighwayNetwork.Offset(n.center,i,lane,true)).ToArray(),true);
 foreach(var ramp in n.ramps){foreach(float lane in ramp.lanes==2?new[]{-1.8f,1.8f}:new[]{0f})Scan(ramp.id+" lane "+lane,ramp.points.Select((p,i)=>HighwayNetwork.Offset(ramp.points,i,lane,false)).ToArray(),false);}
 Render(new Vector3(2200,2400,-2100),new Vector3(0,55,300),"Aerial");Render(new Vector3(230,240,1470),new Vector3(140,100,1450),"NorthGateway");
 if(failures==0&&!joinsOnly){foreach(bool reverse in new[]{false,true})foreach(float lane in new[]{-3.6f,0f,3.6f})Drive("Mainline "+reverse+" lane "+lane,n.Carriageway(reverse),lane,110,33000,true);
 foreach(var ramp in n.ramps){var p=ramp.points.Select(t=>t.Position).ToArray();Drive(ramp.id,p,ramp.lanes==2?1.8f:0,35,18000,false);}Junctions(n);}if(joinsOnly)Junctions(n);
 }finally{UnityEngine.Object.DestroyImmediate(root);File.WriteAllLines("Verification/Highway/geometry-physics.txt",lines);}if(failures>0)throw new Exception("Highway verification failures="+failures);}
 static void Scan(string label,Vector3[] points,bool closed){int gaps=0,blocked=0;string example="";for(int i=0;i<points.Length;i++){var p=points[i];var ground=Physics.RaycastAll(p+Vector3.up*.5f,Vector3.down,1,~0,QueryTriggerInteraction.Ignore);if(!ground.Any(h=>Mathf.Abs(h.point.y-p.y)<.12f)){gaps++;if(example=="")example=" gap="+i+" "+p;}
 var overlaps=Physics.OverlapBox(p+Vector3.up*.9f,new Vector3(.76f,.55f,1.3f),Quaternion.LookRotation(points[(i+1)%points.Length]-points[i==points.Length-1&&!closed?i-1:i]),~0,QueryTriggerInteraction.Ignore);if(overlaps.Length>0){blocked++;if(example=="")example=" blocker="+i+" "+p+" "+overlaps[0].name;}}
 Check(gaps==0&&blocked==0,label+" samples="+points.Length+" surface gaps="+gaps+" body obstructions="+blocked+example);}
 static void Drive(string label,Vector3[] path,float lane,float speed,int ticks,bool closed){var mode=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;var car=TrafficWorld.CreateCar("Highway physical probe",new WorkshopBuild(),0);car.enabled=false;var pilot=car.gameObject.AddComponent<RoadPilot>();pilot.enabled=false;pilot.Car=car;pilot.Path=path;pilot.Lane=lane;pilot.IsRival=true;pilot.CruiseKph=speed;pilot.Place(2);float peak=0,minUp=1;int air=0;bool finish=false,halfway=false;try{for(int i=0;i<ticks;i++){pilot.Drive(.02f);car.Tick(.02f);Physics.Simulate(.02f);peak=Mathf.Max(peak,car.SpeedKph);minUp=Mathf.Min(minUp,Vector3.Dot(car.transform.up,Vector3.up));if(i>100&&car.GroundedWheels==0)air++;if(pilot.Index>path.Length/2)halfway=true;if((closed&&halfway&&pilot.Index<2)||(!closed&&pilot.Index>=path.Length-5)){finish=true;break;}if(car.transform.position.y<path[Mathf.Clamp(pilot.Index,0,path.Length-1)].y-4)break;}
 if(!finish){Render(car.transform.position+new Vector3(0,22,-8),car.transform.position,label.Replace(" ","_")+"_Failure");lines.Add("DEBUG pedal="+car.commandedPedal+" brake="+car.commandedServiceBrake+" steer="+car.commandedSteer+" nearby="+string.Join(",",Physics.OverlapSphere(car.transform.position,5).Where(c=>!c.transform.IsChildOf(car.transform)).Select(c=>c.name)));}Check(finish&&minUp>.65f&&pilot.Recoveries==0,label+" physical drive finish="+finish+" distance="+pilot.DistanceMoved.ToString("F1")+" peakKph="+peak.ToString("F1")+" upright="+minUp.ToString("F2")+" airborneTicks="+air+" index="+pilot.Index+" position="+car.transform.position);
 }finally{UnityEngine.Object.DestroyImmediate(car.gameObject);Physics.simulationMode=mode;}File.WriteAllLines("Verification/Highway/geometry-physics.txt",lines);}
 static Vector3[] Ramp(HighwayNetwork n,int index,float lane=0,bool reverse=false){var r=n.ramps[index];var p=r.points.Select((v,i)=>HighwayNetwork.Offset(r.points,i,lane,false)).ToArray();if(reverse)Array.Reverse(p);return p;}
 static Vector3[] MainJoin(HighwayNetwork n,Vector3 endpoint,bool reverse,bool before){var p=n.Carriageway(reverse);for(int i=0;i<p.Length;i++){int ix=reverse?n.center.Length-1-i:i;p[i]+=HighwayNetwork.Right(n.center,ix,true)*(reverse?-3.6f:3.6f);}int nearest=Enumerable.Range(0,p.Length).OrderBy(i=>(p[i]-endpoint).sqrMagnitude).First();return Enumerable.Range(before?-30:1,30).Select(i=>p[(nearest+i+p.Length)%p.Length]).ToArray();}
 static Vector3[] Chain(params Vector3[][] pieces){var list=new List<Vector3>();foreach(var p in pieces)foreach(var v in p)if(list.Count==0||Vector3.Distance(list[list.Count-1],v)>.3f)list.Add(v);return list.ToArray();}
 static void Junctions(HighwayNetwork n){var east=Ramp(n,1);var west=Ramp(n,4);var exitE=Ramp(n,2);var exitW=Ramp(n,5);
 Drive("City to eastbound merge",Chain(Ramp(n,0,1.8f),east,MainJoin(n,east.Last(),false,false)),0,35,24000,false);
 Drive("Eastbound exit to city",Chain(MainJoin(n,exitE[0],false,true),exitE,Ramp(n,0,-1.8f,true)),0,35,24000,false);
 Drive("City flyover to westbound merge",Chain(Ramp(n,0,1.8f),Ramp(n,3,1.8f),west,MainJoin(n,west.Last(),true,false)),0,35,48000,false);
 Drive("Westbound exit via flyover to city",Chain(MainJoin(n,exitW[0],true,true),exitW,Ramp(n,3,-1.8f,true),Ramp(n,0,-1.8f,true)),0,35,48000,false);
 }
 static void Render(Vector3 from,Vector3 to,string name){var g=new GameObject("Highway verification camera");var c=g.AddComponent<Camera>();g.transform.position=from;g.transform.LookAt(to);c.farClipPlane=10000;c.nearClipPlane=.3f;var rt=new RenderTexture(1600,1000,24);c.targetTexture=rt;c.Render();RenderTexture.active=rt;var t=new Texture2D(1600,1000,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1600,1000),0,0);t.Apply();File.WriteAllBytes("Verification/Highway/"+name+".png",t.EncodeToPNG());RenderTexture.active=null;c.targetTexture=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(t);UnityEngine.Object.DestroyImmediate(g);}
}
}
