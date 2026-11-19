using System;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Garage {
[Serializable]public class EngineTuneOffer {
 public int stage,requiredPreviousStage,requiredEngineStage,minimumReputation,supportingTransmission,price;public string pathId,configId,displayName,kind,partId;
 public string Kind=>string.IsNullOrEmpty(kind)?"engine":kind;
 public string PartId=>string.IsNullOrEmpty(partId)?"engine_stage1":partId;
 public void Validate(){bool valid=Kind=="engine"&&stage==1&&requiredPreviousStage==0&&supportingTransmission==0&&pathId=="path_finch_na"&&configId=="cfg_finch_na_s1"||Kind=="engine"&&stage==2&&requiredPreviousStage==1&&requiredEngineStage==1&&supportingTransmission==1&&pathId=="path_finch_turbo"&&configId=="cfg_finch_turbo_s2"||Kind=="gearbox"&&stage==1&&requiredPreviousStage==0&&requiredEngineStage==1&&configId=="tx_finch_s1"||Kind=="intercooler"&&stage==1&&requiredPreviousStage==0&&requiredEngineStage==2&&supportingTransmission==1&&pathId=="path_finch_turbo"&&configId=="cfg_finch_turbo_s2_ic1";if(!valid||price<=0||minimumReputation<0)throw new ArgumentException("Unsupported garage offer");}
}
[Serializable]public class GarageCatalog {
 public string title;public float x,y,z;public EngineTuneOffer offer,gearboxOffer,turboOffer,intercoolerOffer;public FinchDefinition stock,stageOne,turboBasic,turboStreet;public float gearboxShiftMultiplier;
 public Vector3 Position=>new Vector3(x,y,z);
 public void Validate(){if(offer==null||stock==null||stageOne==null)throw new ArgumentException("Garage catalog missing");offer.Validate();if(stock.carId!="car_finch"||stageOne.carId!="car_finch"||stageOne.rpm.Length!=stageOne.torque.Length)throw new ArgumentException("Incompatible car package");if(turboOffer!=null){gearboxOffer.Validate();turboOffer.Validate();intercoolerOffer.Validate();if(!turboBasic.IsTurbo||!turboStreet.IsTurbo||gearboxShiftMultiplier<=0||gearboxShiftMultiplier>=1||turboBasic.PeakKw()>90||turboStreet.PeakKw()>90)throw new ArgumentException("Invalid supporting/boost package");}}
 public FinchDefinition DefinitionFor(HarborValeGreybox.Racing.RaceProgress p){if(p.workshop!=null)return HarborValeGreybox.Workshop.WorkshopCatalog.Load("car_finch").Resolve(p.workshop);var source=p.engineStage==0?stock:p.engineStage==1?stageOne:p.intercoolerStage>0?turboStreet:turboBasic;var d=JsonUtility.FromJson<FinchDefinition>(JsonUtility.ToJson(source));if(p.gearboxStage>0)d.shiftSeconds*=gearboxShiftMultiplier;return d;}
 public int Installed(HarborValeGreybox.Racing.RaceProgress p,EngineTuneOffer o)=>o.Kind=="gearbox"?p.gearboxStage:o.Kind=="intercooler"?p.intercoolerStage:p.engineStage;
 public string Prerequisite(HarborValeGreybox.Racing.RaceProgress p,EngineTuneOffer o){if(Installed(p,o)>=o.stage)return "Installed";if(p.engineStage<o.requiredEngineStage)return "Requires paid engine Stage "+o.requiredEngineStage;if(p.gearboxStage<o.supportingTransmission)return "Requires gearbox support Stage "+o.supportingTransmission;if(Installed(p,o)!=o.requiredPreviousStage)return "Buy preceding stage first";return null;}
}
}
