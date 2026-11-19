using System;
namespace HarborValeGreybox.Workshop {
// Continuous phases, bounded gain ramps and original synthesis shared by runtime and PCM checks.
public sealed class DrivingAudioDsp {
 readonly double rate,smooth,freqSmooth,masterSmooth,dc;public readonly TurboSoundDsp turbo;
 double frequency=30,engine,turboGain,blower,tire,master,phase,blowerPhase,tirePhase,lowNoise,lastInput,lastOutput,popTime=-1,popDuration,popGain,popFade=1;bool cancelPop;uint random=0x17a9b351;
 public DrivingAudioDsp(int sampleRate){rate=Math.Max(8000,sampleRate);smooth=1-Math.Exp(-1/(rate*.025));freqSmooth=1-Math.Exp(-1/(rate*.012));masterSmooth=1-Math.Exp(-1/(rate*.018));dc=Math.Exp(-2*Math.PI*18/rate);turbo=new TurboSoundDsp(sampleRate);}
 double Noise(){random^=random<<13;random^=random>>17;random^=random<<5;return random/2147483648.0-1;}
 public void Pop(float gain,float seconds){popGain=Math.Max(0,Math.Min(.3,gain));popDuration=Math.Max(.03,seconds);popTime=0;popFade=1;cancelPop=false;}
 public void CancelPops(){cancelPop=true;}
 public float Sample(float hz,float volume,float boost,float whine,float slip,float rotor,float size,float targetMaster,bool apex){
  frequency+=(Math.Max(15,Math.Min(300,hz))-frequency)*freqSmooth;engine+=(Math.Max(0,Math.Min(.2,volume))-engine)*smooth;turboGain+=(Math.Max(0,Math.Min(.2,boost))-turboGain)*smooth;blower+=(Math.Max(0,Math.Min(.15,whine))-blower)*smooth;tire+=(Math.Max(0,Math.Min(.15,slip))-tire)*smooth;master+=(Math.Max(0,Math.Min(1,targetMaster))-master)*masterSmooth;
  phase=(phase+frequency/rate)%1;blowerPhase=(blowerPhase+Math.Min(rate*.4,1200+frequency*7)/rate)%1;tirePhase=(tirePhase+Math.Min(rate*.4,1300+frequency*.8)/rate)%1;double n=Noise();lowNoise+=(n-lowNoise)*(1-Math.Exp(-2*Math.PI*380/rate));double x=phase*2*Math.PI;
  double mixed=engine*(.78*Math.Sin(x)+.25*Math.Sin(x*2)+(apex?.13:.08)*Math.Sin(x*3)+.035*lowNoise);
  mixed+=blower*(Math.Sin(blowerPhase*2*Math.PI)+.12*Math.Sin(blowerPhase*4*Math.PI));mixed+=tire*((n-lowNoise)*.60+Math.Sin(tirePhase*2*Math.PI)*(.40+.08*lowNoise));mixed+=turbo.Sample((float)turboGain,rotor,size);
  if(popTime>=0){double t=popTime;popTime+=1/rate;if(cancelPop)popFade*=Math.Exp(-1/(rate*.006));double envelope=popFade*(1-Math.Exp(-t/.0015))*Math.Pow(Math.Max(0,1-t/popDuration),2);mixed+=popGain*envelope*(.55*(n-lowNoise)+.55*Math.Sin(2*Math.PI*(90*t+2.5*(1-Math.Exp(-t*12)))));if(t>=popDuration||popFade<.0001)popTime=-1;}
  double filtered=mixed-lastInput+dc*lastOutput;lastInput=mixed;lastOutput=filtered;return (float)(.72*Math.Tanh(filtered*master/.72));
 }
}
}
