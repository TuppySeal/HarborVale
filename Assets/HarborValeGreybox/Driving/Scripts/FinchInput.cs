using UnityEngine;
namespace HarborValeGreybox.Driving {
public static class FinchInput {
 public static bool Held(KeyCode k){
  if(!Application.isFocused)return false;
#if ENABLE_INPUT_SYSTEM
  var keyboard=UnityEngine.InputSystem.Keyboard.current;if(keyboard==null)return false;return keyboard[Convert(k)].isPressed;
#else
  return Input.GetKey(k);
#endif
 }
 public static bool Down(KeyCode k){
  if(!Application.isFocused)return false;
#if ENABLE_INPUT_SYSTEM
  var keyboard=UnityEngine.InputSystem.Keyboard.current;if(keyboard==null)return false;return keyboard[Convert(k)].wasPressedThisFrame;
#else
  return Input.GetKeyDown(k);
#endif
 }
#if ENABLE_INPUT_SYSTEM
 static UnityEngine.InputSystem.Key Convert(KeyCode k){switch(k){
  case KeyCode.G:return UnityEngine.InputSystem.Key.G;case KeyCode.Return:return UnityEngine.InputSystem.Key.Enter;case KeyCode.E:return UnityEngine.InputSystem.Key.E;case KeyCode.W:return UnityEngine.InputSystem.Key.W;case KeyCode.S:return UnityEngine.InputSystem.Key.S;
  case KeyCode.A:return UnityEngine.InputSystem.Key.A;case KeyCode.D:return UnityEngine.InputSystem.Key.D;
  case KeyCode.UpArrow:return UnityEngine.InputSystem.Key.UpArrow;case KeyCode.DownArrow:return UnityEngine.InputSystem.Key.DownArrow;
  case KeyCode.LeftArrow:return UnityEngine.InputSystem.Key.LeftArrow;case KeyCode.RightArrow:return UnityEngine.InputSystem.Key.RightArrow;
  case KeyCode.Space:return UnityEngine.InputSystem.Key.Space;case KeyCode.R:return UnityEngine.InputSystem.Key.R;
  case KeyCode.Tab:return UnityEngine.InputSystem.Key.Tab;case KeyCode.C:return UnityEngine.InputSystem.Key.C;
  case KeyCode.LeftAlt:return UnityEngine.InputSystem.Key.LeftAlt;case KeyCode.RightAlt:return UnityEngine.InputSystem.Key.RightAlt;
  case KeyCode.LeftControl:return UnityEngine.InputSystem.Key.LeftCtrl;case KeyCode.RightControl:return UnityEngine.InputSystem.Key.RightCtrl;
  default:return UnityEngine.InputSystem.Key.Escape;}}
#endif
 public static bool Shortcut(KeyCode k)=>Down(k)&&!Held(KeyCode.LeftAlt)&&!Held(KeyCode.RightAlt)&&!Held(KeyCode.LeftControl)&&!Held(KeyCode.RightControl);
 public static float Pedal=>(Held(KeyCode.W)||Held(KeyCode.UpArrow)?1:0)-(Held(KeyCode.S)||Held(KeyCode.DownArrow)?1:0);
 public static float Steer=>(Held(KeyCode.D)||Held(KeyCode.RightArrow)?1:0)-(Held(KeyCode.A)||Held(KeyCode.LeftArrow)?1:0);
}
}



