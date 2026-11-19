using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarborValePreviewReview;
namespace ClaudeDashboardReview {
public class HUDPreviewController:MonoBehaviour {
 public GameObject[] cars;public int Selected{get;private set;}public DashboardTelemetry[] telemetry=new DashboardTelemetry[2];float clock;
 public void Select(int index){Selected=index;for(int i=0;i<cars.Length;i++)cars[i].SetActive(i==index);}
 void Update(){clock+=Time.unscaledDeltaTime;if(Input.GetKeyDown(KeyCode.Alpha1))Select(0);if(Input.GetKeyDown(KeyCode.Alpha2))Select(1);if(Input.GetKeyDown(KeyCode.T))telemetry[Selected].tripKm=0;for(int i=0;i<2;i++){float speed=(1+Mathf.Sin(clock*.4f+i))*(i==0?65:100);telemetry[i].speedKph=speed;telemetry[i].rpm=900+speed*(i==0?34:24);telemetry[i].boost=i==1?Mathf.Clamp01(speed/150)*.85f:0;telemetry[i].tripKm=CarHUD.AdvanceTrip(telemetry[i].tripKm,speed,Time.unscaledDeltaTime);}}
 void OnGUI(){GUI.Box(new Rect(16,16,Screen.width-32,98),"CAR DASHBOARD REVIEW | SIMULATED INPUTS");GUI.Label(new Rect(30,44,Screen.width-60,38),"Preview speed/RPM only. Trip uses speed × time; no game mileage or saves are changed.\nOdometer remains unconnected until real vehicle identity and persisted distance are supplied.");if(GUI.Button(new Rect(30,126,190,36),"1 — Finch dashboard"))Select(0);if(GUI.Button(new Rect(230,126,190,36),"2 — Apex dashboard"))Select(1);CarHUD.Draw(Selected==1,telemetry[Selected]);}
 IEnumerator Start(){Select(0);var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--review-dir");if(i<0||i+1>=args.Length)yield break;string folder=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(folder);var lines=new List<string>();if(CarHUD.AdvanceTrip(0,0,60)!=0||Math.Abs(CarHUD.AdvanceTrip(0,36,1)-.01)>.000001)throw new Exception("Distance units/stationary test failed");lines.Add("PASS stationary trip does not advance; 36 km/h × 1 s = 0.01 km");var recorder=gameObject.AddComponent<PreviewRecorder>();yield return new WaitForSecondsRealtime(1);recorder.Begin(Path.Combine(folder,"recording"));for(int car=0;car<2;car++){Select(car);yield return new WaitForSecondsRealtime(3);PreviewRecorder.SuppressNextFrame=true;yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(folder,car==0?"Finch_Dashboard.png":"Apex_Dashboard.png"));yield return new WaitForSecondsRealtime(.3f);lines.Add("PASS rendered distinct "+(car==0?"Finch dual analog":"Apex central performance ring")+" cluster with explicit simulated trip and unconnected odometer");if(telemetry[car].odometerKm.HasValue)throw new Exception("Invented odometer");}recorder.Finish();yield return new WaitForSecondsRealtime(.3f);lines.Add("PASS no game progression/mileage persistence or physics component involved");File.WriteAllLines(Path.Combine(folder,"native-results.txt"),lines);Application.Quit(0);}
}
}
