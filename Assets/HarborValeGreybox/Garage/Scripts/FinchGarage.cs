using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Garage {
public class FinchGarage:MonoBehaviour {
 public GarageCatalog Catalog{get;private set;}public DocksideTrialDirector Trial{get;private set;}public bool IsOpen{get;private set;}public string Message{get;private set;}public int AppliedStage{get;private set;}=-1;public int SelectedIndex{get;private set;}
 readonly Dictionary<string,string> requests=new Dictionary<string,string>();string appliedSignature;TextMesh sign;
 public EngineTuneOffer[] Offers=>new[]{Catalog.offer,Catalog.gearboxOffer,Catalog.turboOffer,Catalog.intercoolerOffer};
 public static void Hook(){if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_CityMap")new GameObject("Player garage | all prototype parts").AddComponent<FinchGarage>();}
 IEnumerator Start(){while(Trial==null||Trial.Definition==null){Trial=UnityEngine.Object.FindAnyObjectByType<DocksideTrialDirector>();yield return null;}var data=Resources.Load<TextAsset>("FinchGarageCatalog");if(data==null)throw new InvalidOperationException("Garage data missing");Catalog=JsonUtility.FromJson<GarageCatalog>(data.text);Catalog.Validate();if(Trial.Store!=null)ApplyInstalled();
  var g=new GameObject("Player garage sign");g.transform.SetParent(transform,false);g.transform.position=Catalog.Position+Vector3.up*5;sign=g.AddComponent<TextMesh>();sign.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");sign.fontSize=48;sign.characterSize=.15f;sign.anchor=TextAnchor.MiddleCenter;sign.alignment=TextAlignment.Center;sign.text="PLAYER GARAGE\nG — PARTS & TUNING";sign.color=new Color(.5f,.9f,1);g.GetComponent<Renderer>().sharedMaterial=sign.font.material;
 }
 public bool NearGarage=>Catalog!=null&&Trial!=null&&Vector3.Distance(Trial.vehicle.transform.position,Catalog.Position)<26;
 public bool Open(){if(Catalog==null||Trial.Store==null||Trial.Active||Trial.ShowingResult||!NearGarage||Trial.vehicle.SpeedKph>=5||!Trial.vehicle.playerInput)return false;try{Trial.Store.Refresh();ApplyInstalled();}catch(Exception e){Message="Saved progress could not be refreshed.";Debug.LogWarning(e.Message);return false;}Trial.vehicle.GetComponent<FinchDrivingHUD>()?.SetInspection(false);Trial.ExternalModalOpen=true;Trial.vehicle.controlsLocked=true;IsOpen=true;requests.Clear();SelectedIndex=0;Message=null;return true;}
 public void Close(){if(!IsOpen)return;IsOpen=false;if(Trial!=null){Trial.ExternalModalOpen=false;if(Trial.vehicle!=null)Trial.vehicle.controlsLocked=false;}Message=null;}
 public void Select(int index){SelectedIndex=Mathf.Clamp(index,0,3);Message=null;}
 public bool Purchase()=>BuyPart(Catalog.offer);
 public bool BuySelected()=>BuyPart(Offers[SelectedIndex]);
 public bool BuyPart(EngineTuneOffer offer){if(!IsOpen||Trial.Store==null)return false;try{if(!requests.TryGetValue(offer.PartId,out var id)){id=Guid.NewGuid().ToString("N");requests[offer.PartId]=id;}Trial.Store.PurchasePart(id,offer,offer.price,out bool duplicate);ApplyInstalled();Message=duplicate?"Already saved and installed.":offer.displayName+" installed. Payment and parts saved.";return true;}catch(Exception e){ApplyInstalled();Message=e is InvalidOperationException?e.Message:"Could not save. No new part was installed; retry.";Debug.LogWarning("Garage purchase: "+e.Message);return false;}}
 public void ApplyInstalled(){if(Trial.Store==null)return;var p=Trial.Store.Profile;string signature=p.engineStage+"/"+p.gearboxStage+"/"+p.intercoolerStage;if(signature==appliedSignature)return;Trial.vehicle.InstallPowertrain(Catalog.DefinitionFor(p),p.engineStage);AppliedStage=p.engineStage;appliedSignature=signature;}
 void Update(){if(Catalog==null||Trial==null)return;if(FinchInput.Shortcut(KeyCode.G)){if(IsOpen)Close();else Open();}else if(IsOpen&&FinchInput.Shortcut(KeyCode.Escape))Close();else if(IsOpen&&FinchInput.Shortcut(KeyCode.DownArrow))Select((SelectedIndex+1)%4);else if(IsOpen&&FinchInput.Shortcut(KeyCode.UpArrow))Select((SelectedIndex+3)%4);else if(IsOpen&&FinchInput.Shortcut(KeyCode.Return))BuySelected();}
 void LateUpdate(){if(sign!=null){sign.gameObject.SetActive(!IsOpen);if(Camera.main!=null)sign.transform.rotation=Camera.main.transform.rotation;}}
 void OnGUI(){
  if(Catalog==null||Trial==null||Trial.Store==null)return;var p=Trial.Store.Profile;
  if(!IsOpen){if(NearGarage&&!Trial.Active&&!Trial.ShowingResult&&Trial.vehicle.playerInput)GUI.Box(new Rect(16,128,690,26),Trial.vehicle.SpeedKph<5?"G — PLAYER GARAGE  |  Engine tunes, gearbox, turbo kits, intercoolers":"Slow down to enter the player garage");return;}
  float x=(Screen.width-1000)/2f,y=(Screen.height-520)/2f;Color color=GUI.color;GUI.color=new Color(.035f,.05f,.07f,.98f);GUI.DrawTexture(new Rect(x,y,1000,520),Texture2D.whiteTexture);GUI.color=color;GUI.Box(new Rect(x,y,1000,520),"PLAYER GARAGE / ALL PARTS & TUNING");
  var heading=new GUIStyle(GUI.skin.label){fontSize=19,fontStyle=FontStyle.Bold};heading.normal.textColor=Color.white;var wrapped=new GUIStyle(GUI.skin.label){wordWrap=true};wrapped.normal.textColor=Color.white;
  GUI.Label(new Rect(x+24,y+34,710,30),"ASTER FINCH  |  "+(p.engineStage==2?"TURBO STAGE 2":p.engineStage==1?"STAGE 1 NA":"STOCK")+"  |  Gearbox "+p.gearboxStage+" / Cooler "+(p.engineStage<2?"n/a":p.intercoolerStage>0?"street":"basic"),heading);GUI.Label(new Rect(x+790,y+39,190,28),"Available: "+p.availableCredits+" cr");
  var offers=Offers;for(int i=0;i<offers.Length;i++){var item=offers[i];string status=Catalog.Prerequisite(p,item)??"Available";Color old=GUI.backgroundColor;if(i==SelectedIndex)GUI.backgroundColor=new Color(.25f,.65f,.65f);if(GUI.Button(new Rect(x+20,y+84+i*76,300,68),item.displayName+"\n"+item.price+" cr  |  "+status))Select(i);GUI.backgroundColor=old;}
  GUI.Label(new Rect(x+20,y+402,300,24),"Up / Down select  |  Enter installs");
  var o=offers[SelectedIndex];bool installed=Catalog.Installed(p,o)>=o.stage;string requirement=Catalog.Prerequisite(p,o);GUI.Label(new Rect(x+345,y+84,630,30),o.displayName,heading);
  string description=SelectedIndex==0?"Naturally aspirated ECU calibration and breathing package.":SelectedIndex==1?"Supporting gearbox/clutch package. Actual shift interruption: 0.450 → 0.396 seconds.":SelectedIndex==2?"Turbo hardware, basic intercooler, fueling and paid Stage 2 calibration. Boost builds with engine load and RPM.":"Street intercooler and matched calibration reduce typical warm-charge losses. Requires the turbo kit; it already includes a basic intercooler.";
  GUI.Label(new Rect(x+345,y+122,630,56),description,wrapped);
  FinchDefinition a=SelectedIndex==0?Catalog.stock:SelectedIndex==2?Catalog.stageOne:SelectedIndex==3?Catalog.turboBasic:Catalog.DefinitionFor(p),b=SelectedIndex==0?Catalog.stageOne:SelectedIndex==2?Catalog.turboBasic:SelectedIndex==3?Catalog.turboStreet:a;
  GUI.Label(new Rect(x+345,y+180,630,24),"Reference peak crank power: "+a.PeakKw().ToString("F1")+" → "+b.PeakKw().ToString("F1")+" kW");
  var rect=new Rect(x+370,y+222,590,124);GUI.Box(rect,"");Plot(rect,a,false,new Color(.65f,.68f,.72f));Plot(rect,b,false,new Color(.15f,.95f,.8f));if(b.IsTurbo)Plot(rect,b,true,new Color(.95f,.65f,.25f));GUI.Label(new Rect(rect.x-30,rect.y,30,20),"180");GUI.Label(new Rect(rect.x-30,rect.y+53,30,20),"90");GUI.Label(new Rect(rect.x-30,rect.yMax-20,30,20),"0");GUI.Label(new Rect(rect.x,rect.yMax+2,130,24),"900 RPM");GUI.Label(new Rect(rect.xMax-75,rect.yMax+2,100,24),"6500 RPM");GUI.Label(new Rect(rect.x+145,rect.yMax+2,340,24),b.IsTurbo?"Nm: teal steady / orange off-boost":"Nm: gray before / teal after");
  string note=Message??(installed?"Installed and saved.":requirement??(p.availableCredits<o.price?"Need "+(o.price-p.availableCredits)+" more credits.":"After purchase: "+(p.availableCredits-o.price)+" cr remaining."));GUI.Label(new Rect(x+345,y+390,630,45),note,wrapped);
  bool enabled=GUI.enabled;GUI.enabled=!installed&&requirement==null&&p.availableCredits>=o.price;if(GUI.Button(new Rect(x+345,y+454,380,42),installed?"INSTALLED":"Enter — Install for "+o.price+" cr"))BuySelected();GUI.enabled=enabled;if(GUI.Button(new Rect(x+745,y+454,230,42),"Esc / G — Return to driving"))Close();
 }
 static void Plot(Rect r,FinchDefinition d,bool off,Color color){var values=off?d.offBoostTorque:d.torque;Color previous=GUI.color;var matrix=GUI.matrix;GUI.color=color;for(int i=1;i<d.rpm.Length;i++){Vector2 a=new Vector2(r.x+10+(d.rpm[i-1]-900)/5600*(r.width-20),r.yMax-8-values[i-1]/180*(r.height-16)),b=new Vector2(r.x+10+(d.rpm[i]-900)/5600*(r.width-20),r.yMax-8-values[i]/180*(r.height-16));GUIUtility.RotateAroundPivot(Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg,a);GUI.DrawTexture(new Rect(a.x,a.y,Vector2.Distance(a,b),2),Texture2D.whiteTexture);GUI.matrix=matrix;}GUI.color=previous;}
 void OnDestroy(){if(IsOpen)Close();}
}
}
