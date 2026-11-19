using UnityEngine;
namespace HarborValeGreybox.Workshop {
// Original placeholder geometry only; driving, collision and aerodynamic forces stay in FinchVehicle.
public class ChassisVisuals:MonoBehaviour {
 GameObject root,wing;Material material;int stage=-1;
 static void Release(Object item){if(item==null)return;if(Application.isPlaying)Destroy(item);else DestroyImmediate(item);}
 GameObject Box(string name,Vector3 position,Vector3 scale){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,false);g.transform.localPosition=position;g.transform.localScale=scale;var collider=g.GetComponent<Collider>();collider.enabled=false;Release(collider);g.GetComponent<Renderer>().sharedMaterial=material;return g;}
 public void Configure(int next,int angle,bool apex){
  if(stage!=next){if(root!=null){root.SetActive(false);Release(root);}root=null;wing=null;stage=next;
   foreach(var renderer in GetComponentsInChildren<Renderer>())if(renderer.name=="Rear aero wing"||renderer.name=="Wing upright")renderer.enabled=next==0;
   if(next>0){if(material==null){material=new Material(Shader.Find("Standard"));material.color=new Color(.06f,.07f,.08f);material.SetFloat("_Metallic",.25f);}root=new GameObject("Installed aero package | visual only");root.transform.SetParent(transform,false);float width=(apex?1.85f:1.65f)+(next-1)*.10f;float height=next==1?1.20f:next==2?1.38f:1.50f;
    wing=Box("Adjustable aftermarket rear wing",new Vector3(0,height,-1.82f),new Vector3(width,.055f,.24f+.04f*next));for(int side=-1;side<=1;side+=2){Box("Aero wing support",new Vector3(side*.55f,height-.15f,-1.82f),new Vector3(.05f,.28f,.08f));if(next>=2)Box("Aero endplate",new Vector3(side*width*.5f,height+.035f,-1.82f),new Vector3(.025f,.15f,.34f));}Box("Aftermarket front splitter",new Vector3(0,.30f,2.01f),new Vector3(apex?1.93f:1.78f,.035f,.22f+.04f*next));
   }
  }
  if(wing!=null)wing.transform.localRotation=Quaternion.Euler(Mathf.Lerp(0,16,angle/100f),0,0);
 }
 void OnDestroy(){Release(root);Release(material);}
}
}
