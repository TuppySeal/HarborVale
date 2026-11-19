using System.Collections;
using UnityEngine;
namespace HarborValeGreybox.Racing {
public sealed class PhysicalGarage:MonoBehaviour {
 public static PhysicalGarage Instance;public Transform Building{get;private set;}public bool Ready{get;private set;}
 public static void Hook(){if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_CityMap")new GameObject("D01 | physical player garage").AddComponent<PhysicalGarage>();}
 IEnumerator Start(){Instance=this;var site=GameObject.Find("D01 | PLAYER GARAGE / DIY");if(site==null)yield break;foreach(Transform child in site.transform)if(!child.name.Contains("forecourt")&&!child.name.Contains("foundation"))child.gameObject.SetActive(false);var g=Instantiate(Resources.Load<GameObject>("PhysicalGarage"),transform,false);Building=g.transform;Building.SetPositionAndRotation(site.transform.position,site.transform.rotation);var label=new GameObject("Motor Works exterior sign");label.transform.SetParent(Building,false);label.transform.localPosition=new Vector3(0,4.16f,-6.14f);label.transform.localRotation=Quaternion.identity;var text=label.AddComponent<TextMesh>();text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=56;text.characterSize=.07f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.text="HARBOR VALE  /  MOTOR WORKS";text.color=Color.white;label.GetComponent<Renderer>().sharedMaterial=text.font.material;for(int i=0;i<3;i++){var light=new GameObject("Bay task light "+(i+1));light.transform.SetParent(Building,false);light.transform.localPosition=new Vector3((i-1)*5.8f,3.9f,-1);var lamp=light.AddComponent<Light>();lamp.type=LightType.Point;lamp.color=new Color(.86f,.94f,1);lamp.intensity=2;lamp.range=7;lamp.shadows=LightShadows.None;}Ready=true;}
 public bool InBay(Vector3 position){if(Building==null)return false;var p=Building.InverseTransformPoint(position);return Mathf.Abs(p.x)<8.5f&&p.z> -7&&p.z<5.5f&&p.y> -1&&p.y<3;}
 void OnDestroy(){if(Instance==this)Instance=null;}
}
}
