using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing.Editor {
public static class AudioUnitsVerification {
 public static void Run(){Directory.CreateDirectory("Verification/AudioUnits");var lines=new List<string>();void Check(bool ok,string text){lines.Add((ok?"PASS ":"FAIL ")+text);File.WriteAllLines("Verification/AudioUnits/rules.txt",lines);if(!ok)throw new Exception(text);}
 Check(RoadEngineAudio.DistanceGain(2)==1,"NPC signal stays audible inside two metres");Check(RoadEngineAudio.DistanceGain(10)>RoadEngineAudio.DistanceGain(25)&&RoadEngineAudio.DistanceGain(25)>RoadEngineAudio.DistanceGain(40),"NPC generated-signal gain strictly falls with listener distance");Check(RoadEngineAudio.DistanceGain(52)==0&&RoadEngineAudio.DistanceGain(500)==0,"NPC generated engine has a hard silent range boundary");
 double Rms(float distance){var voice=new GameObject("PCM distance verification").AddComponent<WorkshopAudioVoice>();try{voice.rate=48000;voice.hz=150;voice.volume=.042f;voice.engineLoad=1;voice.master=RoadEngineAudio.DistanceGain(distance);var samples=new float[48000];voice.Render(samples,1);return Math.Sqrt(samples.Skip(24000).Select(x=>(double)x*x).Average());}finally{UnityEngine.Object.DestroyImmediate(voice.gameObject);}}
 double near=Rms(3),mid=Rms(25),far=Rms(80);Check(near>mid*3&&far==0,"Actual procedural PCM changes from audible near to quiet mid to exactly silent far");
 var old=DrivingUnits.FileOverrideForTests;try{string folder=Path.GetFullPath("Verification/AudioUnits/settings/"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);DrivingUnits.FileOverrideForTests=folder+"/units.json";DrivingUnits.Reload();Check(DrivingUnits.Mph,"Fresh display defaults to mph");Check(Math.Abs(DrivingUnits.FromMph(70)-112.65408f)<.0001f&&Math.Abs(DrivingUnits.FromMph(20)-32.18688f)<.0001f,"Posted70mph and20mph tolerance convert to exact shared internal kmh");Check(DrivingUnits.Save(false),"Metric preference writes atomically");DrivingUnits.Reload();Check(!DrivingUnits.Mph&&DrivingUnits.Speed(DrivingUnits.FromMph(70))=="113 km/h","Metric preference reloads and formats actual speed");Check(DrivingUnits.Save(true)&&DrivingUnits.Speed(DrivingUnits.FromMph(70))=="70 mph","Mph preference restores without changing internal speed");File.WriteAllText(DrivingUnits.FilePath,"{\"version\":2,\"mph\":false}");string newer=File.ReadAllText(DrivingUnits.FilePath);Check(!DrivingUnits.Save(true)&&File.ReadAllText(DrivingUnits.FilePath)==newer,"Unknown future display settings are preserved without overwrite");}finally{DrivingUnits.FileOverrideForTests=old;DrivingUnits.Reload();}}
 public static void Release(){Run();CircuitDrivingVerification.Release();}
}
}
