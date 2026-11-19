using System;
namespace HarborValeGreybox.Workshop {
// Four-stroke firing harmonics with cylinder variation, intake air and muffled exhaust body.
public sealed class EngineSoundDsp {
 readonly double rate,control,bodyRate,lowRate,airRate;readonly double[] weights=new double[11];double phase,load,body,low,air;int cylinder;uint seed=0x916c4ed1u;
 public EngineSoundDsp(int sampleRate){rate=Math.Max(8000,sampleRate);control=1-Math.Exp(-1/(rate*.04));bodyRate=1-Math.Exp(-2*Math.PI*650/rate);lowRate=1-Math.Exp(-2*Math.PI*500/rate);airRate=1-Math.Exp(-2*Math.PI*Math.Min(3200,rate*.35)/rate);for(int h=1;h<weights.Length;h++)weights[h]=1/Math.Pow(h,1.12);}
 public float Sample(float firingHz,float gain,float targetLoad,bool apex,int exhaustStage){double frequency=Math.Max(15,Math.Min(600,firingHz));load+=(Math.Max(0,Math.Min(1,targetLoad))-load)*control;phase+=frequency/rate;if(phase>=1){phase-=1;cylinder=(cylinder+1)%4;}seed^=seed<<13;seed^=seed>>17;seed^=seed<<5;double n=seed/2147483648.0-1;low+=(n-low)*lowRate;air+=(n-air)*airRate;double brightness=3+load*14+Math.Max(0,Math.Min(2,exhaustStage))*1.5,raw=0;for(int h=1;h<weights.Length&&h*frequency<rate*.42;h++){double envelope=brightness/(brightness+h*h);raw+=weights[h]*envelope*Math.Sin(2*Math.PI*phase*h+.14*h);}
  double imbalance=cylinder==0?1:cylinder==1?.965:cylinder==2?1.025:.985;raw*=imbalance;body+=(raw-body)*bodyRate;double voice=body*(apex?1.05:1.12)+raw*(.28+.18*load+Math.Max(0,Math.Min(2,exhaustStage))*.035)+(air-low)*(.035+.19*load)+low*.025;return(float)(gain*voice*1.25);
 }
}
}
