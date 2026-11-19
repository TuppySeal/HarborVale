using UnityEngine;
namespace HarborValeGreybox.Driving {
public class FinchChaseCamera:MonoBehaviour {
 public FinchVehicle target;public bool bonnet;
 public bool externalLookInput,commandedMouseLook;public float commandedLookYaw;public Vector2 commandedMouseDelta;
 public float LookYaw{get;private set;}public float LookPitch{get;private set;}public bool MouseCaptured=>captured;
 Vector3 smoothVelocity;float yawVelocity,pitchVelocity,mouseYaw,mousePitch;bool snap=true,captured,wasMouse;int reset=-1;CursorLockMode previousLock;bool previousVisible;
 void LateUpdate(){Follow(Time.deltaTime);}
 void CaptureMouse(){if(captured||externalLookInput)return;previousLock=Cursor.lockState;previousVisible=Cursor.visible;Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;captured=true;}
 void ReleaseMouse(){if(!captured)return;Cursor.lockState=previousLock;Cursor.visible=previousVisible;captured=false;}
 void OnDisable(){ReleaseMouse();wasMouse=false;mouseYaw=mousePitch=0;}
 void OnApplicationFocus(bool focused){if(!focused)Snap();}
 public void Snap(){snap=true;smoothVelocity=Vector3.zero;yawVelocity=pitchVelocity=0;LookYaw=LookPitch=mouseYaw=mousePitch=0;wasMouse=false;ReleaseMouse();}
 public void UpdateLook(float dt){
  bool allowed=target!=null&&target.playerInput&&!target.controlsLocked&&(externalLookInput||Application.isFocused);bool mouse=allowed&&(externalLookInput?commandedMouseLook:FinchInput.MouseLookHeld);float wanted=0,pitch=0;
  if(mouse){if(!wasMouse){mouseYaw=LookYaw;mousePitch=LookPitch;CaptureMouse();}else{Vector2 delta=externalLookInput?commandedMouseDelta:FinchInput.MouseLookDelta;mouseYaw=Mathf.Clamp(mouseYaw+delta.x,-145,145);mousePitch=Mathf.Clamp(mousePitch-delta.y,-25,30);}wanted=mouseYaw;pitch=mousePitch;}
  else {ReleaseMouse();if(allowed)wanted=externalLookInput?Mathf.Clamp(commandedLookYaw,-180,180):FinchInput.Held(KeyCode.K)?180:(FinchInput.Held(KeyCode.L)?90:0)-(FinchInput.Held(KeyCode.J)?90:0);mouseYaw=mousePitch=0;}
  wasMouse=mouse;float step=Mathf.Max(.0001f,dt);LookYaw=Mathf.SmoothDampAngle(LookYaw,wanted,ref yawVelocity,.13f,900,step);LookPitch=Mathf.SmoothDamp(LookPitch,pitch,ref pitchVelocity,.12f,300,step);
 }
 Vector3 Unoccluded(Vector3 focus,Vector3 candidate){Vector3 ray=candidate-focus;float d=ray.magnitude;if(d<.001f)return candidate;foreach(var hit in Physics.SphereCastAll(focus,.22f,ray.normalized,d,~0,QueryTriggerInteraction.Ignore))if(!hit.collider.transform.IsChildOf(target.transform))d=Mathf.Min(d,Mathf.Max(.35f,hit.distance-.12f));return focus+ray.normalized*d;}
 public void Follow(float dt){
  if(target==null)return;if(reset!=target.ResetSequence){reset=target.ResetSequence;Snap();}UpdateLook(dt);var car=target.transform;var cam=GetComponent<Camera>();cam.orthographic=false;Vector3 heading=Vector3.ProjectOnPlane(car.forward,Vector3.up).normalized;Quaternion look=Quaternion.AngleAxis(LookYaw,Vector3.up);Vector3 direction=look*heading;
  if(bonnet){Vector3 desired=car.TransformPoint(new Vector3(0,Mathf.Lerp(1.30f,1.70f,Mathf.InverseLerp(45,110,Mathf.Abs(Mathf.DeltaAngle(0,LookYaw)))),.65f));Quaternion rotation=Quaternion.LookRotation(direction,Vector3.up)*Quaternion.Euler(LookPitch+1.15f,0,0);transform.SetPositionAndRotation(desired,rotation);cam.fieldOfView=Mathf.Lerp(cam.fieldOfView,68,1-Mathf.Exp(-Mathf.Max(0,dt)*8));snap=false;return;}
  Vector3 focus=car.position+Vector3.up*1.15f;Vector3 orbit=Quaternion.AngleAxis(LookPitch,look*car.right)*(-direction*(6+Mathf.Clamp(target.SpeedKph/60,0,2))+Vector3.up*2.1f);Vector3 desiredPosition=Unoccluded(focus,focus+orbit);
  Vector3 position=snap||dt<=0||Vector3.Distance(transform.position,desiredPosition)>30?desiredPosition:Vector3.SmoothDamp(transform.position,desiredPosition,ref smoothVelocity,.12f,200,dt);transform.position=Unoccluded(focus,position);
  Quaternion desiredRotation=Quaternion.LookRotation(focus+direction*2-transform.position,Vector3.up);transform.rotation=snap||dt<=0?desiredRotation:Quaternion.Slerp(transform.rotation,desiredRotation,1-Mathf.Exp(-dt*16));snap=false;cam.fieldOfView=Mathf.Lerp(cam.fieldOfView,62+Mathf.Clamp(target.SpeedKph/15,0,9),1-Mathf.Exp(-Mathf.Max(0,dt)*3));
 }
}
}

