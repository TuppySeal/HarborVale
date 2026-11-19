using UnityEngine;
namespace HarborValeGreybox.Presentation {
public sealed class DrivingBillboard:MonoBehaviour {
 TextMesh text;Renderer mesh;Vector3 scale;Color color;float height;
 public static float ScreenScale(float worldHeight,float distance,float fov,int pixels,float maxHeight=42){if(worldHeight<=0||pixels<=0)return 1;float projected=worldHeight/Mathf.Max(1,distance)*pixels/(2*Mathf.Tan(fov*Mathf.Deg2Rad*.5f));return Mathf.Clamp(maxHeight/Mathf.Max(1,projected),.0001f,1);}
 void Start(){text=GetComponent<TextMesh>();mesh=GetComponent<Renderer>();scale=transform.localScale;color=text.color;height=mesh.bounds.size.y;}
 void LateUpdate(){var camera=Camera.main;if(camera==null||text==null)return;if(camera.GetComponent<MapExplorer>()?.enabled==true){transform.localScale=scale;text.color=color;mesh.enabled=true;return;}float distance=Vector3.Distance(camera.transform.position,transform.position);transform.localScale=scale*ScreenScale(height,distance,camera.fieldOfView,camera.pixelHeight);var position=camera.WorldToViewportPoint(transform.position);float alpha=1-Mathf.InverseLerp(65,130,distance);text.color=new Color(color.r,color.g,color.b,color.a*alpha);mesh.enabled=position.z>0&&alpha>.01f;}
 void OnDestroy(){if(text!=null){text.color=color;transform.localScale=scale;}}
}
}
