using UnityEngine;
namespace ClaudeGaragePreview {
public class CameraOrbitController:MonoBehaviour {
 public Transform target;public float distance=10,yaw=32,pitch=22;public float minDistance=2,maxDistance=14;
 public void Orbit(float horizontal,float vertical){yaw+=horizontal;pitch=Mathf.Clamp(pitch+vertical,8,65);Apply();}
 public void Zoom(float amount){distance=Mathf.Clamp(distance+amount,minDistance,maxDistance);Apply();}
 public void ResetView(){distance=10;yaw=32;pitch=22;Apply();}
 public void Apply(){if(target==null)return;transform.position=target.position+Quaternion.Euler(-pitch,yaw,0)*Vector3.forward*distance;transform.LookAt(target.position);}
 void LateUpdate(){if(Input.GetMouseButton(1))Orbit(Input.GetAxis("Mouse X")*3,-Input.GetAxis("Mouse Y")*3);Zoom(-Input.mouseScrollDelta.y*.7f);if(Input.GetKeyDown(KeyCode.R))ResetView();Apply();}
}
}
