using UnityEngine;
namespace ClaudeGaragePreview {
public class UpgradeUIManager:MonoBehaviour {
 public GaragePreviewManager manager;
 void OnGUI(){if(manager==null)return;GUI.Box(new Rect(16,16,Screen.width-32,68),"HARBOR VALE | WORKSHOP PREVIEW");GUI.Label(new Rect(30,44,Screen.width-60,27),"Static Aster Finch · one parts counter · preview states only · no money, saves or vehicle physics");
  if(!manager.MenuOpen){if(GUI.Button(new Rect(16,Screen.height-62,240,42),"G — Open parts counter"))manager.MenuOpen=true;return;}
  float y=Screen.height-177;GUI.Box(new Rect(16,y,Screen.width-32,161),"");var p=GaragePreviewManager.Parts[manager.SelectedIndex];GUI.Label(new Rect(30,y+9,850,27),p.displayName+" | "+(manager.fitted[manager.SelectedIndex]?"PREVIEW FITTED":"DISPLAY ONLY"));
  float w=(Screen.width-340)/4f;for(int i=0;i<4;i++){var old=GUI.backgroundColor;GUI.backgroundColor=i==manager.SelectedIndex?new Color(.2f,.85f,.8f):Color.white;if(GUI.Button(new Rect(30+i*w,y+41,w-8,36),GaragePreviewManager.Parts[i].displayName))manager.Select(i);GUI.backgroundColor=old;}
  if(GUI.Button(new Rect(Screen.width-300,y+41,270,36),manager.fitted[manager.SelectedIndex]?"Remove preview fitment":"Preview fitment (mock)"))manager.ToggleFitment();
  GUI.Label(new Rect(30,y+88,Screen.width-60,38),p.description,new GUIStyle(GUI.skin.label){wordWrap=true});GUI.Label(new Rect(30,y+128,Screen.width-60,26),"Right mouse drag — orbit  |  Wheel — zoom  |  R — reset view  |  G — close/open counter");
 }
}
}
