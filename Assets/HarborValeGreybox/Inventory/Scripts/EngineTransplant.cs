using System;
using System.Linq;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
public static class EngineTransplant {
 public static float EngineMass(string model)=>model=="car_finch"?110:model=="car_coda"?145:model=="car_vector"?190:model=="car_bastion"?230:model=="car_tempest"?225:160;
 static FinchDefinition Merge(FinchDefinition engine,FinchDefinition frame,float delta){var d=JsonUtility.FromJson<FinchDefinition>(JsonUtility.ToJson(engine));d.carId=frame.carId;d.massKg=frame.massKg+delta;d.wheelRadius=frame.wheelRadius;d.efficiency=frame.efficiency;d.ratios=(float[])frame.ratios.Clone();d.finalDrive=frame.finalDrive;d.reverseRatio=frame.reverseRatio;d.shiftSeconds=frame.shiftSeconds;d.brakeDeceleration=frame.brakeDeceleration;d.source="Engine transplant / "+engine.carId+" into "+frame.carId;return d;}
 public static WorkshopCatalog Combine(WorkshopCatalog frame,WorkshopCatalog engine){float delta=EngineMass(engine.car.carId)-EngineMass(frame.car.carId);var c=JsonUtility.FromJson<WorkshopCar>(JsonUtility.ToJson(frame.car));c.stock=Merge(engine.car.stock,frame.car.stock,delta);c.stageOne=Merge(engine.car.stageOne,frame.car.stock,delta);c.legacyTurbo=engine.car.legacyTurbo;c.legacyCooled=engine.car.legacyCooled;c.naCurves=engine.car.naCurves;c.curves=engine.car.curves;c.intakeDelta=engine.car.intakeDelta;c.exhaustDelta=engine.car.exhaustDelta;c.coolingFactors=engine.car.coolingFactors;c.naStagingRpm=engine.car.naStagingRpm;c.maxPowerKw=engine.car.maxPowerKw;var offers=frame.offers.Select(o=>IsEngineSlot(o.slot)?engine.Find(o.id):o).ToArray();return new WorkshopCatalog{car=c,offers=offers,EngineOverlay=true,EngineCalibrationId=engine.car.carId};}
 public static bool IsEngineSlot(string slot)=>slot=="na"||slot=="induction"||slot=="cooling"||slot=="intake"||slot=="exhaust"||slot=="sound";
}
}
