using UnityEngine;

namespace HarborValeGreybox
{
public class MapAnnotation : MonoBehaviour
{
    public string caption;
    public string shortCode;
    public Color tint = Color.white;
    public bool detailOnly;
    void OnGUI()
    {
        var cam=Camera.main; if(cam==null)return;
        if(detailOnly && cam.orthographic && cam.orthographicSize>450)return;
        var p=cam.WorldToScreenPoint(transform.position);
        if(p.z<=0||p.x<0||p.x>Screen.width||p.y<0||p.y>Screen.height)return;
        string text=cam.orthographic&&cam.orthographicSize>500?shortCode:caption;
        var style=new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter,fontSize=12,fontStyle=FontStyle.Bold};
        style.normal.textColor=tint;
        var measured=style.CalcSize(new GUIContent(text));
        var rect=new Rect(p.x-measured.x/2-5,Screen.height-p.y-measured.y/2-3,measured.x+10,measured.y+6);
        Color old=GUI.color;GUI.color=new Color(.035f,.055f,.075f,.90f);GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=old;
        GUI.Label(rect,text,style);
    }
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        var style=new GUIStyle(UnityEditor.EditorStyles.boldLabel);style.normal.textColor=tint;
        UnityEditor.Handles.Label(transform.position,caption,style);
#endif
    }
}

}
