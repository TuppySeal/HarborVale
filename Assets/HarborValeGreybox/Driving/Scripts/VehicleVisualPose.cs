using UnityEngine;
namespace HarborValeGreybox.Driving {
public static class VehicleVisualPose {
 public static Vector3 LocalPosition(Rigidbody body,Vector3 physical)=>Quaternion.Inverse(body.rotation)*(physical-body.position);
 public static Quaternion LocalRotation(Rigidbody body,Quaternion physical)=>Quaternion.Inverse(body.rotation)*physical;
}
}
