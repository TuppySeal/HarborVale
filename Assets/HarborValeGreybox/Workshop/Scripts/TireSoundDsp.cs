using System;
namespace HarborValeGreybox.Workshop {
// Original rubber-friction synthesis: band-limited texture and gently wandering resonances.
public sealed class TireSoundDsp {
 readonly double rate,attack,release,control,noiseLow,noiseHigh,gritRate,outputRate;
 double gain,speed,corner,phase,second,third,wobble,low,high,grit,output;uint seed=0x35d129abu;
 public TireSoundDsp(int sampleRate){rate=Math.Max(8000,sampleRate);attack=1-Math.Exp(-1/(rate*.035));release=1-Math.Exp(-1/(rate*.12));control=1-Math.Exp(-1/(rate*.06));noiseLow=1-Math.Exp(-2*Math.PI*500/rate);noiseHigh=1-Math.Exp(-2*Math.PI*Math.Min(3200,rate*.35)/rate);gritRate=1-Math.Exp(-2*Math.PI*14/rate);outputRate=1-Math.Exp(-2*Math.PI*Math.Min(3600,rate*.4)/rate);}
 public float Sample(float amount,float speedKph,float sideways){double target=Math.Max(0,Math.Min(.15,amount));gain+=(target-gain)*(target>gain?attack:release);speed+=(Math.Max(0,Math.Min(300,speedKph))-speed)*control;corner+=(Math.Max(0,Math.Min(1,sideways))-corner)*control;
  seed^=seed<<13;seed^=seed>>17;seed^=seed<<5;double n=seed/2147483648.0-1;low+=(n-low)*noiseLow;high+=(n-high)*noiseHigh;grit+=(n-grit)*gritRate;
  wobble=(wobble+(6.3+speed*.025)/rate)%1;double wander=Math.Sin(2*Math.PI*wobble);double frequency=Math.Min(rate*.24,820+speed*3.1+Math.Min(1,gain/.045)*230+corner*120+22*wander+45*grit);
  phase=(phase+frequency/rate)%1;second=(second+frequency*1.271/rate)%1;third=(third+frequency*1.613/rate)%1;
  double texture=(high-low)*.90,ring=.30*Math.Sin(2*Math.PI*phase+.20*wander)+.16*Math.Sin(2*Math.PI*second+.25*grit)+.07*Math.Sin(2*Math.PI*third);double rubber=(texture+ring)*(.86+.10*wander+.04*grit);output+=(Math.Tanh(rubber*.9)-output)*outputRate;return(float)(gain*output);
 }
}
}
