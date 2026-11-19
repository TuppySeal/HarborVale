using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace HarborValeGreybox.Workshop.Editor {
public static class TabsTireVerification {
 static List<string> checks=new List<string>();static void Check(bool yes,string text){checks.Add((yes?"PASS ":"FAIL ")+text);if(!yes)throw new Exception(text);}
 static double Rms(float[] a,int start,int end)=>Math.Sqrt(a.Skip(start).Take(end-start).Sum(x=>(double)x*x)/(end-start));
 public static void Run(){Directory.CreateDirectory("Verification/TabsTire");try{
  var c=WorkshopCatalog.Load("car_finch");Check(c.offers.Length==49,"Existing 49 offers retained");var primary=WorkshopTabs.All.Where(t=>t.id!="all").ToArray();Check(primary.Length==16,"All upgrade families have a dedicated tab");foreach(var tab in primary){var offers=c.offers.Where(o=>WorkshopTabs.Contains(tab.id,o)).ToArray();Check(offers.Length>0,tab.label+" is populated");Check(offers.Select(o=>o.stage).SequenceEqual(offers.Select(o=>o.stage).OrderBy(s=>s))||tab.id=="na"||tab.id=="sound",tab.label+" keeps authored stage order");}
  Check(c.offers.All(o=>primary.Count(t=>WorkshopTabs.Contains(t.id,o))==1),"Every offer belongs to exactly one specific tab");foreach(string tab in new[]{"small","large","roots","twin"})Check(c.offers.Count(o=>WorkshopTabs.Contains(tab,o))==3,tab+" induction stages isolated from other families");Check(WorkshopTabs.For(c.Find("induction_none_1"))=="na","Return-to-NA service remains discoverable under Engine tunes");Check(c.offers.Where(o=>WorkshopTabs.Contains("twin",o)).All(o=>o.apexOnly),"Finch twin-turbo compatibility restriction retained");
  Check(WorkshopEffects.TireGain(false,100,true,1)==0,"Airborne wheels produce no squeal");Check(WorkshopEffects.TireGain(true,2,false,1)==0,"Low-speed normal driving produces no squeal");Check(WorkshopEffects.TireGain(true,2,true,1)>0,"Actual low-speed powered burnout remains audible");Check(WorkshopEffects.TireGain(true,100,false,.05f)==0,"Normal measured grip stays silent");
  foreach(int rate in new[]{22050,44100,48000}){var d=new TireSoundDsp(rate);var samples=new float[rate*4];for(int i=0;i<samples.Length;i++)samples[i]=d.Sample(i<rate*2?.045f:0,70,.8f);Check(samples.All(x=>!float.IsNaN(x)&&!float.IsInfinity(x)&&Math.Abs(x)<.15f),"Bounded rubber synthesis at "+rate);Check(Math.Abs(samples[0])<.0001f,"Soft squeal attack at "+rate);Check(Rms(samples,rate,rate*2)>.003,"Sustained tire friction is audible at "+rate);Check(Rms(samples,rate*3,rate*4)<.00003,"Slip release fades to silence at "+rate);Check(Math.Abs(samples.Skip(rate).Take(rate).Average())<.001,"Tire layer has negligible DC at "+rate);if(rate==48000)TurboAudioVerification.WriteWav("Verification/TabsTire/Tire_Slip_Fade.wav",samples,rate);}
  var low=new DrivingAudioDsp(48000);var high=new DrivingAudioDsp(48000);bool equal=true;for(int i=0;i<48000;i++)equal&=low.Sample(30,0,0,0,.045f,0,1,1,false,70,.8f)==high.Sample(220,0,0,0,.045f,0,1,1,false,70,.8f);Check(equal,"Tire tone is independent of engine RPM");
  var slow=new TireSoundDsp(48000);var fast=new TireSoundDsp(48000);double delta=0;for(int i=0;i<48000;i++)delta+=Math.Abs(slow.Sample(.045f,20,.8f)-fast.Sample(.045f,110,.8f));Check(delta/48000>.003,"Road speed changes friction texture and resonance");
  var preview=new TireSoundDsp(48000);var demo=new float[48000*6];for(int i=0;i<demo.Length;i++){float t=i/48000f;float amount=t<.5f?0:t<2?.018f:t<4?.045f:0;demo[i]=preview.Sample(amount,Mathf.Lerp(20,100,t/4),t<2?.25f:.9f);}TurboAudioVerification.WriteWav("Verification/TabsTire/Tire_Squeal_Preview.wav",demo,48000);
  File.WriteAllLines("Verification/TabsTire/results.txt",checks);
 }catch(Exception e){checks.Add(e.ToString());File.WriteAllLines("Verification/TabsTire/results.txt",checks);throw;}}
 public static void Release(){Run();AvCameraVerification.Run();HarborValeGreybox.Presentation.Editor.CityArtVerification.Release();}
}
}
