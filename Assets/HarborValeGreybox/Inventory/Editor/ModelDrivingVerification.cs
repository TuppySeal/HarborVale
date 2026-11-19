using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing.Editor {
public static class ModelDrivingVerification {
 public static void Run(){Directory.CreateDirectory("Verification/Inventory");var lines=new List<string>();foreach(var spec in VehicleLineup.All){EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");foreach(var s in UnityEngine.Object.FindObjectsByType<StarterCarSpecification>())UnityEngine.Object.DestroyImmediate(s.gameObject);var route=JsonUtility.FromJson<TrialDefinition>(Resources.Load<TextAsset>("FirstDocksideTrial").text);var mode=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;var car=TrafficWorld.CreateCar(spec.name,new HarborValeGreybox.Workshop.WorkshopBuild(),-1,spec.id);var pilot=car.gameObject.AddComponent<RoadPilot>();pilot.Car=car;pilot.Path=route.route.Select(p=>p.Position).ToArray();pilot.IsRival=true;pilot.CruiseKph=63;var first=route.checkpoints[0];car.ResetAt(first.Position-first.Forward*12+Vector3.Cross(Vector3.up,first.Forward)*1.7f+Vector3.up*.35f,Quaternion.LookRotation(first.Forward));var session=new TrialSession(route);Vector3 previous=car.transform.position;float stalled=0;int last=0;try{for(int i=0;i<30000&&!session.ValidFinish;i++){pilot.Held=session.State==TrialState.Countdown;car.controlsLocked=pilot.Held;pilot.Drive(.02f);car.Tick(.02f);Physics.Simulate(.02f);session.Tick(.02f,previous,car.transform.position);previous=car.transform.position;if(last==session.NextGate)stalled+=.02f;else{last=session.NextGate;stalled=0;}if(session.State==TrialState.Running&&(stalled>45||pilot.NeedsAssistedReset)){pilot.RecoverAt(session.Recover());previous=car.transform.position;stalled=0;}}lines.Add((session.ValidFinish?"PASS ":"FAIL ")+spec.name+" actual37-gate city circuit: gates="+session.NextGate+",seconds="+session.Elapsed.ToString("F2")+",recoveries="+pilot.Recoveries);File.WriteAllLines("Verification/Inventory/model-laps.txt",lines);if(!session.ValidFinish)throw new Exception("Model AI lap failed: "+spec.name);}finally{UnityEngine.Object.DestroyImmediate(car.gameObject);Physics.simulationMode=mode;}}}
}
}
