using System;
using System.Collections;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Garage {
public class FinchGarage:MonoBehaviour {
 public GarageCatalog Catalog{get;private set;}public DocksideTrialDirector Trial{get;private set;}public bool IsOpen{get;private set;}public string Message{get;private set;}public int AppliedStage{get;private set;}=-1;
 string requestId;TextMesh sign;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Hook(){if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_CityMap")new GameObject("Player garage | paid Stage 1").AddComponent<FinchGarage>();}
 IEnumerator Start(){while(Trial==null||Trial.Definition==null){Trial=UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>();yield return null;}var data=Resources.Load<TextAsset>("FinchGarageCatalog");if(data==null)throw new InvalidOperationException("Garage data missing");Catalog=JsonUtility.FromJson<GarageCatalog>(data.text);Catalog.Validate();if(Trial.Store!=null)ApplyInstalled();
  var label=new GameObject("Player garage sign");label.transform.SetParent(transform,false);label.transform.position=Catalog.Position+Vector3.up*5;sign=label.AddComponent<TextMesh>();sign.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");sign.fontSize=48;sign.characterSize=.15f;sign.anchor=TextAnchor.MiddleCenter;sign.alignment=TextAlignment.Center;sign.text="PLAYER GARAGE\nG — WORKSHOP";sign.color=new Color(.5f,.9f,1);label.GetComponent<Renderer>().sharedMaterial=sign.font.material;
 }
 public bool NearGarage=>Catalog!=null&&Trial!=null&&Vector3.Distance(Trial.vehicle.transform.position,Catalog.Position)<26;
 public bool Open(){
  if(Catalog==null||Trial.Store==null||Trial.Active||Trial.ShowingResult||!NearGarage||Trial.vehicle.SpeedKph>=5||!Trial.vehicle.playerInput)return false;
  try{Trial.Store.Refresh();ApplyInstalled();}catch(Exception e){Message="Saved progress could not be refreshed.";Debug.LogWarning(e.Message);return false;}
  Trial.vehicle.GetComponent<FinchDrivingHUD>()?.SetInspection(false);Trial.ExternalModalOpen=true;Trial.vehicle.controlsLocked=true;IsOpen=true;requestId=Guid.NewGuid().ToString("N");Message=null;return true;
 }
 public void Close(){if(!IsOpen)return;IsOpen=false;if(Trial!=null){Trial.ExternalModalOpen=false;if(Trial.vehicle!=null)Trial.vehicle.controlsLocked=false;}Message=null;}
 public bool Purchase(){
  if(!IsOpen||Trial.Store==null)return false;
  try{Trial.Store.PurchaseStageOne(requestId,Catalog.offer,Catalog.offer.price,out bool duplicate);ApplyInstalled();Message=duplicate?"This tune is already installed.":"Stage 1 installed. Your payment and tune are saved.";return true;}
  catch(Exception e){if(Trial.Store.Profile.engineStage==1)ApplyInstalled();Message=e is InvalidOperationException?e.Message:"Could not save the purchase. No tune was installed; retry.";Debug.LogWarning("Garage purchase: "+e.Message);return false;}
 }
 public void ApplyInstalled(){if(Trial.Store==null)return;int stage=Trial.Store.Profile.engineStage;if(stage==AppliedStage)return;Trial.vehicle.InstallPowertrain(stage==1?Catalog.stageOne:Catalog.stock,stage);AppliedStage=stage;}
 void Update(){if(Catalog==null||Trial==null)return;if(FinchInput.Shortcut(KeyCode.G)){if(IsOpen)Close();else Open();}else if(IsOpen&&FinchInput.Shortcut(KeyCode.Escape))Close();else if(IsOpen&&FinchInput.Shortcut(KeyCode.Return)&&Trial.Store.Profile.engineStage==0&&Trial.Store.Profile.availableCredits>=Catalog.offer.price)Purchase();}
 void LateUpdate(){if(sign!=null){sign.gameObject.SetActive(!IsOpen);if(Camera.main!=null)sign.transform.rotation=Camera.main.transform.rotation;}}
 void OnGUI(){
  if(Catalog==null||Trial==null||Trial.Store==null)return;
  if(!IsOpen){if(NearGarage&&!Trial.Active&&!Trial.ShowingResult&&Trial.vehicle.playerInput)GUI.Box(new Rect(16,128,690,26),Trial.vehicle.SpeedKph<5?"G — PLAYER GARAGE  |  Stage 1 engine package  "+Catalog.offer.price+" cr":"Slow down to enter the player garage");return;}
  float x=(Screen.width-720)/2f,y=(Screen.height-510)/2f;Color oldColor=GUI.color;GUI.color=new Color(.035f,.05f,.07f,.98f);GUI.DrawTexture(new Rect(x,y,720,510),Texture2D.whiteTexture);GUI.color=oldColor;GUI.Box(new Rect(x,y,720,510),"PLAYER GARAGE  /  ASTER FINCH");
  var heading=new GUIStyle(GUI.skin.label){fontSize=20,fontStyle=FontStyle.Bold};heading.normal.textColor=Color.white;
  bool installed=Trial.Store.Profile.engineStage==1;int balance=Trial.Store.Profile.availableCredits;GUI.Label(new Rect(x+28,y+34,660,32),installed?"STAGE 1 — INSTALLED":"STOCK → STAGE 1",heading);
  GUI.Label(new Rect(x+28,y+77,660,55),"Available credits  "+balance+" cr    |    "+(installed?"Paid  "+Trial.Store.Profile.spending[0].credits:"Package price  "+Catalog.offer.price)+" cr\nNaturally aspirated ECU calibration + intake/exhaust breathing package\nCompatible with the stock engine and transmission.");
  GUI.Label(new Rect(x+28,y+142,660,38),"Peak crank power:  "+Catalog.stock.PeakKw().ToString("F1")+" kW stock  →  "+Catalog.stageOne.PeakKw().ToString("F1")+" kW Stage 1\nTorque delivery changes with RPM. Gray: stock. Teal: Stage 1.");
  var graph=new Rect(x+50,y+200,620,142);GUI.Box(graph,"");Plot(graph,Catalog.stock,new Color(.65f,.68f,.72f));Plot(graph,Catalog.stageOne,new Color(.15f,.95f,.8f));GUI.Label(new Rect(graph.x,graph.yMax+2,100,24),"900 RPM");GUI.Label(new Rect(graph.xMax-80,graph.yMax+2,100,24),"6500 RPM");GUI.Label(new Rect(graph.x-28,graph.y+2,28,24),"140");GUI.Label(new Rect(graph.x-28,graph.y+64,28,24),"70");GUI.Label(new Rect(graph.x-28,graph.yMax-22,28,24),"0");GUI.Label(new Rect(graph.x+260,graph.yMax+2,180,24),"Crank torque, Nm");
  if(Message!=null)GUI.Label(new Rect(x+28,y+375,660,38),Message);else GUI.Label(new Rect(x+28,y+375,660,38),installed?"Your paid build survives restarting, recovery and race retries.":balance<Catalog.offer.price?"Earn "+(Catalog.offer.price-balance)+" more credits in First Lap to afford this package.":"After purchase: "+(balance-Catalog.offer.price)+" cr remaining.");
  bool previousEnabled=GUI.enabled;GUI.enabled=!installed&&balance>=Catalog.offer.price;if(GUI.Button(new Rect(x+28,y+432,398,44),installed?"STAGE 1 INSTALLED":"Enter — Install Stage 1 for "+Catalog.offer.price+" cr"))Purchase();GUI.enabled=previousEnabled;if(GUI.Button(new Rect(x+448,y+432,244,44),"Esc / G — Return to driving"))Close();
 }
 static void Plot(Rect rect,FinchDefinition definition,Color color){float max=140;var oldColor=GUI.color;var oldMatrix=GUI.matrix;GUI.color=color;for(int i=1;i<definition.rpm.Length;i++){Vector2 a=new Vector2(rect.x+12+(definition.rpm[i-1]-900)/5600*(rect.width-24),rect.yMax-10-definition.torque[i-1]/max*(rect.height-20)),b=new Vector2(rect.x+12+(definition.rpm[i]-900)/5600*(rect.width-24),rect.yMax-10-definition.torque[i]/max*(rect.height-20));GUIUtility.RotateAroundPivot(Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg,a);GUI.DrawTexture(new Rect(a.x,a.y,Vector2.Distance(a,b),2),Texture2D.whiteTexture);GUI.matrix=oldMatrix;}GUI.color=oldColor;}
 void OnDestroy(){if(IsOpen)Close();}
}
}
