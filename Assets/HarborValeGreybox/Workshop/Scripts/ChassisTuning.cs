using System;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Workshop {
[Serializable] public class WorkshopSetup {
 public int rideHeightMm;public int springPercent=100,damperPercent=100,rollPercent=100,diffLockPercent=50,finalDrivePercent=100,gear1Percent=100,gear2Percent=100,gear3Percent=100,gear4Percent=100,gear5Percent=100,gear6Percent=100,wingPercent=50;
 public int Gear(int i)=>i==0?gear1Percent:i==1?gear2Percent:i==2?gear3Percent:i==3?gear4Percent:i==4?gear5Percent:gear6Percent;
 public void SetGear(int i,int value){switch(i){case 0:gear1Percent=value;break;case 1:gear2Percent=value;break;case 2:gear3Percent=value;break;case 3:gear4Percent=value;break;case 4:gear5Percent=value;break;case 5:gear6Percent=value;break;}}
}
public static class ChassisTuning {
 public const float HpPerKw=1.34102209f;
 public static float CrankHp(FinchDefinition d)=>d.PeakKw()*HpPerKw;
 public static float EstimatedWhp(FinchDefinition d)=>CrankHp(d)*d.efficiency;
 public static WorkshopSetup Copy(WorkshopSetup setup)=>JsonUtility.FromJson<WorkshopSetup>(JsonUtility.ToJson(setup));
 static void Range(int value,int min,int max,string label){if(value<min||value>max)throw new InvalidOperationException(label+" must be "+min+"–"+max);}
 public static void Validate(WorkshopCatalog c,WorkshopBuild b,WorkshopSetup s){
  if(s==null)throw new InvalidOperationException("Setup is missing");Range(s.diffLockPercent,0,100,"Differential lock");Range(s.wingPercent,0,100,"Wing setting");Range(s.rideHeightMm,-30,10,"Ride height");Range(s.springPercent,80,120,"Spring stiffness");Range(s.damperPercent,80,120,"Damping");Range(s.rollPercent,70,130,"Anti-roll");int fdRange=b.gearingStage==0?0:b.gearingStage==1?15:b.gearingStage==2?20:25;Range(s.finalDrivePercent,100-fdRange,100+fdRange,"Final drive");
  if(b.differentialStage==0&&s.diffLockPercent!=50)throw new InvalidOperationException("Install a limited-slip differential first");if(b.aeroStage==0&&s.wingPercent!=50)throw new InvalidOperationException("Install an aero package first");if(b.suspensionStage<2&&(s.rideHeightMm!=0||s.springPercent!=100||s.damperPercent!=100||s.rollPercent!=100))throw new InvalidOperationException("Suspension Stage 2 unlocks adjustment");
  float previous=float.MaxValue;for(int i=0;i<6;i++){int range=b.gearingStage<2?0:b.gearingStage==2?10:20;Range(s.Gear(i),100-range,100+range,"Gear ratio");if(i<c.car.stock.ratios.Length){float ratio=c.car.stock.ratios[i]*s.Gear(i)/100f;if(ratio>=previous*.99f)throw new InvalidOperationException("Gear ratios must remain in descending order");previous=ratio;}else if(s.Gear(i)!=100)throw new InvalidOperationException("This car has no gear "+(i+1));}
 }
 public static void ApplySetup(WorkshopCatalog c,WorkshopBuild b,WorkshopSetup s){Validate(c,b,s);b.setup=Copy(s);}
 public static void Resolve(WorkshopCatalog c,WorkshopBuild b,FinchDefinition d){Validate(c,b,b.setup);d.massKg=c.car.stock.massKg+c.car.aeroMass[b.aeroStage];d.finalDrive=c.car.stock.finalDrive*b.setup.finalDrivePercent/100f;for(int i=0;i<d.ratios.Length;i++)d.ratios[i]=c.car.stock.ratios[i]*b.setup.Gear(i)/100f;}
 public static float Downforce(WorkshopCatalog c,WorkshopBuild b)=>c.car.aeroBase+c.car.aeroDownforce[b.aeroStage]*Mathf.Lerp(.6f,1.4f,b.setup.wingPercent/100f);
 public static float Drag(WorkshopCatalog c,WorkshopBuild b)=>c.car.dragBase+c.car.aeroDrag[b.aeroStage]*Mathf.Lerp(.5f,1.5f,b.setup.wingPercent/100f);
 public static void ApplyVehicle(WorkshopCatalog c,WorkshopBuild b,FinchVehicle car){
  car.differentialBias=c.car.differentialBiases[b.differentialStage];car.differentialLock=b.differentialStage==0?0:b.setup.diffLockPercent/100f;car.downforceCoefficient=Downforce(c,b);car.aeroCdA=Drag(c,b);car.antiRoll=c.car.rollBars[b.suspensionStage]*b.setup.rollPercent/100f;
  foreach(var wheel in car.Wheels){var pos=wheel.transform.localPosition;pos.y=.415f-b.setup.rideHeightMm/1000f;wheel.transform.localPosition=pos;wheel.suspensionSpring=new JointSpring{spring=c.car.springs[b.suspensionStage]*b.setup.springPercent/100f,damper=c.car.dampers[b.suspensionStage]*b.setup.damperPercent/100f,targetPosition=.5f};wheel.sprungMass=car.Body.mass/4;}
  var visual=car.GetComponent<ChassisVisuals>();if(visual==null)visual=car.gameObject.AddComponent<ChassisVisuals>();visual.Configure(b.aeroStage,b.setup.wingPercent,c.car.carId=="car_apex_test");
 }
 public static Vector2 AxleTorque(float total,float leftRpm,float rightRpm,float bias,float locking){float maxTransfer=(Mathf.Max(1,bias)-1)/(Mathf.Max(1,bias)+1);float transfer=Mathf.Clamp((Mathf.Abs(leftRpm)-Mathf.Abs(rightRpm))/200f,-1,1)*Mathf.Clamp01(locking)*maxTransfer;float left=total*(.5f-.5f*transfer);return new Vector2(left,total-left);}
}
}
