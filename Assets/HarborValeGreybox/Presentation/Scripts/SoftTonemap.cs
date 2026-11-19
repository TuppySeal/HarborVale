using UnityEngine;
namespace HarborValeGreybox.Presentation {
[RequireComponent(typeof(Camera))]
public sealed class SoftTonemap:MonoBehaviour {
 Material material;
 public bool Ready=>material!=null;
 void OnEnable(){var shader=Resources.Load<Shader>("HarborValeTonemap");if(shader!=null&&shader.isSupported)material=new Material(shader){hideFlags=HideFlags.HideAndDontSave};}
 void OnRenderImage(RenderTexture source,RenderTexture destination){if(material==null){Graphics.Blit(source,destination);return;}material.SetFloat("_Exposure",1);Graphics.Blit(source,destination,material);}
 void OnDisable(){if(material!=null)Destroy(material);material=null;}
}
}
