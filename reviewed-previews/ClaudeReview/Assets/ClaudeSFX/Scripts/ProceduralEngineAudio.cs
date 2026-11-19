using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace ClaudeSFX {
[Serializable]public struct EffectTelemetry {public float rpm,load,boostBar,slip,speed;public bool grounded,turbo,supercharger,downpipe,superchargerPreview;}
public class ProceduralEngineAudio:MonoBehaviour {
 public float TurboGain{get;private set;}public float SuperGain{get;private set;}public float SquealGain{get;private set;}public EffectTelemetry State{get;private set;}
 volatile float voice=0;
 public void SetVoice(int car){voice=car==0?0:1;}
 volatile float hz=35,turboGain,superGain,squealGain;volatile int popSamples;double phase,turboPhase,superPhase,tirePhase;int rate;AudioSource source;AudioClip carrier;
 readonly object sync=new object();readonly List<float> captured=new List<float>();bool capturing;int channels=2;
 public void Apply(EffectTelemetry state){State=state;hz=Mathf.Clamp(state.rpm,600,7500)/30f;TurboGain=state.turbo&&state.grounded&&state.load>.02f?Mathf.Clamp01(state.boostBar)*Mathf.Clamp01(state.load)*.045f:0;SuperGain=state.supercharger&&state.superchargerPreview?Mathf.Clamp01(state.load)*Mathf.Clamp01(state.rpm/6500)*.06f:0;SquealGain=state.grounded&&state.speed>3?Mathf.InverseLerp(.12f,.8f,Mathf.Abs(state.slip))*.075f:0;turboGain=TurboGain;superGain=SuperGain;squealGain=SquealGain;}
 void Start(){rate=AudioSettings.outputSampleRate;source=gameObject.AddComponent<AudioSource>();source.spatialBlend=0;source.volume=1;source.loop=true;source.dopplerLevel=0;carrier=AudioClip.Create("Silent carrier for procedural DSP",rate,1,rate,false);source.clip=carrier;source.Play();}
 public bool Pop(){if(!State.downpipe||State.rpm<2500||State.load>.15f)return false;popSamples=rate/5;return true;}
 void OnAudioFilterRead(float[] data,int count){if(rate<=0)return;double step=1.0/rate;for(int i=0;i<data.Length;i+=count){phase=(phase+hz*step)%1;turboPhase=(turboPhase+(2000+hz*4)*step)%1;superPhase=(superPhase+(1200+hz*9)*step)%1;tirePhase=(tirePhase+1550*step)%1;double v=.085*(Math.Sin(phase*Math.PI*2)+.45*(1+voice*.4)*Math.Sin(phase*Math.PI*4)+.2*Math.Sin(phase*Math.PI*6));v+=turboGain*Math.Sin(turboPhase*Math.PI*2)+superGain*(Math.Sin(superPhase*Math.PI*2)+.2*Math.Sin(superPhase*Math.PI*4))+squealGain*(Math.Sin(tirePhase*Math.PI*2)+.35*Math.Sin(tirePhase*Math.PI*3));if(popSamples>0){double envelope=(double)popSamples/(rate/5);v+=.22*envelope*envelope*(Math.Sin(phase*Math.PI*7)+.4*Math.Sin(tirePhase*Math.PI*9));popSamples--;}float value=(float)Math.Max(-.9,Math.Min(.9,v));for(int c=0;c<count;c++)data[i+c]=value;}
  lock(sync){if(capturing){channels=count;captured.AddRange(data);}}
 }
 public void BeginCapture(){lock(sync){captured.Clear();popSamples=0;capturing=true;}}
 public string SaveCapture(string file){float[] samples;lock(sync){capturing=false;samples=captured.ToArray();}double energy=0;float peak=0;using(var stream=new BinaryWriter(File.Create(file))){int bytes=samples.Length*2;stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));stream.Write(bytes+36);stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));stream.Write(16);stream.Write((short)1);stream.Write((short)channels);stream.Write(rate);stream.Write(rate*channels*2);stream.Write((short)(channels*2));stream.Write((short)16);stream.Write(System.Text.Encoding.ASCII.GetBytes("data"));stream.Write(bytes);foreach(float v in samples){energy+=v*v;peak=Math.Max(peak,Math.Abs(v));stream.Write((short)(Math.Max(-1,Math.Min(1,v))*32767));}}return "Native DSP mix: "+samples.Length+" samples, "+rate+" Hz, "+channels+" channels, RMS "+Math.Sqrt(energy/Math.Max(1,samples.Length)).ToString("F5")+", peak "+peak.ToString("F4")+". Captured the generated AudioSource output, not a separate synthesized soundtrack.";}
 void OnDestroy(){if(carrier!=null)Destroy(carrier);}
}
}
