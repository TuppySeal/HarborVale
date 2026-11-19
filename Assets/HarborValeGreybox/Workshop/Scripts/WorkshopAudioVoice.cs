using System;
using UnityEngine;
namespace HarborValeGreybox.Workshop {
// A dedicated child has exactly one source and one filter, independent of legacy HUD startup order.
public class WorkshopAudioVoice:MonoBehaviour {
 public int rate;public volatile float hz=30,boost,whine,slip,volume,turboRotor,turboSize=1,master=1,tireSpeed,tireCornering;public bool apex;DrivingAudioDsp dsp;int dspRate,cancelPending,cancelAll;
 sealed class ReleaseRequest {public TurboReleaseMode mode;public float pressure,size;}
 ReleaseRequest pending;
 sealed class PopRequest {public float gain;public int samples;}PopRequest pendingPop;
 public void TriggerRelease(TurboReleaseMode mode,float pressure,float size){System.Threading.Interlocked.Exchange(ref pending,new ReleaseRequest{mode=mode,pressure=pressure,size=size});}
 public void CancelTurbo(){System.Threading.Interlocked.Exchange(ref cancelPending,1);System.Threading.Interlocked.Exchange(ref pending,null);}
 public void CancelTransients(){CancelTurbo();System.Threading.Interlocked.Exchange(ref cancelAll,1);System.Threading.Interlocked.Exchange(ref pendingPop,null);}
 public void TriggerPop(float gain,int samples){System.Threading.Interlocked.Exchange(ref pendingPop,new PopRequest{gain=gain,samples=samples});}
 void OnAudioFilterRead(float[] data,int channels){Render(data,channels);}
 public void Render(float[] data,int channels){if(rate<=0||channels<=0)return;if(dsp==null||dspRate!=rate){dsp=new DrivingAudioDsp(rate);dspRate=rate;}if(System.Threading.Interlocked.Exchange(ref cancelPending,0)!=0)dsp.turbo.Cancel();if(System.Threading.Interlocked.Exchange(ref cancelAll,0)!=0)dsp.CancelPops();var request=System.Threading.Interlocked.Exchange(ref pending,null);if(request!=null)dsp.turbo.Release(request.mode,request.pressure,request.size);var pop=System.Threading.Interlocked.Exchange(ref pendingPop,null);if(pop!=null)dsp.Pop(pop.gain,(float)pop.samples/rate);for(int i=0;i+channels<=data.Length;i+=channels){float sample=dsp.Sample(hz,volume,boost,whine,slip,turboRotor,turboSize,master,apex,tireSpeed,tireCornering);for(int c=0;c<channels;c++)data[i+c]=sample;}}
}
}
