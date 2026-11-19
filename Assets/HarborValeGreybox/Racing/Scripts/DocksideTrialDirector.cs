using System;
using System.IO;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing {
public class DocksideTrialDirector:MonoBehaviour,IFinchRecoveryHandler {
 public FinchVehicle vehicle;public TrialDefinition Definition{get;private set;}public TrialSession Session{get;private set;}public RaceProgressStore Store{get;private set;}
 public TrialReceipt Receipt{get;private set;}public string SaveError{get;private set;}public bool ShowingResult{get;private set;}
 GameObject markers;Transform[] arches;LineRenderer routeLine;Vector3 previous;int visualGate=-2;Material cyan,amber,white;TextMesh startLabel;float flashUntil;string flash;double lastTickTime;
 // Isolated editor probes set this before scene startup; players leave it null.
 public static string SavePathOverrideForTests;
 public bool ExternalModalOpen{get;set;}
 public static string OlderSavePath=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"HarborVale","Prototype","race-progress-v1.json");
 public static string LegacySavePath=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"HarborVale","Prototype","race-progress-v2.json");
 public static string PreviousSavePath=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"HarborVale","Prototype","race-progress-v3.json");
 public static string SavePath=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"HarborVale","Prototype","race-progress-v4.json");
 public bool Active=>Session!=null&&(Session.State==TrialState.Countdown||Session.State==TrialState.Running);
 public static void Hook(){if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name!="HarborVale_CityMap")return;var root=new GameObject("First Lap | time trial");root.AddComponent<DocksideTrialDirector>();}
 void Start(){Initialize();}
 public void Initialize(string saveOverride=null){
  if(Definition!=null)return;var data=Resources.Load<TextAsset>("FirstDocksideTrial");if(data==null)throw new InvalidOperationException("Trial route missing");Definition=JsonUtility.FromJson<TrialDefinition>(data.text);Definition.Validate();vehicle=UnityEngine.Object.FindAnyObjectByType<FinchVehicle>();
  if(vehicle==null){var spec=UnityEngine.Object.FindAnyObjectByType<StarterCarSpecification>();if(spec==null)throw new InvalidOperationException("No starter vehicle");vehicle=spec.gameObject.AddComponent<FinchVehicle>();vehicle.Initialize();}
  vehicle.RecoveryHandler=this;
  try{Store=new RaceProgressStore(saveOverride??SavePathOverrideForTests??SavePath,Definition,legacyFile:saveOverride==null&&SavePathOverrideForTests==null?PreviousSavePath:null,olderLegacyFile:saveOverride==null&&SavePathOverrideForTests==null?LegacySavePath:null,oldestLegacyFile:saveOverride==null&&SavePathOverrideForTests==null?OlderSavePath:null);}catch(Exception e){SaveError="Saved race progress could not be read.";Debug.LogWarning("Race progress unavailable: "+e.Message);}
  previous=vehicle.transform.position;lastTickTime=Time.realtimeSinceStartupAsDouble;CreateVisuals();
 }
 Material Material(string name,Color color){var m=new Material(Shader.Find("Sprites/Default")){name=name,color=color};return m;}
 GameObject VisualBox(string name,Transform parent,Vector3 local,Vector3 size,Material material){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=local;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=material;g.GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;Destroy(g.GetComponent<Collider>());return g;}
 void CreateVisuals(){
  markers=new GameObject("Trial navigation | no collision");markers.transform.SetParent(transform,false);cyan=Material("Trial teal",new Color(.10f,.83f,.76f));amber=Material("Next checkpoint",new Color(1,.72f,.17f));white=Material("Route",new Color(.25f,.92f,.78f,.75f));
  var line=new GameObject("Road-following route");line.transform.SetParent(markers.transform,false);routeLine=line.AddComponent<LineRenderer>();routeLine.useWorldSpace=true;routeLine.sharedMaterial=white;routeLine.widthMultiplier=.20f;routeLine.positionCount=Definition.route.Length;routeLine.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;for(int i=0;i<Definition.route.Length;i++)routeLine.SetPosition(i,Definition.route[i].Position+Vector3.up*.13f);routeLine.enabled=false;
  arches=new Transform[Definition.checkpoints.Length];for(int i=0;i<arches.Length;i++){var gate=Definition.checkpoints[i];var arch=new GameObject("Checkpoint "+(i+1));arch.transform.SetParent(markers.transform,false);arch.transform.SetPositionAndRotation(gate.Position,Quaternion.LookRotation(gate.Forward));arches[i]=arch.transform;foreach(float side in new[]{-1f,1f})VisualBox("Gate post",arch.transform,new Vector3(gate.halfWidth*side,1.7f,0),new Vector3(.22f,3.4f,.22f),amber);VisualBox("Overhead gate",arch.transform,new Vector3(0,3.4f,0),new Vector3(gate.halfWidth*2,.16f,.16f),amber);arch.SetActive(false);}
  var caption=new GameObject("First Lap start label");caption.transform.SetParent(markers.transform,false);caption.transform.position=Definition.checkpoints[0].Position+Vector3.up*5;startLabel=caption.AddComponent<TextMesh>();startLabel.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");startLabel.text="FIRST LAP\nTIME TRIAL";startLabel.fontSize=48;startLabel.characterSize=.18f;startLabel.anchor=TextAnchor.MiddleCenter;startLabel.alignment=TextAlignment.Center;caption.GetComponent<Renderer>().sharedMaterial=startLabel.font.material;startLabel.color=new Color(.2f,1,.8f);
 }
 public bool BeginRun(){
  Initialize();if(Store==null||Active||ExternalModalOpen)return false;var hud=vehicle.GetComponent<FinchDrivingHUD>();if(hud!=null)hud.SetInspection(false);Session=new TrialSession(Definition);Receipt=null;SaveError=null;ShowingResult=false;
  var first=Definition.checkpoints[0];vehicle.ResetAt(first.Position-first.Forward*8+Vector3.up*.4f,Quaternion.LookRotation(first.Forward));vehicle.controlsLocked=true;previous=vehicle.transform.position;visualGate=-2;var follow=Camera.main==null?null:Camera.main.GetComponent<FinchChaseCamera>();if(follow!=null)follow.Snap();return true;
 }
 void ShowResult(){ShowingResult=true;vehicle.controlsLocked=true;var hud=vehicle.GetComponent<FinchDrivingHUD>();if(hud!=null)hud.SetInspection(false);}
 public void Abandon(){if(!Active)return;Session.Abandon();ShowResult();}
 public void DismissResult(bool forfeitUnpaid=false){if(SaveError!=null&&Session!=null&&Session.ValidFinish&&!forfeitUnpaid)return;ShowingResult=false;vehicle.controlsLocked=false;Session=null;Receipt=null;visualGate=-2;}
 void Update(){
  if(Definition==null||vehicle==null)return;Vector3 now=vehicle.transform.position;double realtime=Time.realtimeSinceStartupAsDouble;float dt=(float)Math.Max(0,realtime-lastTickTime);lastTickTime=realtime;
  if(Active){Session.Tick(dt,previous,now);vehicle.controlsLocked=Session.State==TrialState.Countdown;if(Session.State==TrialState.Finished){ShowResult();TrySettlement();}else if(Session.State==TrialState.Abandoned){ShowResult();}}
  previous=now;if(ExternalModalOpen){UpdateVisuals();return;}
  if(FinchInput.Shortcut(KeyCode.Escape)){if(Active)Abandon();else if(ShowingResult)DismissResult();}
  else if((vehicle.playerInput||ShowingResult)&&FinchInput.Shortcut(KeyCode.E)){if(ShowingResult){if(SaveError!=null&&Session!=null&&Session.ValidFinish)TrySettlement();else BeginRun();}else if(!Active&&NearStart()&&vehicle.SpeedKph<8)BeginRun();}
  UpdateVisuals();
 }
 public bool TrySettlement(){if(Session==null||!Session.ValidFinish||Store==null)return false;try{Receipt=Store.Settle(Session,out _);SaveError=null;return true;}catch(Exception e){SaveError="Progress could not be saved. Close any other copy and retry.";Debug.LogWarning("Race save failed: "+e.Message);return false;}}
 bool NearStart()=>Vector3.Distance(vehicle.transform.position,Definition.checkpoints[0].Position)<35;
 void UpdateVisuals(){
  int gate=Active?Session.NextGate:-1;if(gate==visualGate)return;visualGate=gate;for(int i=0;i<arches.Length;i++)arches[i].gameObject.SetActive(gate<0?i==0: i==gate||i==gate+1);
  if(gate>=0&&gate<arches.Length)foreach(var r in arches[gate].GetComponentsInChildren<Renderer>())r.sharedMaterial=amber;
  if(gate+1>=0&&gate+1<arches.Length)foreach(var r in arches[gate+1].GetComponentsInChildren<Renderer>())r.sharedMaterial=cyan;
  routeLine.enabled=Active;startLabel.gameObject.SetActive(!Active&&!ShowingResult);
 }
 void LateUpdate(){if(startLabel!=null&&Camera.main!=null)startLabel.transform.rotation=Camera.main.transform.rotation;}
 public bool TryRecover(FinchVehicle car){
  if(!Active)return false;if(Session.State==TrialState.Countdown)return true;
  var gate=Session.Recover();var position=gate.Position+gate.Forward*(Session.NextGate==0?-8:4)+Vector3.up*.4f;car.ResetAt(position,Quaternion.LookRotation(gate.Forward));previous=position;flash="RECOVERED  +3.0 seconds";flashUntil=Time.unscaledTime+3;var camera=Camera.main==null?null:Camera.main.GetComponent<FinchChaseCamera>();if(camera!=null)camera.Snap();return true;
 }
 static string TimeText(float seconds){int ms=Mathf.RoundToInt(seconds*1000);return (ms/60000).ToString("00")+":"+((ms/1000)%60).ToString("00")+"."+(ms%1000).ToString("000");}
 void OnGUI(){
  if(Definition==null||vehicle==null||ExternalModalOpen)return;var large=new GUIStyle(GUI.skin.label){fontSize=22,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter};large.normal.textColor=Color.white;var centered=new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter,fontSize=15,wordWrap=true};centered.normal.textColor=Color.white;
  bool restored=Store!=null&&Store.RecoveredBackup;GUI.Box(new Rect(Screen.width-270,16,254,restored?88:68),"RACE PROGRESS");GUI.Label(new Rect(Screen.width-255,42,238,restored?60:38),Store==null?"Save unavailable":"Available credits  "+Store.Profile.availableCredits+" cr\nReputation  "+Store.Profile.reputation+(restored?"\nLoaded last valid backup":""));
  if(ShowingResult){float x=(Screen.width-640)/2f,y=(Screen.height-350)/2f;GUI.Box(new Rect(x,y,640,350),Definition.title);bool finish=Session!=null&&Session.ValidFinish;string heading=finish?TrialDefinition.Medal(Definition.Rank(Session.Elapsed))+"  "+TimeText(Session.Elapsed):"RUN ABANDONED";GUI.Label(new Rect(x+20,y+40,600,40),heading,large);
   string detail=finish?(Receipt!=null?"Earned "+Receipt.credits+" cr"+(Receipt.firstClear?" (includes first-clear bonus)":"")+"\nReputation +"+Receipt.reputation+"\nTotal race earnings "+Store.Profile.earnings+" cr\nBest time "+TimeText(Store.Profile.records[0].bestMilliseconds/1000f):"Payout pending. Your previous progress is preserved."):"No credits awarded. Explore or try again.";
   if(SaveError!=null)detail+="\nCould not save: "+SaveError;
   GUI.Label(new Rect(x+30,y+100,580,140),detail,centered);if(GUI.Button(new Rect(x+60,y+260,240,45),SaveError!=null?"E — Retry save":"E — Retry time trial")){if(SaveError!=null)TrySettlement();else BeginRun();}if(GUI.Button(new Rect(x+340,y+260,240,45),SaveError!=null?"Leave without payout":"Esc — Free roam"))DismissResult(SaveError!=null);return;
  }
  if(Active){GUI.Box(new Rect(16,156,450,96),Definition.title);string medal="Gold "+TimeText(Definition.goldSeconds)+"   Silver "+TimeText(Definition.silverSeconds)+"   Bronze "+TimeText(Definition.bronzeSeconds);GUI.Label(new Rect(30,184,430,56),TimeText(Session.Elapsed)+"   Checkpoint "+Math.Min(Session.NextGate+1,Definition.checkpoints.Length)+" / "+Definition.checkpoints.Length+"\n"+medal+"\nEsc abandon   |   R recover +3 s   |   Map does not pause timer");
   if(Session.State==TrialState.Countdown)GUI.Label(new Rect(Screen.width/2-200,Screen.height/2-80,400,80),"READY  "+Mathf.CeilToInt(Session.Countdown),large);
   else{var target=Definition.checkpoints[Session.NextGate];var delta=target.Position-vehicle.transform.position;float angle=Vector3.SignedAngle(vehicle.transform.forward,Vector3.ProjectOnPlane(delta,Vector3.up),Vector3.up);string direction=Mathf.Abs(angle)>120?"TURN AROUND":angle>18?"RIGHT":angle< -18?"LEFT":"AHEAD";GUI.Box(new Rect(Screen.width/2-240,Screen.height-94,480,65),"NEXT CHECKPOINT  "+Mathf.RoundToInt(delta.magnitude)+" m");GUI.Label(new Rect(Screen.width/2-220,Screen.height-65,440,30),direction+((delta.magnitude<65&&Vector3.Angle(vehicle.transform.forward,target.Forward)>55)?" — BRAKE FOR TURN":""),centered);}
   if(Time.unscaledTime<flashUntil)GUI.Label(new Rect(Screen.width/2-220,270,440,35),flash,centered);return;
  }
  if(!vehicle.playerInput)return;float distance=Vector3.Distance(vehicle.transform.position,Definition.checkpoints[0].Position);GUI.Box(new Rect(Screen.width/2-300,Screen.height-120,600,94),Definition.title);
  string offer=Store==null?"Racing is unavailable until the existing save can be read.":NearStart()?(vehicle.SpeedKph<8?"E — Start time trial":"Slow down to start"):(Mathf.RoundToInt(distance)+" m to start — follow the FIRST LAP marker");GUI.Label(new Rect(Screen.width/2-280,Screen.height-91,560,65),offer+"\nGold reward "+Definition.baseReward+" cr   |   First-clear bonus "+Definition.firstClearBonus+" cr\nReplay to improve your time and earn more credits.",centered);
 }
 void OnDestroy(){if(vehicle!=null){vehicle.RecoveryHandler=null;vehicle.controlsLocked=false;}if(cyan!=null)Destroy(cyan);if(amber!=null)Destroy(amber);if(white!=null)Destroy(white);}
}
}
