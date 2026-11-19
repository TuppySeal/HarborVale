using System;
namespace HarborValeGreybox.Workshop {
// Continuous phases, bounded gain ramps and original synthesis shared by runtime and PCM checks.
public sealed class DrivingAudioDsp {
 readonly double rate,smooth,freqSmooth,masterSmooth,dc;public readonly TurboSoundDsp turbo;readonly TireSoundDsp tires;readonly EngineSoundDsp combustion;
 double frequency=30,engine,turboGain,blower,master,blowerPhase,lowNoise,popAir,lastInput,lastOutput,popTime=-1,popDuration,popGain,popFade=1;bool cancelPop;uint random=0x17a9b351;
 public DrivingAudioDsp(int sampleRate){rate=Math.Max(8000,sampleRate);smooth=1-Math.Exp(-1/(rate*.025));freqSmooth=1-Math.Exp(-1/(rate*.012));masterSmooth=1-Math.Exp(-1/(rate*.018));dc=Math.Exp(-2*Math.PI*18/rate);turbo=new TurboSoundDsp(sampleRate);tires=new TireSoundDsp(sampleRate);combustion=new EngineSoundDsp(sampleRate);}
 double Noise(){random^=random<<13;random^=random>>17;random^=random<<5;return random/2147483648.0-1;}
 public void Pop(float gain,float seconds){popGain=Math.Max(0,Math.Min(.3,gain));popDuration=Math.Max(.03,seconds);popTime=0;popFade=1;cancelPop=false;}
 public void CancelPops(){cancelPop=true;}
 public float Sample(float hz,float volume,float boost,float whine,float slip,float rotor,float size,float targetMaster,bool apex,float tireSpeed=50,float cornering=.7f,float engineLoad=.5f,int exhaustStage=0){
  frequency+=(Math.Max(15,Math.Min(300,hz))-frequency)*freqSmooth;engine+=(Math.Max(0,Math.Min(.2,volume))-engine)*smooth;turboGain+=(Math.Max(0,Math.Min(.2,boost))-turboGain)*smooth;blower+=(Math.Max(0,Math.Min(.15,whine))-blower)*smooth;master+=(Math.Max(0,Math.Min(1,targetMaster))-master)*masterSmooth;
  blowerPhase=(blowerPhase+Math.Min(rate*.4,1200+frequency*7)/rate)%1;double n=Noise();lowNoise+=(n-lowNoise)*(1-Math.Exp(-2*Math.PI*380/rate));popAir+=(n-popAir)*(1-Math.Exp(-2*Math.PI*Math.Min(3400,rate*.4)/rate));
  double mixed=combustion.Sample((float)frequency,(float)engine,engineLoad,apex,exhaustStage);
  double blowerFrequency=Math.Min(rate*.4,1200+frequency*7);mixed+=blower*(.76*Math.Sin(blowerPhase*2*Math.PI)+(blowerFrequency*2<rate*.42?.10*Math.Sin(blowerPhase*4*Math.PI):0)+.035*lowNoise);mixed+=tires.Sample(slip,tireSpeed,cornering);mixed+=turbo.Sample((float)turboGain,rotor,size);
  if(popTime>=0){double t=popTime;popTime+=1/rate;if(cancelPop)popFade*=Math.Exp(-1/(rate*.006));double envelope=popFade*(1-Math.Exp(-t/.0015))*Math.Pow(Math.Max(0,1-t/popDuration),2);mixed+=popGain*envelope*(.65*(popAir-lowNoise)*(.75+.25*Math.Cos(2*Math.PI*30*t))+.55*Math.Sin(2*Math.PI*(85*t+1.8*(1-Math.Exp(-t*15)))));if(t>=popDuration||popFade<.0001)popTime=-1;}
  double filtered=mixed-lastInput+dc*lastOutput;lastInput=mixed;lastOutput=filtered;return (float)(.72*Math.Tanh(filtered*master/.72));
 }
}
}
