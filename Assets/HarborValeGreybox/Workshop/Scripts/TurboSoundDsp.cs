using System;
namespace HarborValeGreybox.Workshop {
public enum TurboReleaseMode { BlowOffValve=0, Flutter=1 }
// Original sample-rate-aware synthesis. No borrowed recordings or random calls into Unity's audio thread.
public sealed class TurboSoundDsp {
 readonly double sampleRate;uint random=0x6d2b79f5;double rotor,level,phase,low,air,resonance,releaseTime=-1,releaseStrength,releaseScale=1;int mode;
 public TurboSoundDsp(int rate){sampleRate=Math.Max(8000,rate);}
 public void Release(TurboReleaseMode choice,float pressure,float size){mode=(int)choice;releaseStrength=Math.Min(1,Math.Sqrt(Math.Max(0,pressure)/1.5));releaseScale=Math.Max(.75,Math.Min(1.25,size));releaseTime=0;}
 public void Cancel(){releaseTime=-1;level=0;}
 double Noise(){random^=random<<13;random^=random>>17;random^=random<<5;return random/2147483648.0-1;}
 public float Sample(float gain,float speed,float size){
  double dt=1/sampleRate;level+=(Math.Max(0,gain)-level)*(1-Math.Exp(-dt/.035));rotor+=(Math.Max(0,Math.Min(1,speed))-rotor)*(1-Math.Exp(-dt/.075));double n=Noise();low+=(n-low)*(1-Math.Exp(-dt*2*Math.PI*700));air+=(n-air)*(1-Math.Exp(-dt*2*Math.PI*6200));double band=air-low;
  double pitch=(1500+4500*rotor)*Math.Max(.75,Math.Min(1.25,size));pitch=Math.Min(sampleRate*.4,pitch);phase=(phase+pitch/sampleRate)%1;
  // Air movement dominates; a small modulated harmonic provides the compressor edge without a pure sine siren.
  resonance+=(n-resonance)*.0015;double whistle=Math.Sin(phase*Math.PI*2+resonance*.7)+.12*Math.Sin(phase*Math.PI*4);
  double output=level*(band*.85+whistle*(.07+.10*rotor));
  if(releaseTime>=0){double t=releaseTime;releaseTime+=dt;double envelope=0,tone=0;
   if(mode==0){double length=.32+.30*releaseStrength;envelope=(1-Math.Exp(-t/.006))*Math.Exp(-t/(.09+.10*releaseStrength));tone=.045*Math.Sin(2*Math.PI*(3000*t-1150*t*t)*releaseScale);if(t>length)releaseTime=-1;}
   else {double cursor=0;int pulses=4+(int)(releaseStrength*3);double length=pulses*.055+.014*pulses*(pulses-1)/2;for(int i=0;i<pulses;i++){double period=.055+.014*i;double local=t-cursor;if(local>=0&&local<period){double p=local/(period*.72);if(p<1)envelope=Math.Pow(Math.Sin(p*Math.PI),1.6)*Math.Pow(.76,i);tone=.14*Math.Sin(2*Math.PI*(1100*t-300*t*t)*releaseScale);break;}cursor+=period;}if(t>length)releaseTime=-1;}
   output+=releaseStrength*.50*envelope*(band+tone);
  }
  return (float)Math.Max(-.7,Math.Min(.7,output));
 }
 public static float[] Preview(TurboReleaseMode choice,int rate){var synth=new TurboSoundDsp(rate);var samples=new float[rate*3];for(int i=0;i<samples.Length;i++){float t=(float)i/rate;if(i==rate)synth.Release(choice,1.2f,1);samples[i]=synth.Sample(t<1?.075f*t:0,t<1?t:Math.Max(0,1-(t-1)*1.5f),1);}return samples;}
}
public sealed class TurboReleaseGate {
 bool armed;float previous,peak;
 public void Reset(){armed=false;previous=peak=0;}
 public float Step(bool turbo,bool allowed,float throttle,float pressure){
  if(!turbo||!allowed){Reset();return 0;}
  float result=0;if(pressure<.04f){armed=false;peak=0;}if(throttle>.4f&&pressure>.08f){armed=true;peak=pressure;}
  if(armed&&previous>.4f&&throttle<.15f){if(pressure>.04f)result=Math.Max(pressure,peak);armed=false;peak=0;}
  if(throttle<.15f&&pressure<.04f){armed=false;peak=0;}previous=throttle;return result;
 }
}
}
