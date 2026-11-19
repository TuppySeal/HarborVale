using UnityEngine;
namespace HarborValeGreybox.Workshop {
public sealed partial class ExpandedWorkshopView {
 bool setupMode;WorkshopSetup setupDraft;public bool EditingSetup=>setupMode;
 public void ShowSetup(){setupMode=true;setupDraft=ChassisTuning.Copy(garage.Build.setup);clearFocus=true;}
 int Knob(float x,float y,string title,int value,int min,int max,bool enabled=true,string suffix="%"){
  Label(new Rect(x,y,490,25),enabled?title+"  "+value+suffix:title+" / upgrade required",17,enabled?Color.white:muted);if(!enabled){GUI.enabled=false;GUI.HorizontalSlider(new Rect(x,y+30,470,20),.5f,0,1);GUI.enabled=true;return value;}float next=GUI.HorizontalSlider(new Rect(x,y+30,470,20),value,min,max);return Mathf.RoundToInt(next);
 }
 void DrawSetupPanel(){var b=garage.Build;var c=garage.Catalog;GUI.Box(new Rect(328,124,1088,665),"",card);Label(new Rect(350,142,1000,32),"CHASSIS SETUP / Adjust while parked",26,Color.white,true);Label(new Rect(350,183,1000,28),"Hardware unlocks adjustment. Applying settings is free; your installed parts stay fitted.",16,muted);
  setupDraft.diffLockPercent=Knob(350,231,"Differential lock",setupDraft.diffLockPercent,0,100,b.differentialStage>0);
  Label(new Rect(350,294,490,27),"SUSPENSION / Stage 2 unlocks adjustment",17,accent,true);
  setupDraft.rideHeightMm=Knob(350,334,"Ride height",setupDraft.rideHeightMm,-30,10,b.suspensionStage>=2," mm");setupDraft.springPercent=Knob(350,394,"Spring stiffness",setupDraft.springPercent,80,120,b.suspensionStage>=2);setupDraft.damperPercent=Knob(350,454,"Damping",setupDraft.damperPercent,80,120,b.suspensionStage>=2);setupDraft.rollPercent=Knob(350,514,"Anti-roll",setupDraft.rollPercent,70,130,b.suspensionStage>=2);
  setupDraft.wingPercent=Knob(350,584,"Wing setting",setupDraft.wingPercent,0,100,b.aeroStage>0);
  Label(new Rect(894,231,490,27),"GEARING / Shorter = more torque at the tires",17,accent,true);int fd=b.gearingStage==0?0:b.gearingStage==1?15:b.gearingStage==2?20:25;setupDraft.finalDrivePercent=Knob(894,272,"Final drive "+(c.car.stock.finalDrive*setupDraft.finalDrivePercent/100f).ToString("F2")+":1",setupDraft.finalDrivePercent,100-fd,100+fd,b.gearingStage>0);
  int range=b.gearingStage<2?0:b.gearingStage==2?10:20;for(int i=0;i<c.car.stock.ratios.Length;i++)setupDraft.SetGear(i,Knob(894,334+i*50,"Gear "+(i+1)+" / "+(c.car.stock.ratios[i]*setupDraft.Gear(i)/100f).ToString("F2")+":1",setupDraft.Gear(i),100-range,100+range,b.gearingStage>=2));
  var preview=c.Copy(b);preview.setup=setupDraft;Label(new Rect(350,640,1010,22),"At 100 km/h: "+(ChassisTuning.Downforce(c,preview)*10000/12.96f).ToString("F0")+" N downforce / drag area "+ChassisTuning.Drag(c,preview).ToString("F2")+" m²",15,muted);
  string error=null;try{ChassisTuning.Validate(c,b,setupDraft);}catch(System.Exception e){error=e.Message;}Label(new Rect(350,660,1010,52),error??garage.Message??"No changes reach the car until you apply. Lower/stiffer setups need testing on rough roads.",17,error==null?muted:amber,false,true);
  if(error==null){if(GUI.Button(new Rect(350,731,280,42),garage.IsTest?"Apply test setup":"Save setup · free",primary))garage.SaveSetup(setupDraft);}else GUI.Box(new Rect(350,731,280,42),"CHECK SETUP",disabled);
  if(GUI.Button(new Rect(650,731,240,42),"Reset draft",button))setupDraft=new WorkshopSetup();if(GUI.Button(new Rect(910,731,480,42),"Back to parts",button))setupMode=false;
 }
}
}
