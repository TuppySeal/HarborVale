using UnityEngine;
namespace HarborValeGreybox.Racing {
public enum TrafficStyle { Cautious, Commuter, Hurried, LeftCamper }
[DisallowMultipleComponent]
public sealed class TrafficPersona : MonoBehaviour {
 public int Seed; public TrafficStyle Style { get; private set; }
 public string Label { get; private set; } = "Commuter";
 public int PreferredLane { get; private set; } = 1;
 public float HeadwaySeconds { get; private set; } = 1.7f;
 public float SpeedBias { get; private set; }
 public bool Configured { get; private set; }
 public bool IsCamper => Style == TrafficStyle.LeftCamper;
 public void Configure(int seed) {
  Seed=seed; uint x=unchecked((uint)seed*747796405u+2891336453u); x=((x>>((int)(x>>28)+4))^x)*277803737u; x=(x>>22)^x;
  int pick=(int)(x%100); SpeedBias=((x>>8)%1000)/999f;
  Style=pick<8?TrafficStyle.LeftCamper:pick<32?TrafficStyle.Cautious:pick<78?TrafficStyle.Commuter:TrafficStyle.Hurried;
  Label=Style==TrafficStyle.LeftCamper?"Left-lane camper":Style.ToString();
  PreferredLane=IsCamper?2:Style==TrafficStyle.Cautious?0:1;
  HeadwaySeconds=Style==TrafficStyle.Cautious?2.2f:Style==TrafficStyle.Hurried?1.4f:1.8f; Configured=true;
 }
 public float DesiredHighwayKph(int lane) {
  if(IsCamper&&lane==2)return (63+SpeedBias*2)*1.609344f;
  return (lane==0?Mathf.Lerp(55,62,SpeedBias):lane==1?Mathf.Lerp(65,70,SpeedBias):Mathf.Lerp(71,78,SpeedBias))*1.609344f;
 }
 void Start(){if(!Configured)Configure(Seed);}
}
}
