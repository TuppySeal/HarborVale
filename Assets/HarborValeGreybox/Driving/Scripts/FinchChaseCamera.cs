using UnityEngine;
namespace HarborValeGreybox.Driving {
public class FinchChaseCamera:MonoBehaviour {
 public FinchVehicle target;public bool bonnet;
 Vector3 smoothVelocity;bool snap=true;
 void LateUpdate(){Follow(Time.deltaTime);}
 public void Snap(){snap=true;smoothVelocity=Vector3.zero;}
 public void Follow(float dt){
  if(target==null)return;var car=target.transform;Vector3 focus=car.position+Vector3.up*1.15f;Vector3 desired;
  if(bonnet){desired=car.TransformPoint(new Vector3(0,1.30f,.65f));transform.SetPositionAndRotation(desired,Quaternion.LookRotation(car.forward+Vector3.down*.02f,Vector3.up));GetComponent<Camera>().fieldOfView=68;return;}
  Vector3 heading=Vector3.ProjectOnPlane(car.forward,Vector3.up).normalized;
  desired=focus-heading*(6.0f+Mathf.Clamp(target.SpeedKph/60,0,2))+Vector3.up*2.1f;
  Vector3 ray=desired-focus;float distance=ray.magnitude;
  // Ignore the followed car's colliders when checking scenery occlusion.
  foreach(var hit in Physics.SphereCastAll(focus,.22f,ray.normalized,distance,~0,QueryTriggerInteraction.Ignore))if(!hit.collider.transform.IsChildOf(car))distance=Mathf.Min(distance,Mathf.Max(1,hit.distance-.15f));
  desired=focus+ray.normalized*distance;
  if(snap||dt<=0||Vector3.Distance(transform.position,desired)>30){transform.position=desired;snap=false;}else transform.position=Vector3.SmoothDamp(transform.position,desired,ref smoothVelocity,.12f,200,dt);
  transform.rotation=Quaternion.LookRotation(focus+heading*2-transform.position,Vector3.up);var cam=GetComponent<Camera>();cam.orthographic=false;cam.fieldOfView=Mathf.Lerp(cam.fieldOfView,62+Mathf.Clamp(target.SpeedKph/15,0,9),Mathf.Clamp01(dt*3));
 }
}
}

