using UnityEngine;
namespace HarborValeGreybox.Driving {
public class FinchDrivingHUD:MonoBehaviour {
 public FinchVehicle vehicle;public FinchChaseCamera chase;public MapExplorer map;
 bool inspection;AudioSource engineAudio;
 public void UseExternalEngineAudio(){if(engineAudio==null)return;engineAudio.Stop();if(engineAudio.clip!=null)Destroy(engineAudio.clip);Destroy(engineAudio);engineAudio=null;}
 void Start(){
  if(GetComponent<HarborValeGreybox.Workshop.WorkshopEffects>()!=null)return;
  engineAudio=gameObject.AddComponent<AudioSource>();engineAudio.volume=0;engineAudio.loop=true;engineAudio.spatialBlend=.15f;engineAudio.minDistance=3;engineAudio.maxDistance=35;
  int count=22050;var wave=new float[count];for(int i=0;i<count;i++){float t=i/22050f;wave[i]=(.55f*Mathf.Sin(2*Mathf.PI*80*t)+.25f*Mathf.Sin(2*Mathf.PI*160*t)+.13f*Mathf.Sin(2*Mathf.PI*240*t))*.3f;}
  var clip=AudioClip.Create("Original procedural four cylinder loop",count,1,22050,false);clip.SetData(wave,0);engineAudio.clip=clip;engineAudio.Play();
 }
 void Update(){
  if(FinchInput.Shortcut(KeyCode.Tab)&&map!=null&&!vehicle.controlsLocked)SetInspection(!inspection);
  if(FinchInput.Shortcut(KeyCode.C)&&!inspection){chase.bonnet=!chase.bonnet;chase.Snap();}
  if(engineAudio!=null){engineAudio.pitch=Mathf.Lerp(engineAudio.pitch,Mathf.Clamp(vehicle.Engine.Rpm/2400,.35f,2.8f),1-Mathf.Exp(-Time.unscaledDeltaTime/.08f));float wanted=!Application.isFocused&&!vehicle.externalInput?0:inspection?.035f:.10f+vehicle.Throttle*.12f;engineAudio.volume=Mathf.Lerp(engineAudio.volume,wanted,1-Mathf.Exp(-Time.unscaledDeltaTime/.025f));}
 }
 public void SetInspection(bool value){if(map==null)return;inspection=value;map.enabled=value;chase.enabled=!value;vehicle.playerInput=!value;FinchDrivingBootstrap.SetMapLabels(value);if(!value){chase.GetComponent<Camera>().orthographic=false;chase.Snap();}}
 void OnGUI(){
  if(vehicle==null||vehicle.Engine==null)return;
  if(inspection){GUI.Box(new Rect(16,Screen.height-54,420,38),"MAP INSPECTION | TAB to return to driving");return;}
  var label=new GUIStyle(GUI.skin.label){fontSize=16};label.normal.textColor=Color.white;
  GUI.Box(new Rect(16,16,690,106),"");
  GUI.Label(new Rect(30,20,405,23),vehicle.displayName+" | "+(GetComponent<HarborValeGreybox.Workshop.WorkshopEffects>()!=null?vehicle.Engine.definition.aspiration.ToUpperInvariant():vehicle.InstalledPrototypeStage>=2?"TURBO S2":vehicle.InstalledPrototypeStage==1?"STAGE 1 NA":(vehicle.PowertrainCarId=="car_apex_test"?"STOCK 2.0":"STOCK 1.4"))+" | "+vehicle.driveLabel);
  GUI.enabled=vehicle.playerInput&&!vehicle.controlsLocked;
  if(GUI.Button(new Rect(444,20,110,23),"M: "+(vehicle.Engine.Mode==TransmissionMode.Automatic?"AUTO":"MANUAL")))vehicle.ToggleTransmissionMode();
  if(GUI.Button(new Rect(560,20,60,23),"Z: -"))vehicle.PaddleShift(-1);
  if(GUI.Button(new Rect(626,20,60,23),"X: +"))vehicle.PaddleShift(1);GUI.enabled=true;
  GUI.Label(new Rect(30,44,660,30),HarborValeGreybox.Racing.DrivingUnits.Speed(vehicle.SpeedKph)+(vehicle.IsBurningOut?"  WHEELSPIN":"")+"    "+Mathf.RoundToInt(vehicle.Engine.Rpm)+" RPM    "+(vehicle.Engine.Mode==TransmissionMode.Automatic?"AUTO ":"MANUAL ")+(vehicle.Engine.Gear<0?"R":vehicle.Engine.Gear.ToString())+(vehicle.Engine.Shifting?"   SHIFT":"")+(vehicle.Engine.definition.IsTurbo?"   BOOST "+vehicle.Engine.BoostBar.ToString("F2")+" bar":""),label);
  GUI.Label(new Rect(30,77,660,36),vehicle.IsBrakeBoosting?"STAGING "+vehicle.Engine.StagingTargetRpm.ToString("F0")+" RPM — hold W + S, release S to launch":vehicle.Engine.ShiftNote??"W / S pedals  |  A / D steer  |  Space handbrake  |  Tab map\nR recover  |  C view  |  RMB look  |  J / L sides  |  K rear  |  W + S stage");
 }
 void OnDestroy(){if(engineAudio!=null&&engineAudio.clip!=null)Destroy(engineAudio.clip);}
}
}
