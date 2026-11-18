using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace HarborValeGreybox.Driving.Editor {
public static class FinchDrivingVerification {
 const string Root="Assets/HarborValeGreybox";
 static string Evidence=>Path.GetFullPath("Verification/Driving");
 static List<string> results=new List<string>();
 static FinchVehicle car;static SimulationMode priorMode;
 static void Check(bool ok,string message){results.Add((ok?"PASS ":"FAIL ")+message);if(!ok)throw new Exception(message);}
 static GameObject Box(string name,Vector3 pos,Vector3 size,Color color){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=pos;g.transform.localScale=size;string folder=Root+"/Driving/Materials";if(!AssetDatabase.IsValidFolder(folder)){Directory.CreateDirectory(folder);AssetDatabase.Refresh();}string path=folder+"/Pad_"+Mathf.RoundToInt(color.r*100)+"_"+Mathf.RoundToInt(color.g*100)+"_"+Mathf.RoundToInt(color.b*100)+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(mat==null){mat=new Material(Shader.Find("Standard")){color=color};AssetDatabase.CreateAsset(mat,path);}g.GetComponent<Renderer>().sharedMaterial=mat;return g;}
 [MenuItem("Harbor Vale/Driving/Create handling test scene (new scene)")]
 public static void CreateTestScene(){
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  Box("Handling pad | 600 x 1800 m",new Vector3(0,-.25f,300),new Vector3(600,.5f,1800),new Color(.23f,.26f,.29f));
  var white=new Color(.9f,.88f,.77f);for(int z=-400;z<1100;z+=12)Box("Distance line "+z+" m",new Vector3(0,.01f,z),new Vector3(.12f,.01f,3),white);
  Box("Collision barrier",new Vector3(0,1.5f,1050),new Vector3(50,3,1),new Color(.65f,.30f,.18f));
  Box("Low curb | 8 cm",new Vector3(50,.04f,40),new Vector3(12,.08f,2),white);
  var ramp=Box("Landing ramp | 8 deg",new Vector3(70,.50f,100),new Vector3(12,.3f,12),new Color(.4f,.45f,.45f));ramp.transform.rotation=Quaternion.Euler(-8,0,0);
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Expansion/Prefabs/AsterFinch.prefab");var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab);instance.transform.position=new Vector3(0,.35f,0);
  var camera=new GameObject("Main Camera");camera.tag="MainCamera";var cam=camera.AddComponent<Camera>();cam.farClipPlane=3500;camera.AddComponent<AudioListener>();camera.transform.SetPositionAndRotation(new Vector3(6,3,-7),Quaternion.Euler(15,-30,0));
  var sun=new GameObject("Sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.2f;sun.transform.rotation=Quaternion.Euler(45,-25,0);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.6f,.65f,.7f);
  Directory.CreateDirectory(Root+"/Driving/Scenes");AssetDatabase.Refresh();EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),Root+"/Driving/Scenes/Finch_HandlingTest.unity");
  // Create a separately reusable driveable prefab; wheel physics is assembled in Awake.
  string path=Root+"/Expansion/Prefabs/AsterFinch.prefab";var contents=PrefabUtility.LoadPrefabContents(path);try{var v=contents.GetComponent<FinchVehicle>();if(v==null)v=contents.AddComponent<FinchVehicle>();v.stockDefinition=Resources.Load<TextAsset>("FinchStockPowertrain");PrefabUtility.SaveAsPrefabAsset(contents,Root+"/Driving/AsterFinch_Driveable.prefab");}finally{PrefabUtility.UnloadPrefabContents(contents);}
  AssetDatabase.SaveAssets();
 }
 static FinchVehicle Spawn(Vector3 position,Quaternion rotation){
  if(car!=null)UnityEngine.Object.DestroyImmediate(car.gameObject);
  var model=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Expansion/Prefabs/AsterFinch.prefab");var g=(GameObject)PrefabUtility.InstantiatePrefab(model);g.transform.SetPositionAndRotation(position,rotation);car=g.AddComponent<FinchVehicle>();car.Initialize();car.externalInput=true;car.playerInput=false;Physics.SyncTransforms();return car;
 }
 static void Sim(float seconds,float pedal=0,float steer=0,bool handbrake=false){car.commandedPedal=pedal;car.commandedSteer=steer;car.commandedHandbrake=handbrake;for(int i=0;i<Mathf.RoundToInt(seconds/.02f);i++){car.Tick(.02f);Physics.Simulate(.02f);}}
 static void Fresh(){Spawn(new Vector3(0,.35f,0),Quaternion.identity);Sim(3);}
 public static void Run(){
  Directory.CreateDirectory(Evidence);results.Clear();priorMode=Physics.simulationMode;
  try{
   CreateTestScene();var authored=UnityEngine.Object.FindAnyObjectByType<StarterCarSpecification>();UnityEngine.Object.DestroyImmediate(authored.gameObject);Physics.simulationMode=SimulationMode.Script;
   Fresh();Check(car.GroundedWheels==4&&car.SpeedKph<.5f,"Rest: four grounded wheels, speed "+car.SpeedKph.ToString("F3")+" km/h");
   var initial=car.transform.position;float t100=-1;for(int i=0;i<1000;i++){Sim(.02f,1);if(car.SpeedKph>=100&&t100<0){t100=(i+1)*.02f;break;}}
   Check(t100>0&&t100<22,"Stock 0-100 km/h: "+t100.ToString("F2")+" s; speed "+car.SpeedKph.ToString("F1")+", gear "+car.Engine.Gear+", rpm "+car.Engine.Rpm.ToString("F0"));
   Check(car.Engine.Gear>=2&&car.Engine.Rpm<=car.Engine.definition.redlineRpm+110,"Automatic shifts and bounded RPM");
   Vector3 brakeStart=car.transform.position;float brakeSeconds=0;while(car.SpeedKph>.5f&&brakeSeconds<8){Sim(.02f,-1);brakeSeconds+=.02f;}
   float brakeDistance=Vector3.Distance(brakeStart,car.transform.position);Check(brakeSeconds<7&&brakeDistance<100,"100-0 braking: "+brakeSeconds.ToString("F2")+" s, "+brakeDistance.ToString("F1")+" m");
   Sim(3,-1);Check(car.ForwardSpeed< -2&&car.Engine.Gear==-1,"Reverse after braking: "+car.ForwardSpeed.ToString("F2")+" m/s");
   Fresh();Sim(5,1);float yaw=car.transform.eulerAngles.y;Sim(2,.3f,.45f);float turn=Mathf.Abs(Mathf.DeltaAngle(yaw,car.transform.eulerAngles.y));Check(turn>12&&Vector3.Dot(car.transform.up,Vector3.up)>.8f,"Steering turn: "+turn.ToString("F1")+" deg, upright");
   Sim(1,0,.55f,true);Check(car.Wheels[2].brakeTorque>1800&&car.Wheels[2].sidewaysFriction.stiffness<.7f,"Handbrake locks rear wheels and reduces rear lateral grip");
   Fresh();car.Body.linearVelocity=Vector3.forward*12;var wall=Box("Verification collision wall",new Vector3(0,1.5f,12),new Vector3(20,3,1),Color.red);Physics.SyncTransforms();Sim(2,1);Check(car.transform.position.z<13,"Body collision blocks barrier penetration");UnityEngine.Object.DestroyImmediate(wall);
   car.ResetAt(new Vector3(30,3,0),Quaternion.Euler(0,0,170));car.Recover();Check(car.Body.linearVelocity.sqrMagnitude<.001f&&Vector3.Dot(car.transform.up,Vector3.up)>.99f,"Recovery clears velocity and restores upright spawn");
   // Seam/curb and a short ramp landing are exercised in the physical test area.
   car.ResetAt(new Vector3(50,.35f,28),Quaternion.identity);Sim(2);car.Body.linearVelocity=Vector3.forward*7;Sim(3,.4f);Check(car.transform.position.z>43&&Vector3.Dot(car.transform.up,Vector3.up)>.7f&&car.transform.position.y<1.5f,"8 cm curb traversal remains upright without launch");
   car.ResetAt(new Vector3(70,.35f,88),Quaternion.identity);Sim(2);car.Body.linearVelocity=Vector3.forward*8;Sim(4,.3f);Check(car.transform.position.z>108&&car.GroundedWheels>=2&&Vector3.Dot(car.transform.up,Vector3.up)>.7f,"Ramp traversal and landing return to grounded upright motion");
   // Render the car using the real follow-camera component.
   Fresh();Sim(2,1);var cam=Camera.main;var follow=cam.gameObject.AddComponent<FinchChaseCamera>();follow.target=car;follow.Follow(0);Render(cam,Evidence+"/Finch_Driving_Test.png");
   Check(!cam.orthographic&&Vector3.Distance(cam.transform.position,car.transform.position)>2,"Chase camera initializes in perspective");
   UnityEngine.Object.DestroyImmediate(car.gameObject);car=null;EditorSceneManager.OpenScene(Root+"/Scenes/HarborVale_CityMap.unity");var city=UnityEngine.Object.FindAnyObjectByType<StarterCarSpecification>();var pos=city.transform.position;Check(FinchDrivingBootstrap.TryRecovery(pos,city.transform.forward,out var recovery,out var facing),"City road recovery anchor found");
   UnityEngine.Object.DestroyImmediate(city.gameObject);Spawn(recovery,facing);Sim(3);Check(car.GroundedWheels>=3&&car.SpeedKph<1,"City spawn rests on actual road collision");Sim(2,1);Check(car.SpeedKph>8,"City road acceleration: "+car.SpeedKph.ToString("F1")+" km/h");
   File.WriteAllLines(Evidence+"/physics-results.txt",results);Debug.Log("FINCH_DRIVING_PHYSICS_PASS");
  }catch(Exception e){results.Add(e.ToString());File.WriteAllLines(Evidence+"/physics-results.txt",results);throw;}finally{Physics.simulationMode=priorMode;}
 }
 static void Render(Camera cam,string file){var rt=new RenderTexture(1600,1000,24);cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var image=new Texture2D(1600,1000,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,1000),0,0);image.Apply();File.WriteAllBytes(file,image.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(image);UnityEngine.Object.DestroyImmediate(rt);}
}
}

