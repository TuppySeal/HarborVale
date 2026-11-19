using System;
using UnityEngine;
namespace HarborValeGreybox.Racing {
// Both racers must cross the same ordered, directional gates. No distance-to-finish payout.
public sealed class DuelRaceSession {
 public readonly DuelContract Contract;public readonly TrialSession Player,Rival;public bool Resolved{get;private set;}public string Result{get;private set;}public float Clock{get;private set;}
 public DuelRaceSession(DuelContract c,TrialDefinition route){Contract=c;Player=new TrialSession(route);Rival=new TrialSession(route);}
 public void Tick(float dt,Vector3 playerFrom,Vector3 playerTo,Vector3 rivalFrom,Vector3 rivalTo){if(Resolved)return;Clock+=dt;Player.Tick(dt,playerFrom,playerTo);Rival.Tick(dt,rivalFrom,rivalTo);if(Player.ValidFinish){Result=Rival.ValidFinish&&Math.Abs(Player.Elapsed-Rival.Elapsed)<=.001f?"draw":!Rival.ValidFinish||Player.Elapsed<Rival.Elapsed?"win":"finish";Resolved=true;}else if(Player.State==TrialState.Abandoned||Clock>600)Forfeit();}
 public void Forfeit(){if(Resolved)return;Player.Abandon();Result="forfeit";Resolved=true;}
}
}
