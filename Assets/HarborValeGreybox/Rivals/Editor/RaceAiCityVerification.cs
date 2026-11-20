using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
namespace HarborValeGreybox.Racing.Editor {
public static class RaceAiCityVerification {
 public static void Run(){Directory.CreateDirectory("Verification/RaceAi");var lines=new List<string>();foreach(var rival in RivalsData.Load().opponents){EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");foreach(var s in UnityEngine.Object.FindObjectsByType<StarterCarSpecification>())UnityEngine.Object.DestroyImmediate(s.gameObject);var route=JsonUtility.FromJson<TrialDefinition>(Resources.Load<TextAsset>("FirstDocksideTrial").text);var mode=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;var car=TrafficWorld.CreateCar(rival.name,rival.build,0,rival.modelId);var pilot=car.gameObject.AddComponent<RoadPilot>();pilot.Car=car;pilot.Path=route.route.Select(p=>p.Position).ToArray();pilot.IsRival=true;pilot.CruiseKph=rival.speedKph;var first=route.checkpoints[0];car.ResetAt(first.Position-first.Forward*12+Vector3.Cross(Vector3.up,first.Forward)*1.7f+Vector3.up*.35f,Quaternion.LookRotation(first.Forward));var session=new TrialSession(route);Vector3 previous=car.transform.position;int full=0;float max=0;try{for(int i=0;i<25000&&!session.ValidFinish;i++){pilot.Held=session.State==TrialState.Countdown;car.controlsLocked=pilot.Held;pilot.Drive(.02f);if(car.commandedPedal==1)full++;car.Tick(.02f);Physics.Simulate(.02f);session.Tick(.02f,previous,car.transform.position);previous=car.transform.position;max=Mathf.Max(max,car.SpeedKph);}bool pass=session.ValidFinish&&pilot.Recoveries==0&&full>100;lines.Add((pass?"PASS ":"FAIL ")+rival.id+" actual authored car/build circuit: gates="+session.NextGate+",seconds="+session.Elapsed.ToString("F2")+",peakKph="+max.ToString("F1")+",fullThrottleTicks="+full+",recoveries="+pilot.Recoveries);File.WriteAllLines("Verification/RaceAi/city-rivals.txt",lines);if(!pass)throw new Exception("Race AI city lap failed "+rival.id);}finally{UnityEngine.Object.DestroyImmediate(car.gameObject);Physics.simulationMode=mode;}}}
}
}
