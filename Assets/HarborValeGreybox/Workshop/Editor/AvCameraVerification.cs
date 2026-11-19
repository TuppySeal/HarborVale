using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Workshop.Editor {
public static class AvCameraVerification {
 static List<string> lines=new List<string>();static void Check(bool good,string text){lines.Add((good?"PASS ":"FAIL ")+text);if(!good)throw new Exception(text);}
 static double Rms(float[] values,int start,int end){double e=0;for(int i=start;i<end;i++)e+=values[i]*values[i];return Math.Sqrt(e/Math.Max(1,end-start));}
 public static void Run(){Directory.CreateDirectory("Verification/AvCamera");try{
  foreach(int rate in new[]{22050,44100,48000}){
   var dsp=new DrivingAudioDsp(rate);var values=new float[rate*3];for(int i=0;i<values.Length;i++){if(i==rate){dsp.Pop(.3f,.25f);dsp.turbo.Release(TurboReleaseMode.Flutter,1.5f,1);}float t=(float)i/rate;values[i]=dsp.Sample(30+170*Math.Min(1,t),.2f,.15f,.15f,.15f,1,1,t<2?1:0,true);}
   Check(values.All(v=>!float.IsNaN(v)&&!float.IsInfinity(v)&&Math.Abs(v)<.721f),"All-layer worst-case mix remains finite with headroom at "+rate);Check(Math.Abs(values[0])<.0001f,"Startup has a soft attack at "+rate);Check(Rms(values,rate*5/2,rate*3)<.0001,"Focus/scene master fade reaches silence at "+rate);Check(Math.Abs(values.Skip(rate).Take(rate).Average())<.003,"Mixed signal has negligible DC at "+rate);lines.Add("MEASURE rate="+rate+" peak="+values.Max(v=>Math.Abs(v)).ToString("F5")+" steady RMS="+Rms(values,rate,rate*2).ToString("F5"));
   if(rate==48000)TurboAudioVerification.WriteWav("Verification/AvCamera/All_Layers_Fade.wav",values,rate);
  }
  foreach(string layer in new[]{"engine","tires","blower","turbo","pop"}){var d=new DrivingAudioDsp(48000);if(layer=="pop")d.Pop(.24f,.25f);float[] a=Enumerable.Range(0,48000).Select(_=>d.Sample(120,layer=="engine"?.1f:0,layer=="turbo"?.075f:0,layer=="blower"?.045f:0,layer=="tires"?.045f:0,1,1,1,false)).ToArray();Check(Rms(a,0,a.Length)>.001,"Existing "+layer+" layer is audible and finite");TurboAudioVerification.WriteWav("Verification/AvCamera/Layer_"+layer+".wav",a,48000);}
  var cancelled=new DrivingAudioDsp(48000);cancelled.Pop(.3f,.25f);cancelled.turbo.Release(TurboReleaseMode.BlowOffValve,1.3f,1);for(int i=0;i<2400;i++)cancelled.Sample(30,0,0,0,0,0,1,1,false);cancelled.CancelPops();cancelled.turbo.Cancel();float[] tail=Enumerable.Range(0,24000).Select(_=>cancelled.Sample(30,0,0,0,0,0,1,1,false)).ToArray();Check(Rms(tail,12000,24000)<.0001,"Recovery cancellation fades transient tails cleanly");
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var g=new GameObject("Camera fixture car");var car=g.AddComponent<FinchVehicle>();car.Initialize();car.Body.position=new Vector3(10000,3,10000);car.transform.position=car.Body.position;var cameraObject=new GameObject("Camera fixture");cameraObject.AddComponent<Camera>();var chase=cameraObject.AddComponent<FinchChaseCamera>();chase.target=car;chase.externalLookInput=true;chase.Follow(.02f);
  foreach(float yaw in new[]{-90f,90f,180f,0f}){chase.commandedLookYaw=yaw;for(int i=0;i<90;i++)chase.Follow(1f/60);Check(Math.Abs(Mathf.DeltaAngle(chase.LookYaw,yaw))<.5f,"Smooth keyboard view reaches "+yaw+" degrees");Check(Vector3.Dot(cameraObject.transform.up,Vector3.up)>.7f,"View keeps a stable upright horizon "+yaw);}
  chase.commandedMouseLook=true;chase.UpdateLook(.02f);chase.commandedMouseDelta=new Vector2(200,200);for(int i=0;i<120;i++)chase.UpdateLook(.016f);Check(chase.LookPitch>=-25.01f&&chase.LookPitch<=30.01f&&Math.Abs(chase.LookYaw)<=145.01f,"Mouse yaw/pitch are bounded");car.controlsLocked=true;for(int i=0;i<100;i++)chase.UpdateLook(.016f);Check(Math.Abs(Mathf.DeltaAngle(chase.LookYaw,0))<.2f&&Math.Abs(chase.LookPitch)<.2f&&!chase.MouseCaptured,"Garage/modal lock releases and recenters view");car.controlsLocked=false;chase.commandedMouseLook=false;chase.commandedMouseDelta=Vector2.zero;chase.commandedLookYaw=90;for(int i=0;i<90;i++)chase.Follow(.016f);chase.bonnet=true;chase.Follow(.016f);Check(Vector3.Dot(cameraObject.transform.forward,car.transform.right)>.98f,"Bonnet view supports side looking");
  chase.bonnet=false;chase.commandedLookYaw=0;chase.Snap();var wall=GameObject.CreatePrimitive(PrimitiveType.Cube);wall.transform.position=car.transform.position+Vector3.back*3+Vector3.up;wall.transform.localScale=new Vector3(10,8,.4f);Physics.SyncTransforms();chase.Follow(.016f);Check(cameraObject.transform.position.z>wall.transform.position.z+.3f,"Camera contracts in front of scenery obstruction");UnityEngine.Object.DestroyImmediate(wall);
  var body=car.Body;body.rotation=Quaternion.Euler(0,35,0);Vector3 local=new Vector3(.777f,.4f,1.225f);Vector3 physical=body.position+body.rotation*local;Check(Vector3.Distance(VehicleVisualPose.LocalPosition(body,physical),local)<.002f,"Wheel position maps through physical body pose");Check(Quaternion.Angle(VehicleVisualPose.LocalRotation(body,body.rotation*Quaternion.Euler(0,20,70)),Quaternion.Euler(0,20,70))<.01f,"Wheel steer/spin maps through body rotation without physics writes");
  Check(car.GetComponentsInChildren<WheelCollider>().Length==4,"Presentation changes retain exactly four physical contacts");UnityEngine.Object.DestroyImmediate(cameraObject);UnityEngine.Object.DestroyImmediate(g);File.WriteAllLines("Verification/AvCamera/rules.txt",lines);
 }catch(Exception e){lines.Add(e.ToString());File.WriteAllLines("Verification/AvCamera/rules.txt",lines);throw;}}
 public static void Release(){Run();TurboAudioVerification.Release();}
}
}
