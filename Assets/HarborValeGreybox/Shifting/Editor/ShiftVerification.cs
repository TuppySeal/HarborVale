using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Garage;
using HarborValeGreybox.SportsTest;
namespace HarborValeGreybox.Shifting.Editor {
public static class ShiftVerification {
 static List<string> lines=new List<string>();
 static void Check(bool yes,string message){lines.Add((yes?"PASS ":"FAIL ")+message);if(!yes)throw new Exception(message);}
 static float WheelRpm(FinchDefinition d,float speed)=>Mathf.Abs(speed)/d.wheelRadius*60/(2*Mathf.PI);
 static void Advance(FinchPowertrain e,float speed,float throttle,float seconds,float brake=0){for(int i=0;i<Mathf.CeilToInt(seconds/.02f);i++)e.Step(speed,WheelRpm(e.definition,speed),throttle,false,.02f,true,brake);}
 public static void Run(){Directory.CreateDirectory("Verification/Shifting");lines.Clear();var old=Physics.simulationMode;try{
  var garage=JsonUtility.FromJson<GarageCatalog>(Resources.Load<TextAsset>("FinchGarageCatalog").text);var sports=JsonUtility.FromJson<ApexCatalog>(Resources.Load<TextAsset>("ApexTestCatalog").text);
  foreach(var d in new[]{garage.stock,sports.turboStreet}){
   var e=new FinchPowertrain(d);var legacy=new LegacyFinchPowertrain(d);for(int i=0;i<400;i++){e.Step(60,WheelRpm(d,60),1,false,.02f);legacy.Step(60,WheelRpm(d,60),1,false,.02f);}Check(e.Gear==d.ratios.Length&&legacy.Gear==d.ratios.Length,"Both gearbox versions reach top gear in synthetic high-speed fixture: "+d.carId);
   for(int i=0;i<30;i++){e.Step(30/3.6f,WheelRpm(d,30/3.6f),1,false,.02f);legacy.Step(30/3.6f,WheelRpm(d,30/3.6f),1,false,.02f);}
   Check(e.Gear<=2&&legacy.Gear>=3,"Top-speed slowdown/reacceleration: new gear "+e.Gear+", prior gear "+legacy.Gear+" after 0.6 s at 30 km/h: "+d.carId);
   Check(e.ProjectedRpm(30/3.6f,e.Gear)<d.redlineRpm*.94f,"Matched kickdown remains below safe RPM");
   e.Reset();for(int i=0;i<d.ratios.Length-1;i++){Check(e.RequestShift(1,0),"Manual upshift accepts available gear");Advance(e,10,0,d.shiftSeconds+.1f);}Check(e.Mode==TransmissionMode.Manual,"Successful paddle request selects manual");
   int top=e.Gear;Check(!e.RequestShift(1,60)&&e.Gear==top,"Top gear rejects extra upshift");Check(!e.RequestShift(-1,80)&&e.Gear==top&&e.ShiftNote.Contains("RPM"),"Unsafe downshift rejected without changing gear");
   Check(e.RequestShift(-1,10)&&!e.RequestShift(-1,10),"One paddle command cannot overlap another shift");float min=1,max=0;bool partial=false;for(int i=0;i<Mathf.CeilToInt(d.shiftSeconds/.02f)+1;i++){e.Step(10,WheelRpm(d,10),1,false,.02f);float c=e.ClutchEngagement;Check(c>=0&&c<=1,"Clutch engagement bounded");min=Mathf.Min(min,c);max=Mathf.Max(max,c);if(c>0&&c<1)partial=true;}Check(min==0&&max==1&&partial,"Shift power returns through an intermediate engagement ramp");
   Advance(e,10,0,1);int manual=e.Gear;Advance(e,10,1,2);Check(e.Gear==manual,"Manual mode holds gear instead of automatic kickdown");Advance(e,0,0,1);Check(e.Gear==1&&e.Mode==TransmissionMode.Manual,"Coming to a stop selects first without leaving manual");e.Reset();Check(e.Mode==TransmissionMode.Manual,"Recovery reset preserves transmission mode");Check(!e.RequestShift(-1,0),"First gear rejects extra downshift");
   e.SetMode(TransmissionMode.Automatic);Advance(e,0,0,.1f);e.Step(0,0,1,true,.02f);Check(e.Gear==-1&&!e.RequestShift(1,0),"Reverse rejects paddle shifts");
   e=new FinchPowertrain(d);Advance(e,60,1,8);Advance(e,10,0,.6f,1);Check(e.Gear<=2,"Braking matches a lower road-speed gear");int coasting=e.Gear;Advance(e,10,0,3);Check(e.Gear==coasting,"Steady coasting does not hunt between gears");
  }
  var slipEngine=new FinchPowertrain(garage.stock);int changes=0,last=1;for(int i=0;i<300;i++){slipEngine.Step(11,WheelRpm(garage.stock,11)+200,1,false,.02f);if(slipEngine.Gear!=last){changes++;last=slipEngine.Gear;}}Check(slipEngine.Gear==2&&changes==1,"Loaded wheel slip permits sensible upshift without full-throttle up/down hunting");
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var ground=GameObject.CreatePrimitive(PrimitiveType.Cube);ground.name="Temporary native straight test surface";ground.transform.position=new Vector3(0,-.2f,2500);ground.transform.localScale=new Vector3(100,.4f,6000);Physics.simulationMode=SimulationMode.Script;
  foreach(bool apex in new[]{false,true}){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab");var model=(GameObject)PrefabUtility.InstantiatePrefab(prefab);model.transform.position=new Vector3(0,.5f,0);var car=model.AddComponent<FinchVehicle>();car.Initialize();car.externalInput=true;if(apex){ApexTestDrive.Configure(car,sports);car.InstallPowertrain(sports.Build(4),2);}for(int i=0;i<150;i++){car.Tick(.02f);Physics.Simulate(.02f);}
   car.commandedPedal=1;car.Tick(.02f);Check(car.Throttle>0&&car.Throttle<.2f,"Keyboard throttle ramps rather than jumps: "+apex);Physics.Simulate(.02f);float t100=-1;for(int i=0;i<2000;i++){car.Tick(.02f);Physics.Simulate(.02f);if(car.SpeedKph>=100&&t100<0)t100=(i+2)*.02f;if(car.SpeedKph>=125)break;}Check(t100>0,"Native 0-100 "+(apex?"Apex upgraded":"Finch stock")+": "+t100.ToString("F2")+" s");int high=car.Engine.Gear;car.commandedPedal=-1;car.Tick(.02f);Check(car.Brake>0&&car.Brake<.3f&&car.Throttle==0,"Brake blends in while opposite-direction drive cuts immediately");Physics.Simulate(.02f);for(int i=0;i<1000&&car.SpeedKph>30;i++){car.Tick(.02f);Physics.Simulate(.02f);}Check(car.SpeedKph<32&&car.Engine.Gear<high,"Native high-speed braking selects a lower gear");int low=car.Engine.Gear;car.commandedPedal=1;float roll=-1;for(int i=0;i<1500;i++){car.Tick(.02f);Physics.Simulate(.02f);if(car.SpeedKph>=100){roll=(i+1)*.02f;break;}}Check(roll>0,"Native 30-100 after braking: "+roll.ToString("F2")+" s; gears "+high+" to "+low);
   car.ResetAt(new Vector3(0,1,0),Quaternion.identity);car.ToggleTransmissionMode();Check(car.Engine.Mode==TransmissionMode.Manual,"Vehicle toggle selects manual");var definition=apex?sports.Build(4):garage.stageOne;car.InstallPowertrain(definition,apex?2:1);Check(car.Engine.Mode==TransmissionMode.Manual,"Installing hardware preserves manual preference");car.Recover();Check(car.Engine.Mode==TransmissionMode.Manual,"Vehicle recovery preserves manual preference");car.controlsLocked=true;Check(!car.ToggleTransmissionMode()&&!car.PaddleShift(1),"Garage/countdown lock blocks shift controls");car.controlsLocked=false;car.playerInput=false;Check(!car.ToggleTransmissionMode()&&!car.PaddleShift(1),"Map input lock blocks shift controls");UnityEngine.Object.DestroyImmediate(model);
  }
  File.WriteAllLines("Verification/Shifting/rules-physics.txt",lines);
 }catch(Exception e){lines.Add(e.ToString());File.WriteAllLines("Verification/Shifting/rules-physics.txt",lines);throw;}finally{Physics.simulationMode=old;}}
 public static void Release(){Run();HarborValeGreybox.Boost.Editor.BoostVerification.Run();HarborValeGreybox.Racing.Editor.TrialVerification.Run();Build();}
 public static void Build(){HarborValeGreybox.Launch.Editor.LaunchBuild.Prepare();Directory.CreateDirectory("Verification/Shifting/Windows");PlayerSettings.productName="Harbor Vale";PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=HarborValeGreybox.Launch.Editor.LaunchBuild.Scenes,locationPathName="Verification/Shifting/Windows/HarborVale.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});File.WriteAllText("Verification/Shifting/player-build.txt",r.summary.result+" | "+r.summary.totalSize+" bytes | errors "+r.summary.totalErrors);if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Smooth driving export failed");}
}
}
