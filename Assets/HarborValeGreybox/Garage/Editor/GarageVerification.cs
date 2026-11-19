using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Garage.Editor {
public static class GarageVerification {
 static List<string> results=new List<string>();static string directory;static SimulationMode oldMode;
 static void Check(bool ok,string text){results.Add((ok?"PASS ":"FAIL ")+text);if(!ok)throw new Exception(text);}
 static TrialSession Finish(TrialDefinition d){var s=new TrialSession(d);s.Tick(4,Vector3.zero,Vector3.zero);s.Tick(90,Vector3.zero,Vector3.zero);foreach(var g in d.checkpoints)s.Tick(.02f,g.Position-g.Forward,g.Position+g.Forward);return s;}
 static string FileFor(string name)=>Path.Combine(directory,name+".json");
 static void Legacy(string file,RaceProgress profile){var p=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(profile));p.version=1;p.availableCredits=0;p.prototypeCarId=null;p.spending=null;File.WriteAllText(file,JsonUtility.ToJson(p,true));}
 static float Acceleration(FinchDefinition definition){
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab");var model=(GameObject)PrefabUtility.InstantiatePrefab(prefab);model.transform.position=new Vector3(0,.35f,0);var car=model.AddComponent<FinchVehicle>();car.Initialize();car.externalInput=true;car.InstallPowertrain(definition);Physics.SyncTransforms();for(int i=0;i<150;i++){car.Tick(.02f);Physics.Simulate(.02f);}car.commandedPedal=1;float time=-1;for(int i=0;i<1000;i++){car.Tick(.02f);Physics.Simulate(.02f);if(car.SpeedKph>=100){time=(i+1)*.02f;break;}}UnityEngine.Object.DestroyImmediate(model);return time;
 }
 public static void Run(){
  Directory.CreateDirectory("Verification/Garage");directory=Path.GetFullPath("Verification/Garage/Fixtures_"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(directory);results.Clear();oldMode=Physics.simulationMode;
  try{
   var catalog=JsonUtility.FromJson<GarageCatalog>(Resources.Load<TextAsset>("FinchGarageCatalog").text);catalog.Validate();var trial=JsonUtility.FromJson<TrialDefinition>(Resources.Load<TextAsset>("FirstDocksideTrial").text);Check(catalog.offer.price==400&&catalog.stock.SampleTorque(4500)==119&&catalog.stageOne.SampleTorque(4500)==125,"Imported 400-credit package and authored 119→125 Nm at 4500 RPM");
   var empty=new RaceProgressStore(FileFor("empty"),trial);bool denied=false;try{empty.PurchaseStageOne(Guid.NewGuid().ToString("N"),catalog.offer,400,out _);}catch(InvalidOperationException){denied=true;}Check(denied&&empty.Profile.availableCredits==0&&empty.Profile.engineStage==0,"Insufficient funds cannot debit or install");
   var seed=new RaceProgressStore(FileFor("seed"),trial);seed.Settle(Finish(trial),out _);string legacy=FileFor("legacy-v1");Legacy(legacy,seed.Profile);string oldText=File.ReadAllText(legacy);var migrated=new RaceProgressStore(FileFor("new-v2"),trial,legacyFile:legacy);Check(migrated.Profile.version==2&&migrated.Profile.availableCredits==1100&&migrated.Profile.receipts.Count==1,"Valid v1 earnings/first-clear/history import into v2");
   string id=Guid.NewGuid().ToString("N");migrated.PurchaseStageOne(id,catalog.offer,400,out var duplicate);Check(!duplicate&&migrated.Profile.engineStage==1&&migrated.Profile.availableCredits==700&&migrated.Profile.earnings==1100&&migrated.Profile.spending.Count==1,"Atomic purchase saves stage, receipt and 400-credit debit");Check(File.ReadAllText(legacy)==oldText,"Legacy v1 file remains unchanged");
   migrated.PurchaseStageOne(id,catalog.offer,400,out duplicate);Check(duplicate&&migrated.Profile.availableCredits==700,"Duplicate request cannot charge twice");bool repeat=false;try{migrated.PurchaseStageOne(Guid.NewGuid().ToString("N"),catalog.offer,400,out _);}catch(InvalidOperationException){repeat=true;}Check(repeat&&migrated.Profile.availableCredits==700,"New request cannot rebuy installed stage");
   var reloaded=new RaceProgressStore(FileFor("new-v2"),trial,legacyFile:legacy);Check(reloaded.Profile.engineStage==1&&reloaded.Profile.availableCredits==700&&reloaded.Profile.prototypeCarId==migrated.Profile.prototypeCarId,"Reload preserves installed curve identity and available credits");reloaded.Settle(Finish(trial),out _);Check(reloaded.Profile.earnings==1800&&reloaded.Profile.availableCredits==1400&&reloaded.Profile.engineStage==1,"Replay adds reward without erasing spending or tune");
   File.WriteAllText(legacy,"{old build changed its file}");Check(new RaceProgressStore(FileFor("new-v2"),trial,legacyFile:legacy).Profile.availableCredits==1400,"Existing v2 never reimports or depends on an older build's file");
   var faultSeed=new RaceProgressStore(FileFor("fault"),trial);faultSeed.Settle(Finish(trial),out _);var fault=new RaceProgressStore(FileFor("fault"),trial,()=>{throw new IOException("Injected failed replace");});bool failed=false;try{fault.PurchaseStageOne(Guid.NewGuid().ToString("N"),catalog.offer,400,out _);}catch(IOException){failed=true;}var unchanged=new RaceProgressStore(FileFor("fault"),trial);Check(failed&&fault.Profile.engineStage==0&&unchanged.Profile.availableCredits==1100&&unchanged.Profile.engineStage==0,"Failed save leaves committed funds and installed stage unchanged");
   var stale=new RaceProgressStore(FileFor("fault"),trial);bool quote=false;try{stale.PurchaseStageOne(Guid.NewGuid().ToString("N"),catalog.offer,450,out _);}catch(InvalidOperationException){quote=true;}Check(quote&&stale.Profile.availableCredits==1100,"Stale quote rejected without charge");
   var a=new RaceProgressStore(FileFor("fault"),trial);var b=new RaceProgressStore(FileFor("fault"),trial);a.PurchaseStageOne(Guid.NewGuid().ToString("N"),catalog.offer,400,out _);bool simultaneous=false;try{b.PurchaseStageOne(Guid.NewGuid().ToString("N"),catalog.offer,400,out _);}catch(InvalidOperationException){simultaneous=true;}Check(simultaneous&&b.Profile.availableCredits==700,"Stale second instance cannot buy same stage again");
   var altered=JsonUtility.FromJson<GarageCatalog>(JsonUtility.ToJson(catalog));altered.offer.price=500;migrated.PurchaseStageOne(id,altered.offer,400,out duplicate);Check(duplicate&&migrated.Profile.spending[0].credits==400,"Historical fee and idempotent retry remain 400 when catalog quote changes");
   var paid=new RaceProgressStore(FileFor("fault"),trial);paid.Settle(Finish(trial),out _);File.WriteAllText(FileFor("fault"),"{broken");var restored=new RaceProgressStore(FileFor("fault"),trial);Check(restored.RecoveredBackup&&restored.Profile.engineStage==1&&restored.Profile.availableCredits==700,"Validated backup restores spending and installed tune together");
   var newer=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(restored.Profile));newer.version=3;string future=JsonUtility.ToJson(newer);File.WriteAllText(FileFor("newer"),future);File.WriteAllText(FileFor("newer")+".bak",JsonUtility.ToJson(restored.Profile));bool unsupported=false;try{new RaceProgressStore(FileFor("newer"),trial);}catch(UnsupportedProgressVersionException){unsupported=true;}Check(unsupported&&File.ReadAllText(FileFor("newer"))==future,"Unknown newer schema never falls back to overwrite its progress");
   // Native stock/upgraded acceleration on a preserved independent handling pad.
   EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Driving/Scenes/Finch_HandlingTest.unity");foreach(var spec in UnityEngine.Object.FindObjectsByType<StarterCarSpecification>(FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(spec.gameObject);Physics.simulationMode=SimulationMode.Script;
   float stock=Acceleration(catalog.stock),upgraded=Acceleration(catalog.stageOne);Check(stock>0&&upgraded>0&&upgraded<stock-.25f,"Measured 0-100 km/h stock "+stock.ToString("F2")+" s → Stage 1 "+upgraded.ToString("F2")+" s");Check(catalog.stageOne.PeakKw()>catalog.stock.PeakKw(),"Derived peak power increases from authored torque curve");
   File.WriteAllLines("Verification/Garage/rule-physics-results.txt",results);Debug.Log("FINCH_GARAGE_RULE_PHYSICS_PASS");
  }catch(Exception e){results.Add(e.ToString());File.WriteAllLines("Verification/Garage/rule-physics-results.txt",results);throw;}finally{Physics.simulationMode=oldMode;}
 }
}
}
