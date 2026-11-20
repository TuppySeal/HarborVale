using UnityEngine;
using UnityEngine.SceneManagement;
namespace HarborValeGreybox.Racing {
// Scene-local resource installation preserves the authored city and its manual edits.
public sealed class HighwayRuntime:MonoBehaviour {
 public static HighwayRuntime Instance{get;private set;}
 public static void Hook(){if(SceneManager.GetActiveScene().name!="HarborVale_CityMap"||FindAnyObjectByType<HighwayRuntime>()!=null)return;var prefab=Resources.Load<GameObject>("HarborExpressway");if(prefab==null){Debug.LogError("Harbor Expressway resource is missing");return;}var root=Instantiate(prefab);root.name="HW01 | playable Harbor Ring Expressway";root.AddComponent<HighwayRuntime>();}
 void Awake(){Instance=this;}void OnDestroy(){if(Instance==this)Instance=null;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){Instance=null;}
 public static bool TryRecovery(Vector3 current,Vector3 heading,out Vector3 position,out Quaternion rotation){position=default;rotation=Quaternion.identity;if(Instance==null)return false;var network=HighwayNetwork.Load();float best=35*35;Vector3 chosen=default,direction=default;
 foreach(float offset in new[]{-10.2f,-6.6f,-3f,3f,6.6f,10.2f})for(int i=0;i<network.center.Length;i++){var p=HighwayNetwork.Offset(network.center,i,offset,true);if(Mathf.Abs(p.y-current.y)>8)continue;float d=(p-current).sqrMagnitude;if(d>=best)continue;best=d;chosen=p;direction=(network.center[(i+1)%network.center.Length].Position-network.center[(i-1+network.center.Length)%network.center.Length].Position)*(offset>0?1:-1);}
 foreach(var ramp in network.ramps)for(int i=1;i<ramp.points.Length-1;i++){var forward=ramp.points[i+1].Position-ramp.points[i-1].Position;bool reverse=ramp.lanes==2&&Vector3.Dot(forward,heading)<0;var p=HighwayNetwork.Offset(ramp.points,i,ramp.lanes==2?(reverse?-1.8f:1.8f):0,false);if(Mathf.Abs(p.y-current.y)>8)continue;float d=(p-current).sqrMagnitude;if(d>=best)continue;best=d;chosen=p;direction=reverse?-forward:forward;}
 if(direction.sqrMagnitude<.001f)return false;position=chosen+Vector3.up*.40f;rotation=Quaternion.LookRotation(direction,Vector3.up);return true;
 }
}
}
