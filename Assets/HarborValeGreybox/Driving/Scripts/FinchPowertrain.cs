using System;
using UnityEngine;
namespace HarborValeGreybox.Driving {
[Serializable] public class FinchDefinition {
 public string source,carId,aspiration;public float massKg,wheelRadius,efficiency,finalDrive,reverseRatio,idleRpm,redlineRpm,softLimiterRpm,upshiftRpm,downshiftRpm,launchRpm,shiftSeconds,brakeDeceleration;
 public float[] ratios,rpm,torque,offBoostTorque;public float boostThresholdRpm,fullBoostRpm,spoolRise,spoolFall,displayBoostBar;
 public bool IsTurbo=>offBoostTorque!=null&&rpm!=null&&offBoostTorque.Length==rpm.Length&&spoolRise>0&&spoolFall>0;
 public float PeakKw(){float peak=0;for(int i=0;i<rpm.Length;i++){peak=Mathf.Max(peak,torque[i]*rpm[i]/9549.2966f);if(i>0){float a=(torque[i]-torque[i-1])/(rpm[i]-rpm[i-1]);if(a<0){float b=torque[i-1]-a*rpm[i-1],r=-b/(2*a);if(r>rpm[i-1]&&r<rpm[i])peak=Mathf.Max(peak,(a*r+b)*r/9549.2966f);}}}return peak;}
 public float SampleTorque(float speed)=>Sample(torque,speed);
 public float SampleOffBoost(float speed)=>Sample(IsTurbo?offBoostTorque:torque,speed);
 float Sample(float[] values,float speed){if(speed>=redlineRpm)return 0;speed=Mathf.Max(idleRpm,speed);for(int i=1;i<rpm.Length;i++)if(speed<=rpm[i])return Mathf.Lerp(values[i-1],values[i],Mathf.InverseLerp(rpm[i-1],rpm[i],speed));return values[values.Length-1];}
}
public enum TransmissionMode { Automatic, Manual }
public sealed class FinchPowertrain {
 public readonly FinchDefinition definition;
 public int Gear{get;private set;}=1;
 public float Rpm{get;private set;}=900;
 public bool Shifting=>shiftRemaining>0;
 public TransmissionMode Mode{get;private set;}
 public string ShiftNote{get;private set;}
 public float TurboAvailability{get;private set;}
 public float NetTorque{get;private set;}
 public float ClutchEngagement=>!Shifting?1:Mathf.SmoothStep(0,1,1-shiftRemaining/Mathf.Max(.01f,shiftDuration*.3f));
 // Gauge pressure never multiplies the authored net torque curves.
 public float BoostBar=>definition.IsTurbo?TurboAvailability*definition.displayBoostBar*Mathf.InverseLerp(definition.boostThresholdRpm,definition.fullBoostRpm,Rpm):0;
 float shiftRemaining,shiftDuration,gearHold;bool wasMoving,kickdownArmed=true;
 public FinchPowertrain(FinchDefinition d){definition=d;Reset();}
 public void Reset(){Gear=1;Rpm=definition.idleRpm;shiftRemaining=shiftDuration=0;gearHold=0;wasMoving=false;kickdownArmed=true;TurboAvailability=NetTorque=0;ShiftNote=null;}
 public void SetMode(TransmissionMode mode){Mode=mode;kickdownArmed=true;ShiftNote=null;}
 public float ProjectedRpm(float speed,int gear)=>Mathf.Abs(speed)/definition.wheelRadius*60/(2*Mathf.PI)*definition.ratios[gear-1]*definition.finalDrive;
 void BeginShift(int next,float seconds){Gear=next;shiftDuration=Mathf.Max(.05f,seconds);shiftRemaining=shiftDuration;gearHold=0;}
 public bool RequestShift(int direction,float speed){
  if(direction!=1&&direction!=-1){ShiftNote="Choose up or down.";return false;}
  if(Gear<1){ShiftNote="Paddles unavailable in reverse.";return false;}
  if(Shifting){ShiftNote="Shift in progress.";return false;}
  int next=Gear+direction;
  if(next<1||next>definition.ratios.Length){ShiftNote=next<1?"Already in first gear.":"Already in top gear.";return false;}
  if(direction<0&&ProjectedRpm(speed,next)>definition.redlineRpm*.94f){ShiftNote="Downshift blocked: RPM too high.";return false;}
  BeginShift(next,definition.shiftSeconds);Mode=TransmissionMode.Manual;ShiftNote=null;return true;
 }
 int AutomaticDownshift(float speed,float throttle,float braking,bool kickdown){
  float currentRpm=ProjectedRpm(speed,Gear);
  if(throttle>.65f&&(kickdown||currentRpm<definition.downshiftRpm)){
   // Pick useful wheel torque, with RPM margin below the automatic upshift point.
   // This margin prevents immediately undoing the previous upshift.
   int best=Gear;float bestTorque=definition.SampleTorque(currentRpm)*definition.ratios[Gear-1];
   for(int gear=Gear-1;gear>=1;gear--){float rpm=ProjectedRpm(speed,gear);if(rpm>definition.upshiftRpm*.88f)continue;float torque=definition.SampleTorque(rpm)*definition.ratios[gear-1];if(torque>bestTorque*1.15f){best=gear;bestTorque=torque;}}
   return best;
  }
  float threshold=definition.downshiftRpm+(braking>.05f?500:0);
  if(currentRpm>=threshold)return Gear;
  float target=definition.downshiftRpm+500;
  int matched=Gear;float cost=Mathf.Abs(currentRpm-target);
  for(int gear=Gear-1;gear>=1;gear--){float rpm=ProjectedRpm(speed,gear);if(rpm>definition.upshiftRpm*.85f)continue;float nextCost=Mathf.Abs(rpm-target);if(nextCost<cost){cost=nextCost;matched=gear;}}
  return matched;
 }
 public float Step(float forwardSpeed,float drivenRpm,float throttle,bool reverse,float dt,bool loaded=true,float braking=0){
  dt=Mathf.Max(0,dt);throttle=Mathf.Clamp01(throttle);shiftRemaining=Mathf.Max(0,shiftRemaining-dt);gearHold+=dt;if(throttle<.3f)kickdownArmed=true;
  if(Mathf.Abs(forwardSpeed)<1&&reverse!=(Gear<0))BeginShift(reverse?-1:1,.15f);
  // Paddle-style standstill assist avoids pulling away in a high gear, without a clutch simulation.
  if(Mathf.Abs(forwardSpeed)>1.5f)wasMoving=true;
  if(Gear>1&&Mathf.Abs(forwardSpeed)<.75f&&!Shifting&&(Mode==TransmissionMode.Automatic||wasMoving))BeginShift(1,definition.shiftSeconds);
  if(Mathf.Abs(forwardSpeed)<.75f&&!Shifting)wasMoving=false;
  if(Mode==TransmissionMode.Automatic&&Gear>0&&loaded&&!Shifting){
   float roadRpm=ProjectedRpm(forwardSpeed,Gear);
   bool ready=gearHold>=definition.shiftSeconds+.20f||roadRpm<definition.idleRpm*1.15f;
   if(ready){int next=Gear;bool kickdown=kickdownArmed&&throttle>.65f;if(throttle>.65f)kickdownArmed=false;
    if(throttle>.08f&&(roadRpm>=definition.upshiftRpm||(Rpm>=definition.upshiftRpm&&roadRpm>=definition.upshiftRpm*.8f))&&Gear<definition.ratios.Length)next++;
    else if(Gear>1)next=AutomaticDownshift(forwardSpeed,throttle,braking,kickdown);
    if(next!=Gear)BeginShift(next,definition.shiftSeconds);
   }
  }
  // Recompute after selecting the gear; RPM and wheel torque always use the same ratio.
  float ratio=Gear<0?definition.reverseRatio:definition.ratios[Gear-1];
  float roadWheelRpm=Mathf.Abs(forwardSpeed)/definition.wheelRadius*60/(2*Mathf.PI);
  float coupled=Mathf.Max(roadWheelRpm,Mathf.Min(Mathf.Abs(drivenRpm),roadWheelRpm+200))*ratio*definition.finalDrive;
  float launch=Mathf.Lerp(definition.idleRpm,definition.launchRpm,throttle)*(1-Mathf.Clamp01(Mathf.Abs(forwardSpeed)/8));
  float target=Mathf.Max(definition.idleRpm,coupled,launch);
  Rpm=Mathf.MoveTowards(Rpm,Mathf.Min(target,definition.redlineRpm+100),dt*9500);
  bool cut=ClutchEngagement<=0||Rpm>=definition.redlineRpm;
  NetTorque=ResolveTorque(Rpm,throttle,loaded,cut,dt);
  float limiter=1-Mathf.InverseLerp(definition.softLimiterRpm,definition.redlineRpm,Rpm);
  float torque=NetTorque*ratio*definition.finalDrive*definition.efficiency*limiter*ClutchEngagement*(Gear<0?-1:1);
  if(throttle<.02f&&!Shifting&&loaded&&Mathf.Abs(forwardSpeed)>1){
   float brakingTorque=definition.SampleOffBoost(Rpm)*.08f*ratio*definition.finalDrive*definition.efficiency;
   torque-=Mathf.Sign(forwardSpeed)*brakingTorque;
  }
  return torque;
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
