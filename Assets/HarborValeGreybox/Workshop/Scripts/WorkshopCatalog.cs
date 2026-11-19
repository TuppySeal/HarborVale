using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Workshop {
[Serializable] public class WorkshopBuild {public WorkshopSetup setup=new WorkshopSetup();public int differentialStage,suspensionStage,gearingStage,aeroStage;public string induction="none",sound="road";public int inductionStage,coolingStage,brakeStage,gearboxStage,naStage,intakeStage,exhaustStage,tireStage,soundStage=1;}
[Serializable] public class WorkshopOffer {public string id,name,category,description,slot,choice;public int stage,price;public string[] prerequisites;public bool apexOnly;public float powerLoss;}
[Serializable] public class WorkshopCurve {public string choice;public int stage;public float[] torque,cooling1,offBoostTorque;public float spoolRise,boostThreshold,stagingRpm,stagingBoostFraction;}
[Serializable] public class WorkshopNa {public float[] torque;}
[Serializable] public class WorkshopCar {public string carId;public FinchDefinition stock,stageOne,legacyTurbo,legacyCooled;public float[] gearboxMultipliers,brakes,intakeDelta,exhaustDelta,coolingFactors,gripModifiers,differentialBiases,springs,dampers,rollBars,aeroDownforce,aeroDrag,aeroMass,naStagingRpm;public float aeroBase,dragBase;public WorkshopCurve[] curves;public float maxPowerKw;public WorkshopNa[] naCurves;}
[Serializable] public class WorkshopData {public int schema;public WorkshopOffer[] offers;public WorkshopCar[] cars;}
public sealed class WorkshopCatalog {
 public WorkshopOffer[] offers;public WorkshopCar car;static WorkshopData data;
 public static WorkshopCatalog Load(string carId){if(data==null){var text=Resources.Load<TextAsset>("ExpandedWorkshop");if(text==null)throw new InvalidOperationException("Expanded workshop data missing");data=JsonUtility.FromJson<WorkshopData>(text.text);}var c=data.cars.Single(x=>x.carId==carId);return new WorkshopCatalog{offers=data.offers,car=c};}
 public WorkshopOffer Find(string id)=>offers.SingleOrDefault(o=>o.id==id)??throw new InvalidOperationException("Unknown part: "+id);
 public WorkshopBuild Copy(WorkshopBuild b)=>JsonUtility.FromJson<WorkshopBuild>(JsonUtility.ToJson(b));
 public WorkshopBuild FromLegacy(int engine,int gearbox,int cooler)=>new WorkshopBuild{naStage=engine>0?1:0,gearboxStage=gearbox,coolingStage=cooler,induction=engine>=2?"small":"none",inductionStage=engine>=2?1:0};
 public int Stage(WorkshopBuild b,string slot)=>slot=="differential"?b.differentialStage:slot=="suspension"?b.suspensionStage:slot=="gearing"?b.gearingStage:slot=="aero"?b.aeroStage:slot=="tires"?b.tireStage:slot=="na"?b.naStage:slot=="gearbox"?b.gearboxStage:slot=="cooling"?b.coolingStage:slot=="brakes"?b.brakeStage:slot=="intake"?b.intakeStage:slot=="exhaust"?b.exhaustStage:slot=="sound"?b.soundStage:b.inductionStage;
 public bool Installed(WorkshopBuild b,WorkshopOffer o)=>o.slot=="induction"&&o.choice=="none"?b.induction=="none":Stage(b,o.slot)>=o.stage&&(o.slot!="induction"||b.induction==o.choice)&&(o.slot!="sound"||b.sound==o.choice);
 public string Requirement(WorkshopBuild b,WorkshopOffer o,bool ignorePrice=true){if(o.apexOnly&&car.carId!="car_apex_test")return "Apex test only: no compatible twin-turbo kit for the Finch.";if(Installed(b,o))return "Installed";foreach(var id in o.prerequisites??Array.Empty<string>())if(!Installed(b,Find(id)))return "Requires "+Find(id).name;if(o.slot=="cooling"&&b.induction=="none")return "Requires an induction kit first";return null;}
 public void Apply(WorkshopBuild b,WorkshopOffer o){var reason=Requirement(b,o);if(reason!=null)throw new InvalidOperationException(reason);switch(o.slot){case "differential":b.differentialStage=o.stage;break;case "suspension":b.suspensionStage=o.stage;break;case "gearing":b.gearingStage=o.stage;break;case "aero":b.aeroStage=o.stage;break;case "tires":b.tireStage=o.stage;break;case "na":b.naStage=o.stage;break;case "gearbox":b.gearboxStage=o.stage;break;case "cooling":b.coolingStage=o.stage;break;case "brakes":b.brakeStage=o.stage;break;case "intake":b.intakeStage=o.stage;break;case "exhaust":b.exhaustStage=o.stage;break;case "induction":b.induction=o.choice;b.inductionStage=o.choice=="none"?0:o.stage;break;case "sound":b.sound=o.choice;b.soundStage=o.stage;break;default:throw new InvalidOperationException("Unsupported slot");}}
 public List<WorkshopOffer> MissingPath(WorkshopBuild b,WorkshopOffer o){var list=new List<WorkshopOffer>();var seen=new HashSet<string>();Collect(b,o,list,seen);return list;}
 void Collect(WorkshopBuild b,WorkshopOffer o,List<WorkshopOffer> list,HashSet<string> seen){if(Installed(b,o)||!seen.Add(o.id))return;foreach(var id in o.prerequisites??Array.Empty<string>())Collect(b,Find(id),list,seen);if(o.slot=="cooling"&&b.induction=="none"&&!list.Any(p=>p.slot=="induction"))Collect(b,Find("induction_small_1"),list,seen);list.Add(o);}
 public FinchDefinition Preview(WorkshopBuild b,WorkshopOffer o){var copy=Copy(b);foreach(var step in MissingPath(copy,o))if(!Installed(copy,step))Apply(copy,step);return Resolve(copy);}
 public FinchDefinition Resolve(WorkshopBuild b){
  var d=JsonUtility.FromJson<FinchDefinition>(JsonUtility.ToJson(b.naStage>0?car.stageOne:car.stock));
  if(b.induction!="none"){
   var curve=car.curves.Single(c=>c.choice==b.induction&&c.stage==b.inductionStage);d.stagingRpm=curve.stagingRpm;d.stagingBoostFraction=curve.stagingBoostFraction;d.torque=(float[])(b.coolingStage>0?curve.cooling1:curve.torque).Clone();d.aspiration=b.induction=="roots"?"supercharged":"turbocharged";d.offBoostTorque=(float[])curve.offBoostTorque.Clone();d.spoolRise=curve.spoolRise;d.spoolFall=.18f;d.boostThresholdRpm=curve.boostThreshold;d.fullBoostRpm=b.induction=="large"?4500:b.induction=="twin"?3100:car.legacyTurbo.fullBoostRpm;d.displayBoostBar=car.legacyTurbo.displayBoostBar*(1+.2f*(b.inductionStage-1));
   if(b.coolingStage>1)for(int i=0;i<d.torque.Length;i++)if(!d.IsTurbo||d.rpm[i]>d.boostThresholdRpm)d.torque[i]*=car.coolingFactors[b.coolingStage];
  }else{d.stagingRpm=car.naStagingRpm[b.naStage];d.stagingBoostFraction=0;d.torque=(float[])car.naCurves[b.naStage].torque.Clone();}
  for(int i=0;i<d.torque.Length;i++){float addition=car.intakeDelta[i]*b.intakeStage+car.exhaustDelta[i]*b.exhaustStage;d.torque[i]+=addition;if(d.offBoostTorque!=null&&d.offBoostTorque.Length==d.torque.Length)d.offBoostTorque[i]+=addition;}
  d.shiftSeconds=car.stock.shiftSeconds*car.gearboxMultipliers[b.gearboxStage];d.brakeDeceleration=car.brakes[b.brakeStage];
  float loss=Find("sound_"+b.sound+"_"+b.soundStage).powerLoss;
  for(int i=0;i<d.torque.Length;i++){d.torque[i]*=1-loss;if(d.offBoostTorque!=null&&d.offBoostTorque.Length==d.torque.Length)d.offBoostTorque[i]*=1-loss;}
  d.stagingClutchLoad=.25f;ChassisTuning.Resolve(this,b,d);if(d.PeakKw()>car.maxPowerKw)throw new InvalidOperationException("Authored power limit exceeded");return d;
 }
}
}
