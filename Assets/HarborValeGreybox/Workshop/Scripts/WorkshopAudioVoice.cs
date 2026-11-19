using System;
using UnityEngine;
namespace HarborValeGreybox.Workshop {
// A dedicated child has exactly one source and one filter, independent of legacy HUD startup order.
public class WorkshopAudioVoice:MonoBehaviour {
 public int rate;public volatile float hz=30,boost,whine,slip,volume,popGain,turboRotor,turboSize=1;public int popSamples,popLength;double phase,whistle;TurboSoundDsp turbo;
 sealed class ReleaseRequest {public TurboReleaseMode mode;public float pressure,size;public bool cancel;}
 ReleaseRequest pending;
 public void TriggerRelease(TurboReleaseMode mode,float pressure,float size){System.Threading.Interlocked.Exchange(ref pending,new ReleaseRequest{mode=mode,pressure=pressure,size=size});}
 public void CancelTurbo(){System.Threading.Interlocked.Exchange(ref pending,new ReleaseRequest{cancel=true});}
 void OnAudioFilterRead(float[] data,int channels){if(rate<=0)return;if(turbo==null)turbo=new TurboSoundDsp(rate);var request=System.Threading.Interlocked.Exchange(ref pending,null);if(request!=null){if(request.cancel)turbo.Cancel();else turbo.Release(request.mode,request.pressure,request.size);}for(int i=0;i<data.Length;i+=channels){phase=(phase+hz/rate)%1;whistle=(whistle+(1500+hz*7)/rate)%1;double v=volume*(Math.Sin(phase*Math.PI*2)+.35*Math.Sin(phase*Math.PI*4));v+=whine*Math.Sin(whistle*Math.PI*2)+slip*Math.Sin(whistle*Math.PI*3);v+=turbo.Sample(boost,turboRotor,turboSize);if(popSamples>0){v+=popGain*(double)popSamples/Math.Max(1,popLength)*Math.Sin(phase*Math.PI*17);popSamples--;}float sample=(float)Math.Max(-.8,Math.Min(.8,v));for(int c=0;c<channels;c++)data[i+c]=sample;}}
}
}
