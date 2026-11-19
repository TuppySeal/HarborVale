using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Presentation {
public sealed class CityPresentation:MonoBehaviour {
 public static bool DisabledForComparison;
 public int EligibleStaticRenderers{get;private set;}public int BatchedRenderers{get;private set;}public int DetailRenderers{get;private set;}public bool Ready{get;private set;}
 Material sky;Camera view;float[] detailCulling;bool mapMode;public int ReadableCubeConversions{get;private set;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){DisabledForComparison=Environment.GetCommandLineArgs().Contains("--presentation-off");}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Register(){SceneManager.sceneLoaded-=Loaded;SceneManager.sceneLoaded+=Loaded;}
 static void Loaded(Scene scene,LoadSceneMode mode){if(!DisabledForComparison&&(scene.name=="HarborVale_CityMap"||scene.name=="HarborVale_SportsTest"))new GameObject("Harbor Vale | runtime render presentation").AddComponent<CityPresentation>();}
 IEnumerator Start(){
  QualitySettings.antiAliasing=4;QualitySettings.anisotropicFiltering=AnisotropicFiltering.ForceEnable;QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowResolution=ShadowResolution.High;QualitySettings.shadowCascades=4;QualitySettings.shadowDistance=140;QualitySettings.shadowCascade4Split=new Vector3(.12f,.32f,.62f);QualitySettings.softParticles=true;
  RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.43f,.50f,.60f);RenderSettings.ambientEquatorColor=new Color(.27f,.30f,.34f);RenderSettings.ambientGroundColor=new Color(.12f,.13f,.15f);RenderSettings.ambientIntensity=1;
  RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=600;RenderSettings.fogEndDistance=2300;RenderSettings.fogColor=new Color(.52f,.62f,.70f);
  foreach(var light in FindObjectsByType<Light>(FindObjectsSortMode.None))if(light.type==LightType.Directional){light.color=new Color(1,.96f,.90f);light.intensity=1.05f;light.shadows=LightShadows.Soft;light.shadowBias=.08f;light.shadowNormalBias=.6f;light.shadowNearPlane=1;}
  if(RenderSettings.skybox!=null){sky=new Material(RenderSettings.skybox);RenderSettings.skybox=sky;if(sky.HasProperty("_Exposure"))sky.SetFloat("_Exposure",1);}
  view=Camera.main;if(view!=null){view.allowMSAA=true;view.allowHDR=true;view.renderingPath=RenderingPath.Forward;view.nearClipPlane=.12f;view.farClipPlane=3000;if(view.GetComponent<SoftTonemap>()==null)view.gameObject.AddComponent<SoftTonemap>();detailCulling=new float[32];detailCulling[28]=750;detailCulling[29]=240;view.layerCullDistances=detailCulling;view.layerCullSpherical=true;}
  yield return null;yield return null;
  foreach(var text in FindObjectsByType<TextMesh>(FindObjectsSortMode.None))if(text.name=="Player garage sign"||text.name=="First Lap start label")text.gameObject.AddComponent<DrivingBillboard>();
  var root=SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(g=>g.name.StartsWith("HARBOR VALE | scaled"));
  if(root!=null){var readable=Resources.Load<Mesh>("CityUnitCube");foreach(var filter in root.GetComponentsInChildren<MeshFilter>())if(filter.sharedMesh!=null&&!filter.sharedMesh.isReadable&&filter.sharedMesh.name=="Cube"&&readable!=null&&filter.GetComponentInParent<StarterCarSpecification>()==null&&filter.GetComponentInParent<FinchVehicle>()==null){filter.sharedMesh=readable;ReadableCubeConversions++;}var renderers=root.GetComponentsInChildren<MeshRenderer>().Where(r=>r.GetComponent<MeshFilter>()?.sharedMesh!=null&&r.GetComponent<MeshFilter>().sharedMesh.isReadable&&r.GetComponent<TextMesh>()==null&&r.GetComponentInParent<StarterCarSpecification>()==null&&r.GetComponentInParent<FinchVehicle>()==null).ToArray();EligibleStaticRenderers=renderers.Length;
   foreach(var r in renderers){if(r.name.StartsWith("Landform |")||r.name=="Carriageway"||r.name.StartsWith("Sidewalk ")||r.name.StartsWith("Curb ")||r.name.StartsWith("Shoulder ")||r.name.StartsWith("Edge line ")||r.name=="Lane markings")r.shadowCastingMode=ShadowCastingMode.Off;if(r.name.StartsWith("Floor glazing")){r.gameObject.layer=28;DetailRenderers++;}else if(r.name.StartsWith("Door |")||r.name=="Workshop roller door"||r.name=="Sign board"){r.gameObject.layer=29;DetailRenderers++;}}
   StaticBatchingUtility.Combine(renderers.Select(r=>r.gameObject).ToArray(),root);BatchedRenderers=renderers.Count(r=>r.isPartOfStaticBatch);
  }
  Ready=true;
 }
 void Update(){if(view==null)return;bool inspect=view.GetComponent<MapExplorer>()?.enabled==true;if(inspect==mapMode)return;mapMode=inspect;view.farClipPlane=inspect?9000:3000;RenderSettings.fog=!inspect;view.layerCullDistances=inspect?new float[32]:detailCulling;}
 void OnDestroy(){if(sky!=null)Destroy(sky);}
}
}
