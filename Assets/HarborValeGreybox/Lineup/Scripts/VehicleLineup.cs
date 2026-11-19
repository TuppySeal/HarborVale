using System;
using System.Linq;
using UnityEngine;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
[Serializable] public class VehicleSpec {public string id,name,shape,engine,stockDrive;public int cylinders,price;public float length,width,height,wheelBase,track,wheelRadius,mass,priceFactor,steer;public float[] color;public WorkshopCar workshop;}
[Serializable] public class VehicleLineupData {public int schema;public VehicleSpec[] cars;}
public static class VehicleLineup {
 static VehicleLineupData data;public static VehicleSpec[] All{get{if(data==null)data=JsonUtility.FromJson<VehicleLineupData>(Resources.Load<TextAsset>("VehicleLineup").text);return data.cars;}}
 public static VehicleSpec Find(string id)=>All.SingleOrDefault(c=>c.id==id)??throw new InvalidOperationException("Unknown car model");
 public static bool IsNew(string id)=>id!="car_finch"&&id!="car_apex_test";
 public static string Model(FleetCar car)=>string.IsNullOrEmpty(car.modelId)?"car_finch":car.modelId;
 public static int Cylinders(string id)=>IsNew(id)?Find(id).cylinders:4;
 public static void NormalizeLegacy(FleetState f){if(f==null)return;foreach(var c in f.cars){if(!string.IsNullOrEmpty(c.modelId)&&c.modelId!="car_finch")throw new InvalidOperationException("Legacy fleet contains unsupported car model");c.modelId="car_finch";}if(f.HasPending){if(!string.IsNullOrEmpty(f.pending.playerModelId)&&f.pending.playerModelId!="car_finch"||!string.IsNullOrEmpty(f.pending.opponentModelId)&&f.pending.opponentModelId!="car_finch")throw new InvalidOperationException("Legacy stake contains unsupported model");f.pending.playerModelId=f.pending.opponentModelId="car_finch";}}
 public static WorkshopCatalog Catalog(string id,WorkshopOffer[] offers){var s=Find(id);var copy=JsonUtility.FromJson<WorkshopOfferList>(JsonUtility.ToJson(new WorkshopOfferList{offers=offers})).offers;foreach(var o in copy){o.description=o.description?.Replace("Finch",s.name);if(id=="car_tempest"&&o.slot=="na"){o.name="Factory twin-turbo tune / Stage "+o.stage;o.description="Matched ECU and engine support for the factory twin-turbo V8. Buy preceding support stages first.";}if(o.price>0)o.price=checked((int)(((long)o.price*(int)Math.Round(s.priceFactor*10000)+99999)/100000)*10);}return new WorkshopCatalog{offers=copy,car=s.workshop};}
 [Serializable]class WorkshopOfferList{public WorkshopOffer[] offers;}
}
}
