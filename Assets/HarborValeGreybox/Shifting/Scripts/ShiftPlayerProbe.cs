using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Launch;
using HarborValeGreybox.SportsTest;
namespace HarborValeGreybox.Shifting {
public sealed class ShiftPlayerProbe:MonoBehaviour {
 static string folder;List<string> report=new List<string>();double deadline;bool failed;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]static void Prepare(){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--shift-smoke-dir");folder=Debug.isDebugBuild&&i>=0&&i+1<args.Length?Path.GetFullPath(args[i+1]):null;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Hook(){if(folder==null)return;var g=new GameObject("Smooth shifting Windows verification");DontDestroyOnLoad(g);g.AddComponent<ShiftPlayerProbe>();}
 void OnEnable(){Application.logMessageReceived+=Log;}void OnDisable(){Application.logMessageReceived-=Log;}
 void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)Fail(message);}
 void Fail(string message){if(failed)return;failed=true;report.Add("FAIL "+message);File.WriteAllLines(Path.Combine(folder,"player-shifting.txt"),report);Application.Quit(1);}
 void Check(bool yes,string text){if(!yes){Fail(text);throw new Exception(text);}report.Add("PASS "+text);}
 void Update(){if(deadline>0&&Time.realtimeSinceStartupAsDouble>deadline)Fail("Timeout");}
 IEnumerator Wait(Func<bool> condition){double until=Time.realtimeSinceStartupAsDouble+25;while(!condition()){if(Time.realtimeSinceStartupAsDouble>until){Check(false,"Wait timeout");yield break;}yield return null;}yield return new WaitForSecondsRealtime(.15f);}
 IEnumerator Capture(string name){yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,name+".png"));yield return new WaitForSecondsRealtime(.2f);}
 IEnumerator Start(){Directory.CreateDirectory(folder);deadline=Time.realtimeSinceStartupAsDouble+110;yield return new WaitForSecondsRealtime(.5f);Check(HarborValeLaunch.Instance.Open(HarborValeLaunch.SportsScene),"Menu starts sports mode");yield return Wait(()=>UnityEngine.Object.FindAnyObjectByType<ApexTestDrive>()?.Ready==true);var sports=UnityEngine.Object.FindAnyObjectByType<ApexTestDrive>();var car=sports.Car;car.externalInput=true;for(int i=1;i<=4;i++)Check(sports.SelectStep(i),"Upgrade "+i+" applies");yield return Wait(()=>car.SpeedKph<.5f&&car.GroundedWheels>=2);Check(car.Engine.Mode==TransmissionMode.Automatic,"New drive defaults to automatic");Check(car.ToggleTransmissionMode()&&car.Engine.Mode==TransmissionMode.Manual,"Mode toggle selects manual");Check(car.PaddleShift(1),"Paddle selects second gear");yield return Wait(()=>!car.Engine.Shifting);Check(car.Engine.Gear==2,"Manual second remains selected");car.commandedPedal=1;yield return Wait(()=>car.SpeedKph>=80);Check(car.Engine.Gear==2,"Manual holds second during real acceleration");Check(!car.PaddleShift(-1)&&car.Engine.Gear==2&&car.Engine.ShiftNote.Contains("RPM"),"Unsafe real-speed downshift is rejected");yield return Capture("Manual_Downshift_Protection");Check(car.PaddleShift(1),"Safe third gear accepted");yield return Wait(()=>!car.Engine.Shifting);yield return Capture("Manual_Third_Gear");car.commandedPedal=-1;yield return Wait(()=>car.SpeedKph<=30);car.commandedPedal=1;Check(car.ToggleTransmissionMode()&&car.Engine.Mode==TransmissionMode.Automatic,"Automatic resumes after manual deceleration");yield return Wait(()=>car.Engine.Gear<3);Check(car.Engine.Gear<=2,"Real throttle kickdown selects lower gear");yield return Capture("Automatic_Kickdown");car.commandedPedal=0;car.Recover();yield return Wait(()=>car.SpeedKph<.5f&&car.GroundedWheels>=2);Check(car.ToggleTransmissionMode(),"Manual selected after recovery");Check(sports.SelectStep(4)&&car.Engine.Mode==TransmissionMode.Manual,"Part reinstallation keeps manual mode");car.Recover();Check(car.Engine.Mode==TransmissionMode.Manual,"Recovery keeps manual mode");car.GetComponent<FinchDrivingHUD>().SetInspection(true);Check(!car.ToggleTransmissionMode()&&!car.PaddleShift(1),"Map blocks transmission controls");car.GetComponent<FinchDrivingHUD>().SetInspection(false);car.controlsLocked=true;Check(!car.ToggleTransmissionMode()&&!car.PaddleShift(1),"Countdown/shop lock blocks transmission controls");car.controlsLocked=false;yield return Capture("Manual_Controls_HUD");File.WriteAllLines(Path.Combine(folder,"player-shifting.txt"),report);Application.Quit(0);
 }
}
}
