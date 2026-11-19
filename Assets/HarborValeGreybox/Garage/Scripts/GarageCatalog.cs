using System;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Garage {
[Serializable]public class EngineTuneOffer {
 public int stage,requiredPreviousStage,minimumReputation,supportingTransmission,price;public string pathId,configId,displayName;
 public void Validate(){if(stage!=1||requiredPreviousStage!=0||supportingTransmission!=0||price<=0||minimumReputation<0||pathId!="path_finch_na"||configId!="cfg_finch_na_s1")throw new ArgumentException("Unsupported garage offer");}
}
[Serializable]public class GarageCatalog {
 public string title;public float x,y,z;public EngineTuneOffer offer;public FinchDefinition stock,stageOne;
 public Vector3 Position=>new Vector3(x,y,z);
 public void Validate(){if(offer==null||stock==null||stageOne==null)throw new ArgumentException("Garage catalog missing");offer.Validate();if(stock.carId!="car_finch"||stageOne.carId!="car_finch"||stageOne.rpm.Length!=stageOne.torque.Length)throw new ArgumentException("Incompatible car package");}
}
}
