using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.SportsTest.Editor {
public static class ApexVerification {
 public static void Release(){HarborValeGreybox.Boost.Editor.BoostVerification.Run();HarborValeGreybox.Boost.Editor.BoostPlayerBuild.Build();Build();}
 const string Scene="Assets/HarborValeGreybox/SportsTest/Scenes/HarborVale_SportsTest.unity";
 static List<string> lines=new List<string>();
 static void Check(bool ok,string text){lines.Add((ok?"PASS ":"FAIL ")+text);if(!ok)throw new Exception(text);}
 public static void Run(){Directory.CreateDirectory("Verification/Sports");lines.Clear();var old=Physics.simulationMode;try{
  if(!File.Exists(Scene)){if(!AssetDatabase.CopyAsset("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity",Scene))throw new Exception("Scene copy failed");AssetDatabase.Refresh();}
  var c=JsonUtility.FromJson<ApexCatalog>(Resources.Load<TextAsset>("ApexTestCatalog").text);Check(c.stock.carId=="car_apex_test"&&c.turboStreet.SampleTorque(4500)==510,"Dedicated sports data: 300 Nm stock to 510 Nm cooler package");Check(c.turboStreet.PeakKw()>c.turboBasic.PeakKw()&&c.turboBasic.PeakKw()>c.stageOne.PeakKw()&&c.stageOne.PeakKw()>c.stock.PeakKw(),"Matched torque curves increase peak power in order");Check(c.turboBasic.SampleOffBoost(4500)==c.turboStreet.SampleOffBoost(4500),"Intercooler preserves off-boost curve; no duplicate multiplier");Check(Mathf.Abs(c.Build(2).shiftSeconds-.2808f)<.00001f&&c.stock.shiftSeconds==.36f,"Gearbox reduces shift interruption without source compounding");
  EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Driving/Scenes/Finch_HandlingTest.unity");foreach(var spec in UnityEngine.Object.FindObjectsByType<StarterCarSpecification>(FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(spec.gameObject);Physics.simulationMode=SimulationMode.Script;float[] times=new float[5];
  for(int step=0;step<5;step++){var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab");var model=(GameObject)PrefabUtility.InstantiatePrefab(prefab);model.transform.position=new Vector3(0,.35f,0);var car=model.AddComponent<FinchVehicle>();car.Initialize();Check(car.frontDriveShare==1&&car.lateralGrip==1.1f&&car.downforceCoefficient==.35f,"Finch defaults remain unchanged (sample "+step+")");ApexTestDrive.Configure(car,c);car.InstallPowertrain(c.Build(step));car.externalInput=true;Physics.SyncTransforms();for(int n=0;n<150;n++){car.Tick(.02f);Physics.Simulate(.02f);}Check(car.GroundedWheels==4&&car.SpeedKph<1,"Sports chassis rests on four suspension contacts");Check(car.Body.mass==1250&&car.frontDriveShare==.45f&&car.lateralGrip==1.65f&&car.downforceCoefficient==2.2f,"Sports mass/AWD/grip/aero configured independently");car.commandedPedal=1;times[step]=-1;for(int n=0;n<1000;n++){car.Tick(.02f);Physics.Simulate(.02f);if(car.SpeedKph>=100){times[step]=(n+1)*.02f;break;}}Check(times[step]>0,"Actual sports 0-100 step "+step+": "+times[step].ToString("F2")+" s; peak "+c.Build(step).PeakKw().ToString("F1")+" kW");car.ResetAt(new Vector3(0,1,0),Quaternion.identity);Check(car.Engine.TurboAvailability==0&&car.Engine.definition.carId=="car_apex_test","Recovery clears spool while preserving sports configuration");UnityEngine.Object.DestroyImmediate(model);}
  Check(times[1]<times[0]&&times[3]<times[1]&&times[4]<times[3],"Upgraded sports acceleration improves measurably");File.WriteAllLines("Verification/Sports/physics-results.txt",lines);
 }catch(Exception e){lines.Add(e.ToString());File.WriteAllLines("Verification/Sports/physics-results.txt",lines);throw;}finally{Physics.simulationMode=old;}}
 public static void Build(){Directory.CreateDirectory("Verification/Sports/Windows");PlayerSettings.productName="Harbor Vale — Apex Sports Test";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Scene},locationPathName="Verification/Sports/Windows/HarborVale-SportsTest.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});File.WriteAllText("Verification/Sports/player-build.txt",r.summary.result+" | errors "+r.summary.totalErrors);if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Sports build failed");}
}
}
