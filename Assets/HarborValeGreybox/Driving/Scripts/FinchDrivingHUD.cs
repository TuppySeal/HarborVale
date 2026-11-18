using UnityEngine;
namespace HarborValeGreybox.Driving {
public class FinchDrivingHUD:MonoBehaviour {
 public FinchVehicle vehicle;public FinchChaseCamera chase;public MapExplorer map;
 bool inspection;AudioSource engineAudio;
 void Start(){
  engineAudio=gameObject.AddComponent<AudioSource>();engineAudio.loop=true;engineAudio.spatialBlend=.15f;engineAudio.minDistance=3;engineAudio.maxDistance=35;
  int count=22050;var wave=new float[count];for(int i=0;i<count;i++){float t=i/22050f;wave[i]=(.55f*Mathf.Sin(2*Mathf.PI*80*t)+.25f*Mathf.Sin(2*Mathf.PI*160*t)+.13f*Mathf.Sin(2*Mathf.PI*240*t))*.3f;}
  var clip=AudioClip.Create("Original procedural four cylinder loop",count,1,22050,false);clip.SetData(wave,0);engineAudio.clip=clip;engineAudio.Play();
 }
 void Update(){
  if(FinchInput.Shortcut(KeyCode.Tab)&&map!=null)SetInspection(!inspection);
  if(FinchInput.Shortcut(KeyCode.C)&&!inspection){chase.bonnet=!chase.bonnet;chase.Snap();}
  if(engineAudio!=null){engineAudio.pitch=Mathf.Clamp(vehicle.Engine.Rpm/2400,.35f,2.8f);engineAudio.volume=inspection?.035f:.10f+vehicle.Throttle*.12f;}
 }
 public void SetInspection(bool value){if(map==null)return;inspection=value;map.enabled=value;chase.enabled=!value;vehicle.playerInput=!value;FinchDrivingBootstrap.SetMapLabels(value);if(!value){chase.GetComponent<Camera>().orthographic=false;chase.Snap();}}
 void OnGUI(){
  if(vehicle==null||vehicle.Engine==null)return;
  if(inspection){GUI.Box(new Rect(16,Screen.height-54,420,38),"MAP INSPECTION | TAB to return to driving");return;}
  var label=new GUIStyle(GUI.skin.label){fontSize=16};label.normal.textColor=Color.white;
  GUI.Box(new Rect(16,16,690,106),"ASTER FINCH | STOCK 1.4 | FWD");
  GUI.Label(new Rect(30,44,660,30),Mathf.RoundToInt(vehicle.SpeedKph)+" km/h    "+Mathf.RoundToInt(vehicle.Engine.Rpm)+" RPM    "+(vehicle.Engine.Gear<0?"R":vehicle.Engine.Gear.ToString())+(vehicle.Engine.Shifting?"   SHIFT":""),label);
  GUI.Label(new Rect(30,77,660,36),"W / S accelerate, brake, reverse  |  A / D steer  |  Space handbrake\nR recover on road  |  C camera  |  Tab map inspection");
 }
 void OnDestroy(){if(engineAudio!=null&&engineAudio.clip!=null)Destroy(engineAudio.clip);}
}
}

