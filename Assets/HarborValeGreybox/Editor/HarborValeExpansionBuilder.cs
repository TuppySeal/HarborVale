using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HarborValeGreybox.Editor {
// Repeatable map/model authoring. Never runs automatically on import.
public static class HarborValeExpansionBuilder {
 const string Root="Assets/HarborValeGreybox";
 const string Scene=Root+"/Scenes/HarborVale_CityMap.unity";
 [Serializable] public class P {public float x,y,z;public Vector3 V=>new Vector3(x,y,z);}
 [Serializable] public class Road {public string id,name;public float width;public int lanes;public bool sidewalk;public P[] points;}
 [Serializable] public class Junction:P {public string id;public float radius;}
 [Serializable] public class Site:P {public string id,name,district,kind;public float w,d,h,yaw,accessX,accessZ;}
 [Serializable] public class Car:P {public string id,name;public float length,width,height,wheelbase,wheelRadius,massKg,yaw;}
 [Serializable] public class Layout {public Road[] roads;public Junction[] junctions;public Site[] sites;public Car car;}
 static Layout map;static Transform world;static string assetDir;static int assetIndex;
 static Material asphalt,concrete,paint,grass,glass,metal,rubber,carPaint;
 static float H(float z){float t=Mathf.Clamp01((z-340)/1000);return 84*t*t*(3-2*t);}
 static GameObject Group(string name,Transform parent=null){var g=new GameObject(name);g.transform.SetParent(parent,false);return g;}
 static Material Mat(string name,Color color,float smooth=0){string path=Root+"/Expansion/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find("Standard"));AssetDatabase.CreateAsset(m,path);}m.color=color;m.SetFloat("_Glossiness",smooth);return m;}
 static GameObject Box(string name,Transform parent,Vector3 pos,Vector3 scale,Material mat,bool collider=true){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=mat;if(!collider)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
 static GameObject MeshObject(string name,Transform parent,List<Vector3> v,List<int> t,Material mat,bool collision=false){
  var mesh=new Mesh{name=name,indexFormat=IndexFormat.UInt32};mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
  AssetDatabase.CreateAsset(mesh,assetDir+"/Mesh_"+(assetIndex++).ToString("D4")+".asset");
  var g=Group(name,parent);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=mat;if(collision)g.AddComponent<MeshCollider>().sharedMesh=mesh;return g;
 }
 static void Quad(List<int> t,int a,int b,int c,int d){t.Add(a);t.Add(c);t.Add(b);t.Add(a);t.Add(d);t.Add(c);}
 static void Label(Transform parent,string text,Vector3 position,float size,Color color){
  var g=Group("Label | "+text,parent);g.transform.localPosition=position;
  var tm=g.AddComponent<TextMesh>();tm.text=text;tm.fontSize=48;tm.characterSize=size/4.8f;tm.anchor=TextAnchor.MiddleCenter;tm.alignment=TextAlignment.Center;tm.color=color;
  tm.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");g.GetComponent<Renderer>().sharedMaterial=tm.font.material;
  var l=g.AddComponent<MapLabel>();l.fullCaption=text;l.shortCaption=text.Split('\n')[0];
 }
 static float JunctionDistance(Vector3 p){float d=99999;foreach(var j in map.junctions)d=Mathf.Min(d,Vector2.Distance(new Vector2(p.x,p.z),new Vector2(j.x,j.z))-j.radius);return d;}
 static void Strip(string name,Transform parent,Road r,float offset,float width,float lift,Material material,bool collider,bool skipJunction=false){
  var v=new List<Vector3>();var t=new List<int>();
  for(int i=0;i<r.points.Length;i++){
   Vector3 p=r.points[i].V;Vector3 tangent=r.points[Math.Min(i+1,r.points.Length-1)].V-r.points[Math.Max(0,i-1)].V;tangent.y=0;tangent.Normalize();var right=new Vector3(tangent.z,0,-tangent.x);
   var a=p+right*(offset-width/2);var b=p+right*(offset+width/2);a.y=H(a.z)+lift;b.y=H(b.z)+lift;v.Add(a);v.Add(b);
   if(i>0&&(!skipJunction||JunctionDistance(p)>7&&JunctionDistance(r.points[i-1].V)>7))Quad(t,(i-1)*2,(i-1)*2+1,i*2+1,i*2);
  }
  MeshObject(name,parent,v,t,material,collider);
 }
 static void RoadBuild(Road r,Transform parent){
  var group=Group(r.id+" | "+r.name+" | "+r.width+" m",parent).transform;
  Strip("Carriageway",group,r,0,r.width,.08f,asphalt,true);
  for(int side=-1;side<=1;side+=2){
   if(r.sidewalk){Strip("Sidewalk "+side,group,r,side*(r.width/2+1.65f),3,.23f,concrete,true,true);Strip("Curb "+side,group,r,side*(r.width/2+.15f),.3f,.24f,concrete,true,true);}
   else Strip("Shoulder "+side,group,r,side*(r.width/2+.6f),1.2f,.065f,concrete,true);
   Strip("Edge line "+side,group,r,side*(r.width/2-.25f),.10f,.10f,paint,false,true);
  }
  float distance=0,nextDash=0;var dv=new List<Vector3>();var dt=new List<int>();
  for(int i=1;i<r.points.Length-1;i++){
   var p=r.points[i].V;distance+=Vector3.Distance(p,r.points[i-1].V);
   if(distance<nextDash||JunctionDistance(p)<15)continue;nextDash=distance+12;
   var forward=(r.points[i+1].V-r.points[i-1].V).normalized;var right=Vector3.Cross(Vector3.up,forward).normalized;
   foreach(float offset in r.lanes==4?new[]{-3.5f,0,3.5f}:new[]{0f}){
    int n=dv.Count;foreach(var q in new[]{p-forward*1.5f-right*.06f+right*offset,p-forward*1.5f+right*.06f+right*offset,p+forward*1.5f+right*.06f+right*offset,p+forward*1.5f-right*.06f+right*offset})dv.Add(new Vector3(q.x,H(q.z)+.105f,q.z));Quad(dt,n,n+1,n+2,n+3);
   }
  }
  MeshObject("Lane markings",group,dv,dt,paint);
  var mid=r.points[r.points.Length/2].V;Label(group,r.id+"\n"+r.name,mid+Vector3.up*7,6,Color.white);
 }
 static void Terrain(){
  var v=new List<Vector3>();var t=new List<int>();int nx=172,nz=150;
  for(int z=0;z<=nz;z++)for(int x=0;x<=nx;x++){float px=-1720+x*20,pz=-1200+z*20;v.Add(new Vector3(px,H(pz)-.02f,pz));}
  for(int z=0;z<nz;z++)for(int x=0;x<nx;x++){int a=z*(nx+1)+x;Quad(t,a,a+1,a+nx+2,a+nx+1);}
  MeshObject("Landform | meters | north rises to 84 m",world,v,t,grass,true);
  Box("Harbor water | visual boundary",world,new Vector3(-1930,-2,0),new Vector3(400,1,3200),Mat("Water",new Color(.08f,.24f,.32f)),false);
 }
 static void SiteBuild(Site s,Transform parent){
  var g=Group(s.id+" | "+s.name,parent).transform;g.position=s.V;g.rotation=Quaternion.Euler(0,s.yaw,0);
  var col=s.kind=="block"?new Color(.42f,.46f,.49f):s.kind=="fuel"?new Color(.84f,.61f,.20f):s.kind=="police"?new Color(.24f,.40f,.68f):s.kind=="event"?new Color(.59f,.39f,.72f):new Color(.20f,.58f,.55f);
  var mat=Mat("Kind_"+s.kind,col);
  if(s.kind=="event"||s.kind=="lookout"||s.kind=="meet"){
   Box("Marked activity area",g,new Vector3(0,.09f,0),new Vector3(s.w,.16f,s.d),mat);
  }else{
   float foundation=Mathf.Max(.08f,s.y-H(s.z-s.d*.6f)+1,s.y-H(s.z+s.d*.6f)+1);
   Box("Level forecourt / foundation",g,new Vector3(0,.08f-foundation/2,0),new Vector3(s.w+6,foundation,s.d+12),concrete);
   Vector3 access=new Vector3(s.accessX,H(s.accessZ)+.08f,s.accessZ);Vector3 entrance=g.TransformPoint(new Vector3(0,.09f,-s.d/2-5.8f));Vector3 delta=entrance-access;delta.y=0;float distance=delta.magnitude;delta.Normalize();Vector3 right=Vector3.Cross(Vector3.up,delta).normalized*2.2f;
   var driveV=new List<Vector3>();var driveT=new List<int>();
   foreach(float d in new[]{0f,Mathf.Min(.8f,distance*.25f),Mathf.Min(4,distance*.5f),distance}){Vector3 q=access+delta*d;q.y=d==0?access.y:d==distance?entrance.y:H(q.z)+.25f;driveV.Add(q-right);driveV.Add(q+right);if(driveV.Count>2){int n=driveV.Count;Quad(driveT,n-4,n-3,n-1,n-2);}}
   MeshObject(s.id+" access driveway",world,driveV,driveT,concrete,true);
   Box("Building | "+s.w+" x "+s.d+" m",g,new Vector3(0,s.h/2,0),new Vector3(s.w,s.h,s.d),mat);
   Box("Roof cap",g,new Vector3(0,s.h+.15f,0),new Vector3(s.w+.3f,.3f,s.d+.3f),metal,false);
   Box("Door | 2.1 m reference",g,new Vector3(-s.w*.25f,1.05f,-s.d/2-.025f),new Vector3(1,2.1f,.08f),metal,false);
   if(s.kind=="service")Box("Workshop roller door",g,new Vector3(s.w*.15f,1.5f,-s.d/2-.05f),new Vector3(3.2f,3,.1f),metal,false);
   if(s.kind=="block"&&s.h>8)for(float y=4;y<s.h-1;y+=3.2f)Box("Floor glazing "+y,g,new Vector3(0,y,-s.d/2-.03f),new Vector3(s.w*.8f,1.2f,.06f),glass,false);
   if(s.h>3&&s.h<14&&s.kind!="block")Box("Sign board",g,new Vector3(0,s.h-.7f,-s.d/2-.08f),new Vector3(s.w*.7f,.8f,.1f),paint,false);
  }
  if(s.kind!="block")Label(g,s.id+"\n"+s.name,new Vector3(0,s.h+4,0),s.kind=="event"?3:4,Color.white);
 }
 static void CarPanel(string name,Transform parent,Vector3[] points,Material mat){var v=new List<Vector3>(points);var t=new List<int>();Quad(t,0,1,2,3);int count=t.Count;for(int i=0;i<count;i+=3){t.Add(t[i]);t.Add(t[i+2]);t.Add(t[i+1]);}MeshObject(name,parent,v,t,mat);}
 static GameObject Cylinder(string name,Transform parent,Vector3 pos,float radius,float width,Material material){var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localRotation=Quaternion.Euler(0,0,90);g.transform.localScale=new Vector3(radius*2,width/2,radius*2);g.GetComponent<Renderer>().sharedMaterial=material;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
 static GameObject CarBuild(){
  var g=Group("Aster Finch | car_finch | 4.05 m hatchback");var tr=g.transform;
  // Lower body has real wheel-arch openings, cut into both side panels.
  var v=new List<Vector3>();var t=new List<int>();int samples=120;
  for(int i=0;i<=samples;i++){
   float z=-2.025f+4.05f*i/samples;float taper=Mathf.Clamp01((Mathf.Abs(z)-1.65f)/.375f);float half=.85f-.13f*taper;
   float bottom=.30f;
   foreach(float axle in new[]{-1.225f,1.225f}){float dz=z-axle;if(Mathf.Abs(dz)<.35f)bottom=Mathf.Max(bottom,.295f+Mathf.Sqrt(.35f*.35f-dz*dz));}
   float top=.79f+.09f*(1-taper);
   v.Add(new Vector3(-half,bottom,z));v.Add(new Vector3(-half,top,z));v.Add(new Vector3(half,top,z));v.Add(new Vector3(half,bottom,z));
   if(i>0){int n=i*4;for(int a=0;a<3;a++)Quad(t,n-4+a,n-4+a+1,n+a+1,n+a);}
  }
  Quad(t,3,2,1,0);int end=samples*4;Quad(t,end,end+1,end+2,end+3);
  MeshObject("Body shell with four wheel arches",tr,v,t,carPaint);
  var c0=new Vector3(-.78f,.89f,-1.35f);var c1=new Vector3(-.65f,1.43f,-.92f);var c2=new Vector3(-.64f,1.43f,.43f);var c3=new Vector3(-.77f,.89f,1.01f);
  CarPanel("Left cabin glass",tr,new[]{c0,c1,c2,c3},glass);
  CarPanel("Right cabin glass",tr,new[]{new Vector3(.78f,.89f,-1.35f),new Vector3(.77f,.89f,1.01f),new Vector3(.64f,1.43f,.43f),new Vector3(.65f,1.43f,-.92f)},glass);
  CarPanel("Windshield",tr,new[]{new Vector3(-.77f,.89f,1.01f),new Vector3(-.64f,1.43f,.43f),new Vector3(.64f,1.43f,.43f),new Vector3(.77f,.89f,1.01f)},glass);
  CarPanel("Rear hatch glass",tr,new[]{new Vector3(-.78f,.89f,-1.35f),new Vector3(.78f,.89f,-1.35f),new Vector3(.65f,1.43f,-.92f),new Vector3(-.65f,1.43f,-.92f)},glass);
  Box("Roof panel",tr,new Vector3(0,1.445f,-.245f),new Vector3(1.31f,.03f,1.39f),carPaint,false);
  for(int side=-1;side<=1;side+=2){
   Box("B pillar "+side,tr,new Vector3(side*.708f,1.16f,-.16f),new Vector3(.06f,.55f,.075f),rubber,false);
   Box("Window sill "+side,tr,new Vector3(side*.81f,.895f,-.16f),new Vector3(.04f,.035f,2.3f),rubber,false);
   Box("Mirror housing "+side,tr,new Vector3(side*.92f,1,.70f),new Vector3(.16f,.13f,.23f),carPaint,false);
   Box("Mirror face "+side,tr,new Vector3(side*.925f,1,.577f),new Vector3(.12f,.09f,.015f),metal,false);
   Box("Door handle "+side,tr,new Vector3(side*.856f,.78f,-.33f),new Vector3(.025f,.04f,.17f),rubber,false);
   Box("Door seam "+side,tr,new Vector3(side*.851f,.64f,-.62f),new Vector3(.008f,.27f,.012f),rubber,false);
   foreach(float z in new[]{-1.225f,1.225f}){
    var wheel=Group((z>0?"Front":"Rear")+(side<0?" left":" right")+" wheel pivot",tr).transform;wheel.localPosition=new Vector3(side*.777f,.295f,z);
    Cylinder("Tire | 590 mm diameter",wheel,Vector3.zero,.295f,.185f,rubber);
    Cylinder("Steel rim",wheel,new Vector3(side*.098f,0,0),.195f,.014f,metal);
    Cylinder("Hub cap",wheel,new Vector3(side*.11f,0,0),.085f,.018f,rubber);
    for(int n=0;n<8;n++){float angle=n*Mathf.PI/4;Cylinder("Rim ventilation "+n,wheel,new Vector3(side*.11f,Mathf.Sin(angle)*.143f,Mathf.Cos(angle)*.143f),.026f,.005f,rubber);}
   }
  }
  var head=Mat("Headlight",new Color(.88f,.91f,.74f),.6f);var tail=Mat("Taillight",new Color(.62f,.035f,.025f),.5f);
  foreach(int side in new[]{-1,1}){Box("Headlamp "+side,tr,new Vector3(side*.52f,.69f,2.011f),new Vector3(.34f,.17f,.04f),head,false);Box("Rear lamp "+side,tr,new Vector3(side*.61f,.68f,-2.01f),new Vector3(.19f,.24f,.04f),tail,false);}
  Box("Front bumper",tr,new Vector3(0,.40f,1.999f),new Vector3(1.44f,.16f,.05f),rubber,false);
  Box("Rear bumper",tr,new Vector3(0,.40f,-1.999f),new Vector3(1.44f,.16f,.05f),rubber,false);
  Box("Front grille",tr,new Vector3(0,.65f,2.022f),new Vector3(.52f,.13f,.025f),rubber,false);
  Box("Rear registration plate",tr,new Vector3(0,.62f,-2.03f),new Vector3(.46f,.12f,.015f),paint,false);
  Box("Front registration plate",tr,new Vector3(0,.40f,2.033f),new Vector3(.46f,.11f,.015f),paint,false);
  Box("Exhaust tip",tr,new Vector3(-.5f,.24f,-1.97f),new Vector3(.09f,.08f,.18f),metal,false);
  foreach(float x in new[]{-.36f,.36f}){Box("Front seat",tr,new Vector3(x,.94f,-.03f),new Vector3(.44f,.6f,.34f),rubber,false);Box("Head rest",tr,new Vector3(x,1.25f,-.08f),new Vector3(.29f,.18f,.12f),rubber,false);}
  var collider=g.AddComponent<BoxCollider>();collider.center=new Vector3(0,.77f,0);collider.size=new Vector3(1.70f,1.38f,4.05f);
  var metadata=g.AddComponent<StarterCarSpecification>();metadata.lengthM=4.05f;metadata.bodyWidthM=1.70f;metadata.heightM=1.46f;metadata.wheelbaseM=2.45f;metadata.massKg=1050;
  string prefab=Root+"/Expansion/Prefabs/AsterFinch.prefab";PrefabUtility.SaveAsPrefabAsset(g,prefab);UnityEngine.Object.DestroyImmediate(g);
  return AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
 }
 [MenuItem("Harbor Vale/Rebuild expanded map (replaces map scene)")]
 public static void Build(){
  map=JsonUtility.FromJson<Layout>(File.ReadAllText(Root+"/Expansion/harbor_vale_expanded_map.json"));
  if(File.Exists(Scene)){string backups="Verification/Expansion/SceneBackups";Directory.CreateDirectory(backups);File.Copy(Scene,backups+"/HarborVale_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")+".unity");}
  string meshPath=Root+"/Expansion/GeneratedMeshes";
  // Keep the previous scene intact until new meshes and the new scene can be saved.
  assetDir=AssetDatabase.GenerateUniqueAssetPath(meshPath+"/Revision2");
  Directory.CreateDirectory(assetDir);Directory.CreateDirectory(Root+"/Expansion/Materials");Directory.CreateDirectory(Root+"/Expansion/Prefabs");AssetDatabase.Refresh();assetIndex=0;
  asphalt=Mat("Asphalt",new Color(.14f,.16f,.18f));concrete=Mat("Concrete",new Color(.48f,.48f,.43f));paint=Mat("RoadPaint",new Color(.87f,.86f,.74f));grass=Mat("Land",new Color(.29f,.38f,.29f));glass=Mat("Glass",new Color(.11f,.19f,.23f),.7f);metal=Mat("Metal",new Color(.30f,.33f,.35f),.3f);rubber=Mat("Rubber",new Color(.035f,.041f,.047f));carPaint=Mat("FinchBlue",new Color(.14f,.37f,.49f),.6f);
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);world=Group("HARBOR VALE | scaled revision 2 | 1 unit = 1 meter").transform;
  Terrain();var roadParent=Group("Road network | "+map.roads.Length+" named roads",world).transform;foreach(var r in map.roads)RoadBuild(r,roadParent);
  var junctionParent=Group("Shared intersections | markings yield at junctions",world).transform;
  foreach(var j in map.junctions){var v=new List<Vector3>{j.V+Vector3.up*.115f};var t=new List<int>();for(int i=0;i<=32;i++){float a=i*Mathf.PI/16;float x=j.x+Mathf.Cos(a)*j.radius,z=j.z+Mathf.Sin(a)*j.radius;v.Add(new Vector3(x,H(z)+.115f,z));if(i>0){t.Add(0);t.Add(i+1);t.Add(i);}}MeshObject(j.id,junctionParent,v,t,asphalt);}
  var districts=new Dictionary<string,Transform>();foreach(var name in new[]{"Dockside","Midtown","Ridgeway"})districts[name]=Group(name+" | labeled sites",world).transform;
  foreach(var s in map.sites)SiteBuild(s,districts[s.district]);
  Label(world,"DOCKSIDE",new Vector3(-1020,25,-650),10,new Color(.94f,.78f,.50f));
  Label(world,"MIDTOWN",new Vector3(600,30,-590),10,new Color(.72f,.85f,1));
  Label(world,"RIDGEWAY",new Vector3(100,140,1050),10,new Color(.77f,.93f,.70f));
  var prefab=CarBuild();var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab);instance.transform.SetParent(world);instance.transform.position=map.car.V+Vector3.up*.10f;instance.transform.rotation=Quaternion.Euler(0,map.car.yaw,0);
  var person=Group("Scale reference | 1.75 m person",world).transform;person.position=instance.transform.position+instance.transform.right*2.5f;Box("Human torso",person,new Vector3(0,1.12f,0),new Vector3(.44f,.68f,.24f),paint,false);Box("Head",person,new Vector3(0,1.62f,0),new Vector3(.23f,.26f,.23f),paint,false);foreach(float x in new[]{-.12f,.12f})Box("Leg",person,new Vector3(x,.40f,0),new Vector3(.16f,.80f,.18f),metal,false);
  var camera=Group("Main Camera | 1 city 2 docks 3 midtown 4 ridge 5 car 6 street");camera.tag="MainCamera";var cam=camera.AddComponent<Camera>();cam.nearClipPlane=.1f;cam.farClipPlane=9000;cam.backgroundColor=new Color(.11f,.16f,.21f);camera.AddComponent<AudioListener>();camera.AddComponent<MapExplorer>();
  cam.orthographic=true;cam.orthographicSize=1650;camera.transform.SetPositionAndRotation(new Vector3(0,4000,250),Quaternion.Euler(90,0,0));
  var sun=Group("Sun");var light=sun.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.2f;light.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(48,-32,0);RenderSettings.ambientLight=new Color(.56f,.60f,.64f);RenderSettings.ambientMode=AmbientMode.Flat;
  EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();ValidateAndRender();
 }
 public static void ValidateAndRender(){
  EditorSceneManager.OpenScene(Scene);map=JsonUtility.FromJson<Layout>(File.ReadAllText(Root+"/Expansion/harbor_vale_expanded_map.json"));
  var car=UnityEngine.Object.FindFirstObjectByType<StarterCarSpecification>();if(car==null)throw new Exception("Missing starter model");
  if(Mathf.Abs(car.GetComponent<BoxCollider>().size.z-4.05f)>.001f)throw new Exception("Car scale mismatch");
  Physics.SyncTransforms();int roadChecks=0;
  foreach(var r in map.roads){var road=GameObject.Find(r.id+" | "+r.name+" | "+r.width+" m");if(road==null)throw new Exception("Missing road "+r.id);var collider=road.transform.Find("Carriageway").GetComponent<MeshCollider>();for(int i=4;i<r.points.Length-4;i+=35){var p=r.points[i].V;if(!collider.Raycast(new Ray(p+Vector3.up*2,Vector3.down),out var hit,5)||Mathf.Abs(hit.point.y-p.y-.08f)>.05f)throw new Exception("Road surface/collider discontinuity "+r.id+" sample "+i);roadChecks++;}}
  var bounds=new Bounds(Vector3.zero,Vector3.zero);bool first=true;foreach(var f in car.GetComponentsInChildren<MeshFilter>())foreach(var p in f.sharedMesh.vertices){var q=car.transform.InverseTransformPoint(f.transform.TransformPoint(p));if(first){bounds=new Bounds(q,Vector3.zero);first=false;}else bounds.Encapsulate(q);}
  if(bounds.size.y<1.44f||bounds.size.y>1.48f||bounds.size.z<4.04f||bounds.size.z>4.12f)throw new Exception("Visual starter model dimensions differ from specification: "+bounds.size);
  foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))if(r.sharedMaterial==null||r.sharedMaterial.shader==null||r.sharedMaterial.shader.name=="Hidden/InternalErrorShader")throw new Exception("Bad material "+r.name);
  string evidence=Path.GetFullPath("Verification/Expansion");Directory.CreateDirectory(evidence);var cam=Camera.main;
  Render(cam,evidence+"/City_Expanded.png",new Vector3(0,4000,250),Quaternion.Euler(90,0,0),true,1650);
  Render(cam,evidence+"/Curved_Streets.png",new Vector3(250,760,-900),Quaternion.Euler(48,-12,0),false,50);
  Render(cam,evidence+"/Starter_Finch.png",car.transform.TransformPoint(new Vector3(5.5f,2.8f,6)),Quaternion.LookRotation(car.transform.TransformPoint(new Vector3(0,.75f,0))-car.transform.TransformPoint(new Vector3(5.5f,2.8f,6))),false,38);
  Render(cam,evidence+"/Street_Scale.png",car.transform.TransformPoint(new Vector3(14,4,22)),Quaternion.LookRotation(car.transform.position+Vector3.up-car.transform.TransformPoint(new Vector3(14,4,22))),false,55);
  cam.orthographic=true;cam.orthographicSize=1650;cam.transform.SetPositionAndRotation(new Vector3(0,4000,250),Quaternion.Euler(90,0,0));
  EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
  File.WriteAllLines(evidence+"/dependency-paths.txt",AssetDatabase.GetDependencies(Scene,true));
  File.WriteAllText(evidence+"/verification.txt","PASS Unity "+Application.unityVersion+": scene saved/reopened, materials valid, starter prefab found, 4.05 m length collider checked. Actual visual model bounds (including mirrors/plates) "+bounds.size+" m. "+roadChecks+" road collider raycasts passed. Four actual Unity renders generated. Road geometry scale/grade checks in source-report.json. No driving, input or performance playtest claimed.\n");Debug.Log("HARBOR_VALE_EXPANSION_PASS");
 }
 static void Render(Camera cam,string file,Vector3 position,Quaternion rotation,bool ortho,float size){
  cam.transform.SetPositionAndRotation(position,rotation);cam.orthographic=ortho;if(ortho)cam.orthographicSize=size;else cam.fieldOfView=size;
  foreach(var l in UnityEngine.Object.FindObjectsByType<MapLabel>(FindObjectsSortMode.None)){l.Refresh(cam);l.GetComponent<Renderer>().enabled=ortho;}
  var rt=new RenderTexture(1920,1440,24);cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var image=new Texture2D(1920,1440,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1920,1440),0,0);image.Apply();File.WriteAllBytes(file,image.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(image);
  foreach(var l in UnityEngine.Object.FindObjectsByType<MapLabel>(FindObjectsSortMode.None))l.GetComponent<Renderer>().enabled=true;
 }
}
}
