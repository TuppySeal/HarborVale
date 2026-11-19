using UnityEngine;
namespace HarborValeGreybox.Workshop {
public sealed partial class ExpandedWorkshopView {
 bool turboAudioMode;public bool EditingTurboAudio=>turboAudioMode;
 public void ShowTurboAudio(){setupMode=false;turboAudioMode=true;clearFocus=true;}
 void DrawTurboAudioPanel(){
  GUI.Box(new Rect(328,124,1088,665),"",card);Label(new Rect(350,148,1025,38),"TURBO AUDIO / Choose your lift-off sound",28,Color.white,true);
  Label(new Rect(350,197,1025,56),"Spool follows boost and engine load. Lift off under boost to hear your selected release. Both choices are free and keep the same performance.",18,muted,false,true);
  for(int i=0;i<2;i++){var mode=(TurboReleaseMode)i;bool active=garage.TurboRelease==mode;float x=350+i*523;GUI.Box(new Rect(x,284,503,335),"",active?selectedCard:card);Label(new Rect(x+20,309,460,38),i==0?"BLOW-OFF VALVE":"FLUTTER / STUTUTU",24,active?accent:Color.white,true);Label(new Rect(x+20,364,458,110),i==0?"A quick pressure-release whoosh with a short, airy tail. One clean pssh when you lift off under boost.":"A pulsing, decaying release with progressively wider gaps. The classic stututu sound after a boosted pull.",19,muted,false,true);
   if(GUI.Button(new Rect(x+20,492,460,44),"▶ Preview sound",button))garage.PreviewTurboRelease(mode);
   if(GUI.Button(new Rect(x+20,551,460,44),active?"✓ Selected":garage.IsTest?"Use for this test":"Select · free",active?disabled:primary)&&!active)garage.ChooseTurboRelease(mode);
  }
  bool turbo=garage.Car.Engine.definition.IsTurbo;Label(new Rect(350,644,1025,54),turbo?"Active with your installed turbo. Release strength follows the boost stored before lifting.":"Preview both sounds now. Your choice becomes audible when a turbo kit is installed; NA and supercharged engines stay free of turbo effects.",18,muted,false,true);
  Label(new Rect(350,718,1025,55),garage.TurboAudioError??garage.Message??"Preview uses simulated boost for audition only. Your actual car stays parked.",17,garage.TurboAudioError==null?muted:amber,false,true);
 }
}
}
