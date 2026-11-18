using UnityEngine;

// Inspection controls only; no driving or career systems.
namespace HarborValeGreybox
{
public class MapExplorer : MonoBehaviour
{

#if ENABLE_INPUT_SYSTEM
    static UnityEngine.InputSystem.Key ConvertKey(KeyCode key) {
        if(key>=KeyCode.Alpha1 && key<=KeyCode.Alpha4)return (UnityEngine.InputSystem.Key)((int)UnityEngine.InputSystem.Key.Digit1+(key-KeyCode.Alpha1));
        switch(key) {
            case KeyCode.W:return UnityEngine.InputSystem.Key.W;case KeyCode.A:return UnityEngine.InputSystem.Key.A;
            case KeyCode.S:return UnityEngine.InputSystem.Key.S;case KeyCode.D:return UnityEngine.InputSystem.Key.D;
            case KeyCode.Q:return UnityEngine.InputSystem.Key.Q;case KeyCode.E:return UnityEngine.InputSystem.Key.E;
            case KeyCode.F:return UnityEngine.InputSystem.Key.F;case KeyCode.L:return UnityEngine.InputSystem.Key.L;
            case KeyCode.LeftArrow:return UnityEngine.InputSystem.Key.LeftArrow;case KeyCode.RightArrow:return UnityEngine.InputSystem.Key.RightArrow;
            case KeyCode.UpArrow:return UnityEngine.InputSystem.Key.UpArrow;case KeyCode.DownArrow:return UnityEngine.InputSystem.Key.DownArrow;
            default:return UnityEngine.InputSystem.Key.LeftShift;
        }
    }
    static bool KeyHeld(KeyCode k)=>UnityEngine.InputSystem.Keyboard.current!=null&&UnityEngine.InputSystem.Keyboard.current[ConvertKey(k)].isPressed;
    static bool KeyDown(KeyCode k)=>UnityEngine.InputSystem.Keyboard.current!=null&&UnityEngine.InputSystem.Keyboard.current[ConvertKey(k)].wasPressedThisFrame;
    static bool RightHeld=>UnityEngine.InputSystem.Mouse.current!=null&&UnityEngine.InputSystem.Mouse.current.rightButton.isPressed;
    static bool RightDown=>UnityEngine.InputSystem.Mouse.current!=null&&UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame;
    static Vector3 MousePosition=>UnityEngine.InputSystem.Mouse.current==null?Vector3.zero:(Vector3)UnityEngine.InputSystem.Mouse.current.position.ReadValue();
    static float Scroll=>UnityEngine.InputSystem.Mouse.current==null?0:UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y/120f;
#else
    static bool KeyHeld(KeyCode k)=>Input.GetKey(k);static bool KeyDown(KeyCode k)=>Input.GetKeyDown(k);
    static bool RightHeld=>Input.GetMouseButton(1);static bool RightDown=>Input.GetMouseButtonDown(1);
    static Vector3 MousePosition=>Input.mousePosition;static float Scroll=>Input.mouseScrollDelta.y;
#endif

    Camera view;
    bool labels = true;
    bool overview = true;
    float yaw, pitch = 35;
    Vector3 lastMouse;
    float Horizontal => (KeyHeld(KeyCode.D)||KeyHeld(KeyCode.RightArrow)?1:0)-(KeyHeld(KeyCode.A)||KeyHeld(KeyCode.LeftArrow)?1:0);
    float Vertical => (KeyHeld(KeyCode.W)||KeyHeld(KeyCode.UpArrow)?1:0)-(KeyHeld(KeyCode.S)||KeyHeld(KeyCode.DownArrow)?1:0);
    void Start() { view = GetComponent<Camera>(); Overview(Vector3.zero, 820); }
    void Overview(Vector3 center, float size)
    {
        overview = true; view.orthographic = true; view.orthographicSize = size;
        transform.SetPositionAndRotation(center + Vector3.up * 1800, Quaternion.Euler(90, 0, 0));
    }
    void Update()
    {
        if (KeyDown(KeyCode.Alpha1)) Overview(Vector3.zero, 820);
        if (KeyDown(KeyCode.Alpha2)) Overview(new Vector3(-430, 0, -370), 340);
        if (KeyDown(KeyCode.Alpha3)) Overview(new Vector3(430, 0, -280), 380);
        if (KeyDown(KeyCode.Alpha4)) Overview(new Vector3(-50, 0, 450), 390);
        if (KeyDown(KeyCode.F)) {
            overview = false; view.orthographic = false;
            transform.SetPositionAndRotation(new Vector3(0, 700, -900), Quaternion.Euler(35, 0, 0)); yaw = 0; pitch = 35;
        }
        if (KeyDown(KeyCode.L)) {
            labels = !labels;
            foreach (var label in FindObjectsByType<MapLabel>(FindObjectsInactive.Include, FindObjectsSortMode.None)) label.GetComponent<MeshRenderer>().enabled = labels;
            foreach (var annotation in FindObjectsByType<MapAnnotation>(FindObjectsInactive.Include, FindObjectsSortMode.None)) annotation.enabled=labels;
        }
        float speed = (KeyHeld(KeyCode.LeftShift) ? 450 : 150) * Time.unscaledDeltaTime;
        if (overview) {
            Vector3 d = new Vector3(Horizontal, 0, Vertical);
            transform.position += d * speed;
            view.orthographicSize = Mathf.Clamp(view.orthographicSize - Scroll * 40, 65, 1200);
        } else {
            if (RightHeld) {
                Vector3 delta=MousePosition-lastMouse;
                if(!RightDown){yaw += delta.x*.15f; pitch = Mathf.Clamp(pitch-delta.y*.15f,-85,85);}
                transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            }
            float lift = (KeyHeld(KeyCode.E) ? 1 : 0) - (KeyHeld(KeyCode.Q) ? 1 : 0);
            transform.position += (transform.forward * Vertical + transform.right * Horizontal + Vector3.up * lift) * speed;
        }
        lastMouse=MousePosition;
    }
    void OnGUI()
    {
        GUI.Box(new Rect(14, 14, 530, 114), "HARBOR VALE / UNITY GREYBOX");
        GUI.Label(new Rect(28, 42, 510, 90), "1 City  |  2 Dockside  |  3 Midtown  |  4 Ridgeway\nF Fly mode: hold RMB to look, WASD move, Q/E height, Shift faster\nOverview: WASD pan, wheel zoom. L labels on/off\nTeal services / amber fuel / blue police / purple events / gray blocks");
    }
}

}
