using System;
using UnityEngine;
namespace HarborValeGreybox.Driving {
[Serializable] public class FinchDefinition {
 public string source,carId;public float massKg,wheelRadius,efficiency,finalDrive,reverseRatio,idleRpm,redlineRpm,softLimiterRpm,upshiftRpm,downshiftRpm,launchRpm,shiftSeconds,brakeDeceleration;
 public float[] ratios,rpm,torque;
 public float PeakKw(){float peak=0;for(int i=0;i<rpm.Length;i++){peak=Mathf.Max(peak,torque[i]*rpm[i]/9549.2966f);if(i>0){float a=(torque[i]-torque[i-1])/(rpm[i]-rpm[i-1]);if(a<0){float b=torque[i-1]-a*rpm[i-1],r=-b/(2*a);if(r>rpm[i-1]&&r<rpm[i])peak=Mathf.Max(peak,(a*r+b)*r/9549.2966f);}}}return peak;}
 public float SampleTorque(float speed){if(speed>=redlineRpm)return 0;speed=Mathf.Max(idleRpm,speed);for(int i=1;i<rpm.Length;i++)if(speed<=rpm[i])return Mathf.Lerp(torque[i-1],torque[i],Mathf.InverseLerp(rpm[i-1],rpm[i],speed));return torque[torque.Length-1];}
}
// Per-car transient gearbox/engine state. Authored net torque is applied once.
public sealed class FinchPowertrain {
 public readonly FinchDefinition definition;public int Gear{get;private set;}=1;public float Rpm{get;private set;}=900;public bool Shifting=>shiftRemaining>0;
 float shiftRemaining,gearHold;
 public FinchPowertrain(FinchDefinition d){definition=d;Reset();}
 public void Reset(){Gear=1;Rpm=definition.idleRpm;shiftRemaining=0;gearHold=0;}
 public float Step(float forwardSpeed,float drivenRpm,float throttle,bool reverse,float dt){
  shiftRemaining=Mathf.Max(0,shiftRemaining-dt);gearHold+=dt;
  if(Mathf.Abs(forwardSpeed)<1&&reverse!=(Gear<0)){Gear=reverse?-1:1;shiftRemaining=.15f;gearHold=0;}
  float ratio=Gear<0?definition.reverseRatio:definition.ratios[Gear-1];
  float roadRpm=Mathf.Abs(forwardSpeed)/definition.wheelRadius*60/(2*Mathf.PI);
  // Ground-speed coupling gives a forgiving launch; wheelspin can still raise RPM.
  float coupled=Mathf.Max(roadRpm,Mathf.Min(Mathf.Abs(drivenRpm),roadRpm+200))*ratio*definition.finalDrive;
  float target=Mathf.Max(definition.idleRpm,coupled,Mathf.Lerp(definition.idleRpm,definition.launchRpm,throttle)*(1-Mathf.Clamp01(Mathf.Abs(forwardSpeed)/8)));
  Rpm=Mathf.MoveTowards(Rpm,Mathf.Min(target,definition.redlineRpm+100),dt*9500);
  if(Gear>0&&gearHold>.7f&&!Shifting){
   int next=Gear;if(Rpm>definition.upshiftRpm&&Gear<definition.ratios.Length)next++;
   else if(Rpm<definition.downshiftRpm&&Gear>1&&roadRpm*definition.ratios[Gear-2]*definition.finalDrive<definition.upshiftRpm*.88f)next--;
   if(next!=Gear){Gear=next;shiftRemaining=definition.shiftSeconds;gearHold=0;return 0;}
  }
  if(Shifting||Rpm>=definition.redlineRpm)return 0;
  float limiter=1-Mathf.InverseLerp(definition.softLimiterRpm,definition.redlineRpm,Rpm);
  return definition.SampleTorque(Rpm)*throttle*ratio*definition.finalDrive*definition.efficiency*limiter*(Gear<0?-1:1);
 }
}
}


