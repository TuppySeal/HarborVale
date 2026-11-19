using System;
using UnityEngine;
namespace HarborValeGreybox.Driving {
public static class FinchDrivingBootstrap {
 [Serializable] public class Anchor{public float x,y,z,yaw;}
 [Serializable] public class Recovery{public Anchor[] anchors;}
 static Recovery recovery;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Clear(){recovery=null;}
 public static void StartDriving(){
  string scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;if(scene!="HarborVale_CityMap"&&scene!="HarborVale_SportsTest"&&scene!="Finch_HandlingTest")return;
  var specification=UnityEngine.Object.FindAnyObjectByType<StarterCarSpecification>();if(specification==null)return;
  var car=specification.GetComponent<FinchVehicle>();if(car==null)car=specification.gameObject.AddComponent<FinchVehicle>();car.Initialize();
  // The city starts on a clear nearby carriageway; test scenes preserve their authored spawn.
  if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_CityMap")car.Recover();
  var cam=Camera.main;if(cam==null)return;var map=cam.GetComponent<MapExplorer>();if(map!=null)map.enabled=false;
  var chase=cam.GetComponent<FinchChaseCamera>();if(chase==null)chase=cam.gameObject.AddComponent<FinchChaseCamera>();chase.target=car;chase.Snap();cam.orthographic=false;cam.nearClipPlane=.08f;
  var hud=car.GetComponent<FinchDrivingHUD>();if(hud==null)hud=car.gameObject.AddComponent<FinchDrivingHUD>();hud.vehicle=car;hud.chase=chase;hud.map=map;
  SetMapLabels(false);
 }
 public static void SetMapLabels(bool visible){foreach(var label in UnityEngine.Object.FindObjectsByType<MapLabel>(FindObjectsSortMode.None))label.GetComponent<Renderer>().enabled=visible;foreach(var annotation in UnityEngine.Object.FindObjectsByType<MapAnnotation>(FindObjectsSortMode.None))annotation.enabled=visible;}
 public static bool TryRecovery(Vector3 current,Vector3 heading,out Vector3 position,out Quaternion rotation){
  if(recovery==null){var asset=Resources.Load<TextAsset>("FinchRoadRecovery");if(asset!=null)recovery=JsonUtility.FromJson<Recovery>(asset.text);}
  position=default;rotation=Quaternion.identity;if(recovery==null||recovery.anchors.Length==0)return false;
  Anchor nearest=null;float score=float.MaxValue;foreach(var a in recovery.anchors){float d=(new Vector2(a.x-current.x,a.z-current.z)).sqrMagnitude;if(d<score){score=d;nearest=a;}}
  if(nearest==null)return false;position=new Vector3(nearest.x,nearest.y+.40f,nearest.z);rotation=Quaternion.Euler(0,nearest.yaw,0);if(Vector3.Dot(rotation*Vector3.forward,heading)<0)rotation*=Quaternion.Euler(0,180,0);return true;
 }
}
}

