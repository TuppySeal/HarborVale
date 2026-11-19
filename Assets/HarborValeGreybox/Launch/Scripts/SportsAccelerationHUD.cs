using UnityEngine;
using System.Collections.Generic;
using HarborValeGreybox.Workshop;
using HarborValeGreybox.SportsTest;
namespace HarborValeGreybox.Launch {
public enum AccelerationState{Idle,Armed,Running,Finished,Cancelled}
public sealed class AccelerationRun {
 public AccelerationState State{get;private set;}public float Seconds{get;private set;}public string Reason{get;private set;}float previous;
 public void Clear(){State=AccelerationState.Idle;Seconds=0;previous=0;Reason=null;}
 public bool Arm(float speed){if(speed>.5f)return false;State=AccelerationState.Armed;Seconds=0;previous=speed;Reason=null;return true;}
 public void Cancel(string reason){if(State==AccelerationState.Armed||State==AccelerationState.Running){State=AccelerationState.Cancelled;Reason=reason;}}
 public void Tick(float dt,float speed,float throttle){if(State==AccelerationState.Armed){if(speed>2&&throttle<=0){Cancel("Car rolled before launch. Stop and re-arm.");return;}if(throttle>0){State=AccelerationState.Running;previous=speed;}return;}if(State!=AccelerationState.Running)return;dt=Mathf.Max(0,dt);if(speed>=100){float fraction=speed>previous?Mathf.Clamp01((100-previous)/(speed-previous)):1;Seconds+=dt*fraction;State=AccelerationState.Finished;}else Seconds+=dt;previous=speed;if(Seconds>60)Cancel("Test timed out. Stop and try again.");}
}
public sealed class SportsAccelerationHUD:MonoBehaviour {
 readonly Dictionary<string,float> buildBests=new Dictionary<string,float>();string configuration;
 string CurrentKey=>UnityEngine.Object.FindAnyObjectByType<ExpandedWorkshop>()?.Signature??("legacy_"+Test.Step);
 bool IsLegacyBuild {get{var shop=UnityEngine.Object.FindAnyObjectByType<ExpandedWorkshop>();return shop==null||CurrentKey==JsonUtility.ToJson(shop.Catalog.FromLegacy(Test.Step>=3?2:Test.Step>=1?1:0,Test.Step>=2?1:0,Test.Step>=4?1:0));}}
 public readonly AccelerationRun Run=new AccelerationRun();public float[] Best{get;}={-1,-1,-1,-1,-1};public ApexTestDrive Test{get;private set;}int step,reset,observedReset=-1;float air,stoppedTime;bool recorded;HarborValeGreybox.Driving.FinchPowertrain engine;
 public bool ReadyToArm=>Test!=null&&Test.Ready&&Test.Car.SpeedKph<=.5f&&Test.Car.GroundedWheels>=2&&Test.Car.playerInput&&!Test.Car.controlsLocked&&stoppedTime>=.35f;
 public bool Arm(){if(!ReadyToArm)return false;if(!Run.Arm(Test.Car.SpeedKph))return false;configuration=CurrentKey;step=Test.Step;reset=Test.Car.ResetSequence;engine=Test.Car.Engine;air=0;recorded=false;return true;}
 void FixedUpdate(){if(Test==null)Test=UnityEngine.Object.FindAnyObjectByType<ApexTestDrive>();if(Test==null||!Test.Ready)return;var car=Test.Car;
  if(observedReset!=car.ResetSequence){observedReset=car.ResetSequence;stoppedTime=0;}
  stoppedTime=car.SpeedKph<=.5f&&car.GroundedWheels>=2&&car.playerInput&&!car.controlsLocked?stoppedTime+Time.fixedDeltaTime:0;
  if(Run.State==AccelerationState.Armed||Run.State==AccelerationState.Running){if(step!=Test.Step||reset!=car.ResetSequence||!ReferenceEquals(engine,car.Engine))Run.Cancel("Build changed or car recovered. Stop and re-arm.");if(!car.playerInput||car.controlsLocked||car.ForwardSpeed<-.5f)Run.Cancel("Driving paused or reversed. Stop and re-arm.");air=car.GroundedWheels<2?air+Time.fixedDeltaTime:0;if(air>.3f)Run.Cancel("Airborne test cancelled. Try a clear road.");}
  Run.Tick(Time.fixedDeltaTime,car.SpeedKph,car.Throttle);if(Run.State==AccelerationState.Finished&&!recorded){if(IsLegacyBuild&&(Best[step]<0||Run.Seconds<Best[step]))Best[step]=Run.Seconds;if(!buildBests.TryGetValue(configuration,out float previousBest)||Run.Seconds<previousBest)buildBests[configuration]=Run.Seconds;recorded=true;}
 }
 void OnGUI(){if(Test==null||!Test.Ready||Test.Car.controlsLocked)return;float x=Screen.width-302;GUI.Box(new Rect(x,16,286,226),"APEX | 0–100 km/h TEST");var number=new GUIStyle(GUI.skin.label){fontSize=32,fontStyle=FontStyle.Bold};GUI.Label(new Rect(x+14,42,260,48),Run.Seconds.ToString("F2")+" s",number);string status=Run.State==AccelerationState.Armed?"Armed — accelerate when ready":Run.State==AccelerationState.Running?"Timing…":Run.State==AccelerationState.Finished?"Complete":Run.State==AccelerationState.Cancelled?"Cancelled":"Stop, arm, then accelerate";GUI.Label(new Rect(x+14,94,260,25),status);float best=buildBests.TryGetValue(CurrentKey,out var bestForBuild)?bestForBuild:-1;GUI.Label(new Rect(x+14,119,260,24),"Best with these parts: "+(best<0?"—":best.ToString("F2")+" s"));GUI.enabled=ReadyToArm;if(GUI.Button(new Rect(x+14,150,123,30),"Arm test"))Arm();GUI.enabled=true;if(GUI.Button(new Rect(x+146,150,123,30),"Clear timer"))Run.Clear();GUI.Label(new Rect(x+14,187,260,34),Run.State==AccelerationState.Cancelled?Run.Reason:"Session best only. Road slope affects results.",new GUIStyle(GUI.skin.label){wordWrap=true});}
}
}
