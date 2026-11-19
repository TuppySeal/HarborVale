using UnityEngine;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Shifting.Editor {
public sealed class LegacyFinchPowertrain {
 public readonly FinchDefinition definition;public int Gear{get;private set;}=1;public float Rpm{get;private set;}=900;public bool Shifting=>shiftRemaining>0;public float TurboAvailability{get;private set;}public float NetTorque{get;private set;}
 // Pressure is a fictional gauge reading only; never multiplied into drive torque.
 public float BoostBar=>definition.IsTurbo?TurboAvailability*definition.displayBoostBar*Mathf.InverseLerp(definition.boostThresholdRpm,definition.fullBoostRpm,Rpm):0;
 float shiftRemaining,gearHold;
 public LegacyFinchPowertrain(FinchDefinition d){definition=d;Reset();}
 public void Reset(){Gear=1;Rpm=definition.idleRpm;shiftRemaining=0;gearHold=0;TurboAvailability=0;NetTorque=0;}
 public float Step(float forwardSpeed,float drivenRpm,float throttle,bool reverse,float dt,bool loaded=true){
  shiftRemaining=Mathf.Max(0,shiftRemaining-dt);gearHold+=dt;
  if(Mathf.Abs(forwardSpeed)<1&&reverse!=(Gear<0)){Gear=reverse?-1:1;shiftRemaining=.15f;gearHold=0;}
  float ratio=Gear<0?definition.reverseRatio:definition.ratios[Gear-1],roadRpm=Mathf.Abs(forwardSpeed)/definition.wheelRadius*60/(2*Mathf.PI);
  float coupled=Mathf.Max(roadRpm,Mathf.Min(Mathf.Abs(drivenRpm),roadRpm+200))*ratio*definition.finalDrive;
  float target=Mathf.Max(definition.idleRpm,coupled,Mathf.Lerp(definition.idleRpm,definition.launchRpm,throttle)*(1-Mathf.Clamp01(Mathf.Abs(forwardSpeed)/8)));
  Rpm=Mathf.MoveTowards(Rpm,Mathf.Min(target,definition.redlineRpm+100),dt*9500);
  if(Gear>0&&gearHold>.7f&&!Shifting){int next=Gear;if(Rpm>definition.upshiftRpm&&Gear<definition.ratios.Length)next++;else if(Rpm<definition.downshiftRpm&&Gear>1&&roadRpm*definition.ratios[Gear-2]*definition.finalDrive<definition.upshiftRpm*.88f)next--;if(next!=Gear){Gear=next;shiftRemaining=definition.shiftSeconds;gearHold=0;}}
  NetTorque=ResolveTorque(Rpm,throttle,loaded,Shifting||Rpm>=definition.redlineRpm,dt);
  if(Shifting||Rpm>=definition.redlineRpm)return 0;
  float limiter=1-Mathf.InverseLerp(definition.softLimiterRpm,definition.redlineRpm,Rpm);
  return NetTorque*ratio*definition.finalDrive*definition.efficiency*limiter*(Gear<0?-1:1);
 }
 public float ResolveTorque(float rpm,float throttle,bool loaded,bool cut,float dt){
  throttle=Mathf.Clamp01(throttle);if(!definition.IsTurbo){TurboAvailability=0;return cut?0:definition.SampleTorque(rpm)*throttle;}
  float target=loaded&&!cut&&throttle>0&&rpm>definition.boostThresholdRpm?throttle:0;
  float tau=target>TurboAvailability?definition.spoolRise:definition.spoolFall;
  TurboAvailability=Mathf.Lerp(TurboAvailability,target,1-Mathf.Exp(-Mathf.Max(0,dt)/tau));
  float off=definition.SampleOffBoost(rpm),steady=definition.SampleTorque(rpm);
  return cut?0:throttle*(off+TurboAvailability*(steady-off));
 }
}
}
