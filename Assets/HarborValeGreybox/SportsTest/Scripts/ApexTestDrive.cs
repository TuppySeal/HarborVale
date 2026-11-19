using System;
using System.Collections;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.SportsTest {
[Serializable] public class ApexCatalog {
 public FinchDefinition stock,stageOne,turboBasic,turboStreet;public float gearboxShiftMultiplier;
 public FinchDefinition Build(int step){var source=step>=4?turboStreet:step>=3?turboBasic:step>=1?stageOne:stock;var d=JsonUtility.FromJson<FinchDefinition>(JsonUtility.ToJson(source));if(step>=2)d.shiftSeconds*=gearboxShiftMultiplier;return d;}
}
public class ApexTestDrive:MonoBehaviour {
 public FinchVehicle Car{get;private set;}public ApexCatalog Catalog{get;private set;}public int Step{get;private set;}public bool Ready{get;private set;}
 public string Message{get;private set;}="Free test parts; career saves and credits are untouched.";
 string[] labels={"Stock","Stage 1 tune","Gearbox support","Turbo + basic intercooler","Street intercooler"};
 public static void Hook(){if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_SportsTest")new GameObject("Aster Apex | isolated sports testing").AddComponent<ApexTestDrive>();}
 IEnumerator Start(){while((Car=UnityEngine.Object.FindAnyObjectByType<FinchVehicle>())==null)yield return null;Catalog=JsonUtility.FromJson<ApexCatalog>(Resources.Load<TextAsset>("ApexTestCatalog").text);Configure(Car,Catalog);MakeVisual(Car);Car.Recover();Ready=true;}
 public static void Configure(FinchVehicle car,ApexCatalog catalog){car.Initialize();car.PowertrainCarId="car_apex_test";car.displayName="ASTER APEX TEST";car.driveLabel="AWD";car.frontDriveShare=.45f;car.forwardGrip=1.7f;car.lateralGrip=1.65f;car.downforceCoefficient=2.2f;car.aeroCdA=.80f;car.antiRoll=6200;car.highSpeedSteer=6.5f;car.Body.mass=catalog.stock.massKg;car.Body.centerOfMass=new Vector3(0,.30f,0);car.InstallPowertrain(catalog.Build(0));var shell=car.GetComponent<BoxCollider>();shell.center=new Vector3(0,.67f,0);shell.size=new Vector3(1.75f,.87f,4.2f);foreach(var w in car.Wheels){w.sprungMass=car.Body.mass/4;w.suspensionSpring=new JointSpring{spring=35000,damper=4000,targetPosition=.5f};var f=w.forwardFriction;f.stiffness=car.forwardGrip;w.forwardFriction=f;f=w.sidewaysFriction;f.stiffness=car.lateralGrip;w.sidewaysFriction=f;}}
 public bool SelectStep(int next){if(!Ready||Car.SpeedKph>=5){Message="Stop below 5 km/h before changing test parts.";return false;}if(next<0||next>4||next>Step+1){Message="Install the preceding test packages first.";return false;}Step=next;Car.InstallPowertrain(Catalog.Build(Step),Step>=3?2:Step>=1?1:0);UnityEngine.Object.FindAnyObjectByType<HarborValeGreybox.Workshop.ExpandedWorkshop>()?.SyncSports(Step);Message=labels[Step]+" applied. Test configuration only; no money spent.";return true;}
 public static void MakeVisual(FinchVehicle car){
  foreach(var r in car.GetComponentsInChildren<Renderer>()){bool wheel=false;for(Transform t=r.transform;t!=car.transform&&t!=null;t=t.parent)if(t.name.Contains("wheel pivot"))wheel=true;if(!wheel)r.enabled=false;}
  var body=new Material(Shader.Find("Standard")){color=new Color(.06f,.34f,.75f)};body.SetFloat("_Metallic",.55f);body.SetFloat("_Glossiness",.65f);var dark=new Material(Shader.Find("Standard")){color=new Color(.025f,.035f,.055f)};var lamp=new Material(Shader.Find("Standard")){color=new Color(.85f,.94f,1)};var red=new Material(Shader.Find("Standard")){color=new Color(.8f,.02f,.025f)};
  var root=new GameObject("Original Aster Apex sports coupe | 4.3 x 1.85 m");root.transform.SetParent(car.transform,false);
  Box(root.transform,"Low sports body",new Vector3(0,.61f,0),new Vector3(1.85f,.37f,4.3f),body);Box(root.transform,"Hood",new Vector3(0,.82f,1.16f),new Vector3(1.72f,.12f,1.50f),body);Box(root.transform,"Rear deck",new Vector3(0,.86f,-1.42f),new Vector3(1.72f,.15f,1.12f),body);
  var cabin=Box(root.transform,"Raked dark canopy",new Vector3(0,1.02f,-.22f),new Vector3(1.42f,.42f,1.6f),dark);cabin.transform.localRotation=Quaternion.Euler(5,0,0);Box(root.transform,"Roof",new Vector3(0,1.25f,-.30f),new Vector3(1.40f,.06f,.86f),body);
  for(int i=-1;i<=1;i+=2){Box(root.transform,"Wing upright",new Vector3(i*.58f,1.05f,-1.73f),new Vector3(.07f,.40f,.10f),dark);Box(root.transform,"Headlamp",new Vector3(i*.64f,.77f,2.16f),new Vector3(.43f,.08f,.035f),lamp);Box(root.transform,"Rear lamp",new Vector3(i*.64f,.74f,-2.16f),new Vector3(.43f,.07f,.035f),red);}
  Box(root.transform,"Rear aero wing",new Vector3(0,1.25f,-1.73f),new Vector3(1.95f,.09f,.36f),dark);Box(root.transform,"Front splitter",new Vector3(0,.40f,1.97f),new Vector3(1.94f,.06f,.48f),dark);Box(root.transform,"Front intake",new Vector3(0,.59f,2.16f),new Vector3(1.2f,.18f,.03f),dark);
 }
 static GameObject Box(Transform parent,string name,Vector3 position,Vector3 scale,Material material){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=material;UnityEngine.Object.Destroy(g.GetComponent<Collider>());return g;}
 void OnGUI(){if(UnityEngine.Object.FindAnyObjectByType<HarborValeGreybox.Workshop.ExpandedWorkshop>()?.Ready==true)return;if(!Ready)return;float width=Mathf.Min(930,Screen.width-32);float x=16,y=Screen.height-158;GUI.Box(new Rect(x,y,width,146),"SPORTS TEST LAB | ONE PARTS COUNTER | FREE TEST CONFIGURATIONS");GUI.Label(new Rect(x+14,y+25,width-28,24),"Installed: "+labels[Step]+" | "+Car.Engine.definition.PeakKw().ToString("F0")+" kW | downforce at 100 km/h: "+(2.2f*10000/12.96f).ToString("F0")+" N");for(int i=0;i<5;i++){GUI.enabled=Car.SpeedKph<5&&i<=Step+1;float bw=(width-28)/5;if(GUI.Button(new Rect(x+14+i*bw,y+53,bw-5,32),labels[i]))SelectStep(i);}GUI.enabled=true;GUI.Label(new Rect(x+14,y+91,width-28,43),Message+"\nHigher grip, AWD and physical aero. R recovers; changing parts clears temporary spool. No race rewards in this scene.");}
}
}
