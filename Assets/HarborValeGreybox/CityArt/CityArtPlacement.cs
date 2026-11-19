using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HarborValeGreybox.Presentation {
public sealed class CityArtPlacement:MonoBehaviour {
 [Serializable] public class Site {public string id;public float w,d,h;}
 [Serializable] public class Layout {public Site[] sites;}
 public int Buildings{get;private set;}public int Props{get;private set;}public bool Ready{get;private set;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Register(){SceneManager.sceneLoaded-=Loaded;SceneManager.sceneLoaded+=Loaded;}
 static void Loaded(Scene scene,LoadSceneMode mode){if(scene.name=="HarborVale_CityMap"||scene.name=="HarborVale_SportsTest")new GameObject("Harbor Vale | reviewed city art").AddComponent<CityArtPlacement>();}
 void Start(){Apply();}
 public void Apply(){if(Ready)return;var data=Resources.Load<TextAsset>("CityArtPlacements");var apartment=Resources.Load<GameObject>("CityArt/Architecture/Prefabs/Building_Apartment_12x14x15");var lamp=Resources.Load<GameObject>("CityArt/Roadside/Prefabs/Streetlamp_7m");var tree=Resources.Load<GameObject>("CityArt/Roadside/Prefabs/PlanterTree");if(data==null||apartment==null||lamp==null||tree==null){Debug.LogError("Reviewed city-art resource missing");Ready=true;return;}
  var layout=JsonUtility.FromJson<Layout>(data.text);var all=SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
  foreach(var site in layout.sites){var plot=all.FirstOrDefault(t=>t.name.StartsWith(site.id+" | ",StringComparison.Ordinal));if(plot==null)continue;
   foreach(var r in plot.GetComponentsInChildren<MeshRenderer>())if(r.name.StartsWith("Building | ",StringComparison.Ordinal)||r.name=="Roof cap"||r.name.StartsWith("Door | ",StringComparison.Ordinal)||r.name.StartsWith("Floor glazing ",StringComparison.Ordinal))r.enabled=false;
   var building=Instantiate(apartment,plot,false);building.name="Reviewed apartment | "+site.id;building.transform.localPosition=Vector3.zero;building.transform.localRotation=Quaternion.identity;building.transform.localScale=new Vector3(site.w/12,site.h/15,site.d/14);foreach(var c in building.GetComponentsInChildren<Collider>()){if(Application.isPlaying)Destroy(c);else DestroyImmediate(c);}Buildings++;
   // All dressing stays inside the original level forecourt, away from its central driveway.
   foreach(float side in new[]{-1f,1f}){var g=Instantiate(lamp,plot,false);g.name="Reviewed forecourt lamp | "+site.id;g.transform.localPosition=new Vector3(side*(site.w/2+1.6f),.08f,-site.d/2-3);Props++;}
   var planter=Instantiate(tree,plot,false);planter.name="Reviewed forecourt planter | "+site.id;planter.transform.localPosition=new Vector3(-site.w/2-1.5f,.08f,0);Props++;
  }
  Ready=true;
 }
}
}
