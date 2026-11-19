using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Workshop.Editor {
public static class BrakeBoostVerification {
 static List<string> lines=new List<string>();static void Check(bool yes,string name){lines.Add((yes?"PASS ":"FAIL ")+name);if(!yes)throw new Exception(name);}
 public static void Run(){Directory.CreateDirectory("Verification/BrakeBoost");var prior=Physics.simulationMode;try{
  var c=WorkshopCatalog.Load("car_finch");var b=new WorkshopBuild();foreach(var o in c.MissingPath(b,c.Find("induction_large_1")))c.Apply(b,o);b.tireStage=3;var d=c.Resolve(b);var engine=new FinchPowertrain(d);for(int i=0;i<150;i++)engine.HoldStaging(1,.02f);Check(Mathf.Abs(engine.Rpm-d.stagingRpm)<1&&engine.TurboAvailability>.2f,"Tune-specific hold RPM and limited staged turbo boost build progressively");Check(!engine.RequestShift(1,0),"Paddle shift rejected during brake staging");float held=engine.Rpm,boost=engine.TurboAvailability;engine.ReleaseStaging();engine.Step(0,0,1,false,.02f,true,0);Check(engine.Rpm>held-300&&engine.TurboAvailability>=boost,"Release carries finite rev/spool state into actual launch");engine.CancelStaging();Check(!engine.BrakeBoostHolding,"Lift/modal cancellation clears staging");var small=c.Copy(b);small.induction="small";Check(c.Resolve(small).stagingRpm!=d.stagingRpm,"Launch target depends on induction calibration");
  Physics.simulationMode=SimulationMode.Script;EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Driving/Scenes/Finch_HandlingTest.unity");foreach(var authored in UnityEngine.Object.FindObjectsByType<StarterCarSpecification>(FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(authored.gameObject);
  float plain=Launch(c,b,false,false),staged=Launch(c,b,true,false);Check(staged>plain,"Staged turbo launch covers more distance in first three seconds with suitable tires");Launch(c,b,true,true);File.WriteAllLines("Verification/BrakeBoost/results.txt",lines);
 }catch(Exception e){lines.Add(e.ToString());File.WriteAllLines("Verification/BrakeBoost/results.txt",lines);throw;}finally{Physics.simulationMode=prior;}}
 static float Launch(WorkshopCatalog c,WorkshopBuild b,bool stage,bool countdown){var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab");var g=(GameObject)PrefabUtility.InstantiatePrefab(prefab);g.transform.position=new Vector3(10,.35f,0);var car=g.AddComponent<FinchVehicle>();car.Initialize();car.externalInput=true;car.playerInput=true;car.InstallPowertrain(c.Resolve(b));car.forwardGrip=1.15f*c.car.gripModifiers[3];car.lateralGrip=1.1f*c.car.gripModifiers[3];ChassisTuning.ApplyVehicle(c,b,car);Physics.SyncTransforms();for(int i=0;i<150;i++){car.Tick(.02f);Physics.Simulate(.02f);}float start=car.Body.position.z;
  if(stage){car.commandedPedal=1;car.commandedServiceBrake=1;car.controlsLocked=countdown;car.countdownStagingAllowed=countdown;for(int i=0;i<150;i++){car.Tick(.02f);Physics.Simulate(.02f);}Check(car.IsBrakeBoosting&&Mathf.Abs(car.Engine.Rpm-car.Engine.StagingTargetRpm)<100,"Vehicle holds calibrated RPM under brake "+(countdown?"during countdown":"while stopped"));Check(car.SpeedKph<.5f&&Mathf.Abs(car.Body.position.z-start)<.2f,"Brakes prevent premature launch "+countdown);if(countdown){car.commandedServiceBrake=0;for(int i=0;i<15;i++){car.Tick(.02f);Physics.Simulate(.02f);}Check(car.SpeedKph<.5f,"Releasing before green cannot launch through countdown lock");car.controlsLocked=false;car.countdownStagingAllowed=false;car.commandedPedal=0;car.Tick(.02f);Check(!car.IsBrakeBoosting,"Countdown/staging cancellation safe");UnityEngine.Object.DestroyImmediate(g);return 0;}}
  car.controlsLocked=false;car.countdownStagingAllowed=false;car.commandedPedal=1;car.commandedServiceBrake=0;for(int i=0;i<150;i++){car.Tick(.02f);Physics.Simulate(.02f);}float distance=car.Body.position.z-start;lines.Add("MEASURE staged="+stage+" distance3s="+distance.ToString("F2")+"m speed="+car.SpeedKph.ToString("F1")+"km/h");UnityEngine.Object.DestroyImmediate(g);return distance;
 }
 public static void Release(){Run();ChassisVerification.Release();}
}
}
