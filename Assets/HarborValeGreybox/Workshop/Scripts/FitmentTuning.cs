using System;
using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Workshop {
public static class FitmentTuning {
 public static bool IsNew(WorkshopOffer o)=>o.slot=="drivetrain"||o.slot=="tirecompound";
 public static int TireBit(string choice)=>choice=="road"?1:choice=="drift"?2:choice=="slick"?4:throw new InvalidOperationException("Unknown tire set");
 public static bool Owns(WorkshopBuild b,string choice)=>(b.tireOwned&TireBit(choice))!=0;
 public static string Drive(WorkshopCatalog c,WorkshopBuild b)=>b.drive=="stock"?(!string.IsNullOrEmpty(c.car.stockDrive)?c.car.stockDrive:c.car.carId=="car_apex_test"?"awd":"fwd"):b.drive;
 public static void NormalizeLegacy(WorkshopBuild b){if(b==null)return;if(!string.IsNullOrEmpty(b.drive)&&b.drive!="stock"||!string.IsNullOrEmpty(b.tireCompound)&&b.tireCompound!="road"||b.tireOwned!=0&&b.tireOwned!=1)throw new InvalidOperationException("Legacy build contains unsupported fitment data");b.drive="stock";b.tireCompound="road";b.tireOwned=1;}
 public static void Validate(WorkshopCatalog c,WorkshopBuild b){if(b.drive!="stock"&&b.drive!="fwd"&&b.drive!="rwd"&&b.drive!="awd")throw new InvalidOperationException("Unknown drivetrain");if(b.tireOwned<1||b.tireOwned>7||(b.tireOwned&1)==0||!Owns(b,b.tireCompound))throw new InvalidOperationException("Installed tire set is not owned");}
 static int Index(string layout)=>layout=="fwd"?0:layout=="rwd"?1:layout=="awd"?2:throw new InvalidOperationException("Unknown drivetrain");
 public static void Resolve(WorkshopCatalog c,WorkshopBuild b,FinchDefinition d){Validate(c,b);int i=Index(Drive(c,b));if(b.drive!="stock"){if(c.car.driveMass==null||c.car.driveEfficiencies==null||c.car.driveMass.Length!=3||c.car.driveEfficiencies.Length!=3)throw new InvalidOperationException("Matched conversion specifications missing");d.massKg+=c.car.driveMass[i];d.efficiency=c.car.driveEfficiencies[i];}}
 public static void ApplyVehicle(WorkshopCatalog c,WorkshopBuild b,FinchVehicle car){Validate(c,b);string drive=Drive(c,b);car.frontDriveShare=drive=="fwd"?1:drive=="rwd"?0:.45f;car.driveLabel=drive.ToUpperInvariant();car.rearLateralGripMultiplier=b.tireCompound=="drift"?.82f:1;float quality=c.car.gripModifiers[b.tireStage];car.forwardGrip=(c.car.forwardGrip>0?c.car.forwardGrip:c.car.carId=="car_apex_test"?1.70f:1.15f)*quality*(b.tireCompound=="slick"?1.25f:b.tireCompound=="drift"?.98f:1);car.lateralGrip=(c.car.lateralGrip>0?c.car.lateralGrip:c.car.carId=="car_apex_test"?1.65f:1.10f)*quality*(b.tireCompound=="slick"?1.22f:1);
  for(int i=0;i<car.Wheels.Length;i++){var w=car.Wheels[i];var f=w.forwardFriction;f.extremumSlip=b.tireCompound=="slick"?.20f:b.tireCompound=="drift"?.38f:.32f;f.extremumValue=1;f.asymptoteSlip=b.tireCompound=="slick"?.60f:b.tireCompound=="drift"?1:.85f;f.asymptoteValue=b.tireCompound=="slick"?.68f:b.tireCompound=="drift"?.86f:.75f;f.stiffness=car.forwardGrip;w.forwardFriction=f;f=w.sidewaysFriction;bool drift=b.tireCompound=="drift"&&i>=2;f.extremumSlip=b.tireCompound=="slick"?.18f:drift?.32f:.22f;f.extremumValue=1;f.asymptoteSlip=b.tireCompound=="slick"?.55f:drift?.90f:.65f;f.asymptoteValue=b.tireCompound=="slick"?.68f:drift?.88f:.78f;f.stiffness=car.lateralGrip*(i>=2?car.rearLateralGripMultiplier:1);w.sidewaysFriction=f;}
 }
}
}
