using System;
using UnityEngine;
namespace HarborValeGreybox.Racing {
[Serializable]public class HighwayRules {public int version,highwayLimitMph,cityLocalLimitMph,cityBoulevardLimitMph,rampLimitMph,highwayToleranceMph,cityToleranceMph,cityTrafficCount,highwayTrafficCount,radarDetectorPrice,fineBaseCredits,fineCreditsPerMph;public float radarRangeM,detectorRangeM,escapeDistanceM,escapeSeconds;}
[Serializable]public class HighwayRamp {public string id,name,merge;public float width,lengthM,speedLimitMph;public int lanes;public RoutePoint[] points;}
[Serializable]public class CitySpeedRoad {public string id,name;public float width,speedLimitMph;public RoutePoint[] points;}
[Serializable]public class HighwayNetwork {public int version,lanesPerDirection;public string id,name,sourceMapSha256;public float laneWidth,medianWidth,shoulderWidth,roadWidth;public RoutePoint[] center;public HighwayRamp[] ramps;public CitySpeedRoad[] cityRoads;public HighwayRules rules;static HighwayNetwork cached;
 public static HighwayNetwork Load(){if(cached==null){var data=Resources.Load<TextAsset>("HighwayNetwork");if(data==null)throw new InvalidOperationException("Highway source has not been imported");cached=JsonUtility.FromJson<HighwayNetwork>(data.text);if(cached.version!=1||cached.lanesPerDirection!=3||cached.center.Length<100)throw new InvalidOperationException("Unsupported highway source");}return cached;}
 public static Vector3 Right(RoutePoint[] path,int index,bool closed){int prev=closed?(index-1+path.Length)%path.Length:Math.Max(0,index-1),next=closed?(index+1)%path.Length:Math.Min(path.Length-1,index+1);var t=path[next].Position-path[prev].Position;t.y=0;t.Normalize();return new Vector3(t.z,0,-t.x);}
 public static Vector3 Offset(RoutePoint[] path,int i,float offset,bool closed)=>path[i].Position+Right(path,i,closed)*offset;
 public static float GroundHeight(float z){float t=Mathf.Clamp01((z-340)/1000);return 84*t*t*(3-2*t);}
 public Vector3[] Carriageway(bool reverse){var result=new Vector3[center.Length];for(int i=0;i<result.Length;i++){int index=reverse?result.Length-1-i:i;result[i]=Offset(center,index,reverse?-6.6f:6.6f,true);}return result;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){cached=null;}
}
}
