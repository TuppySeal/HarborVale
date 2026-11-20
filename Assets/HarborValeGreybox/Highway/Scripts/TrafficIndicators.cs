using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing {
// Original four amber marker lamps, separate from the authored body/lighting materials.
[DisallowMultipleComponent]
public sealed class TrafficIndicators : MonoBehaviour {
 public HighwayDriver Driver; public TrafficPersona Persona; public FinchVehicle Car;
 public bool ShowLabel=true;
 public int UrbanSignalDirection;
 Renderer[] left=new Renderer[2],right=new Renderer[2]; Material amber; Camera view; GUIStyle labelStyle;
 bool built; float cameraClock;
 void Start(){Build();}
 public void Build(){
  if(built)return;if(Car==null)Car=GetComponent<FinchVehicle>();if(Car==null)return;
  if(Driver==null)Driver=GetComponent<HighwayDriver>();if(Persona==null)Persona=GetComponent<TrafficPersona>();
  float width=1.70f,length=4.05f,height=1.46f;
  if(VehicleLineup.IsNew(Car.PowertrainCarId)){var spec=VehicleLineup.Find(Car.PowertrainCarId);width=spec.width;length=spec.length;height=spec.height;}
  else {var shell=Car.GetComponent<BoxCollider>();if(shell!=null){width=Mathf.Max(width,shell.size.x);length=Mathf.Max(length,shell.size.z);}}
  Shader shader=Shader.Find("Unlit/Color");if(shader==null)shader=Shader.Find("Standard");amber=new Material(shader){name="Original amber traffic indicator",color=new Color(1,.42f,.015f)};
  for(int side=0;side<2;side++)for(int end=0;end<2;end++){
   var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=(side==0?"Left":"Right")+(end==0?" front":" rear")+" turn marker";
   g.transform.SetParent(Car.transform,false);g.transform.localPosition=new Vector3((side==0?-1:1)*(width*.5f-.15f),Mathf.Clamp(height*.43f,.5f,.85f),(end==0?1:-1)*(length*.5f+.025f));g.transform.localScale=new Vector3(.20f,.11f,.045f);
   var collider=g.GetComponent<Collider>();collider.enabled=false;Destroy(collider);var renderer=g.GetComponent<Renderer>();renderer.sharedMaterial=amber;renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;renderer.receiveShadows=false;renderer.enabled=false;
   (side==0?left:right)[end]=renderer;
  }
  built=true;view=Camera.main;
 }
 void Update(){if(!built){Build();return;}int signal=Driver!=null?Driver.SignalDirection:UrbanSignalDirection;bool on=(Time.time%1f)<.5f;for(int i=0;i<2;i++){left[i].enabled=signal<0&&on;right[i].enabled=signal>0&&on;}cameraClock-=Time.deltaTime;if(view==null&&cameraClock<=0){view=Camera.main;cameraClock=1;}}
 void OnGUI(){if(!ShowLabel||Persona==null||Car==null||view==null||Driver!=null&&(Driver.IsRacer||Driver.IsPolice))return;Vector3 p=Car.transform.position+Vector3.up*2.1f;if((view.transform.position-p).sqrMagnitude>35*35)return;Vector3 screen=view.WorldToScreenPoint(p);if(screen.z<=0)return;if(labelStyle==null)labelStyle=new GUIStyle(GUI.skin.box){fontSize=11,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(screen.x-61,Screen.height-screen.y-10,122,20),Persona.Label,labelStyle);}
 void OnDestroy(){if(amber!=null)Destroy(amber);}
}
}
