using System;
using System.IO;
using UnityEngine;
namespace HarborValeGreybox.Racing {
// Development-only observation; never commands vehicles or touches a production profile.
public sealed class RaceAiTelemetryProbe:MonoBehaviour {
 [Serializable]class Measurements{public int runningFrames,fullThrottleFrames,overtakes,recoveries,gates;public float peakKph;}
 string file;readonly Measurements data=new Measurements();
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Hook(){if(!Debug.isDebugBuild)return;var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--race-ai-telemetry");if(i<0||i+1>=args.Length)return;var go=new GameObject("Read-only race AI telemetry");DontDestroyOnLoad(go);go.AddComponent<RaceAiTelemetryProbe>().file=Path.GetFullPath(args[i+1]);}
 void FixedUpdate(){var d=RivalRaceDirector.Instance;if(d?.Opponent==null||d.Race==null||d.Opponent.Held)return;var p=d.Opponent;data.runningFrames++;if(p.Car.commandedPedal>=.999f)data.fullThrottleFrames++;data.peakKph=Mathf.Max(data.peakKph,p.Car.SpeedKph);data.overtakes=Mathf.Max(data.overtakes,p.Overtakes);data.recoveries=Mathf.Max(data.recoveries,p.Recoveries);data.gates=Math.Max(data.gates,d.Race.Rival.NextGate);}
 void OnApplicationQuit(){if(file!=null){Directory.CreateDirectory(Path.GetDirectoryName(file));File.WriteAllText(file,JsonUtility.ToJson(data,true));}}
}
}
