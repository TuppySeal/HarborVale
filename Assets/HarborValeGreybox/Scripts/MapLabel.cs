using UnityEngine;
namespace HarborValeGreybox
{
[ExecuteAlways]
public class MapLabel : MonoBehaviour
{
    public string fullCaption;
    public string shortCaption;
    public void Refresh(Camera camera) {
        transform.rotation=camera.transform.rotation; transform.localScale=Vector3.one*(camera.orthographic?Mathf.Max(1,camera.orthographicSize/300f):1);
        var text=GetComponent<TextMesh>();
        if(text!=null&&!string.IsNullOrEmpty(fullCaption))text.text=camera.orthographic&&camera.orthographicSize>500?shortCaption:fullCaption;
    }
    void LateUpdate() { if (Camera.main != null) Refresh(Camera.main); }
}

}

