using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace HarborValeGreybox.Racing.Editor {
public static class TrialVerification {
 static List<string> lines=new List<string>();static string root;
 static void Check(bool pass,string text){lines.Add((pass?"PASS ":"FAIL ")+text);if(!pass)throw new Exception(text);}
 static TrialDefinition Fixture(){var data=Resources.Load<TextAsset>("FirstDocksideTrial");var d=JsonUtility.FromJson<TrialDefinition>(data.text);d.checkpoints=new[]{Gate(0),Gate(20),Gate(40)};d.route=new[]{new RoutePoint{z=0},new RoutePoint{z=20},new RoutePoint{z=40}};return d;}
 static TrialGate Gate(float z)=>new TrialGate{z=z,forwardZ=1,halfWidth=10};
 static void Cross(TrialSession s,int index){var g=s.Definition.checkpoints[index];s.Tick(.02f,g.Position-g.Forward,g.Position+g.Forward);}
 static TrialSession Finish(TrialDefinition d,float elapsed){var s=new TrialSession(d);s.Tick(4,Vector3.zero,Vector3.zero);s.Tick(elapsed-.1f,new Vector3(0,0,-10),new Vector3(0,0,-10));for(int i=0;i<d.checkpoints.Length;i++)Cross(s,i);return s;}
 static string PathFor(string name)=>Path.Combine(root,name+".json");
 public static void Run(){
  root=Path.GetFullPath("Verification/Trial/Rules_"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);lines.Clear();
  try{
   var d=Fixture();var s=new TrialSession(d);s.Tick(1,Vector3.back,Vector3.forward);Check(s.State==TrialState.Countdown&&s.NextGate==0&&s.Elapsed==0,"Countdown blocks checkpoint progress and race time");s.Tick(3,Vector3.zero,Vector3.zero);
   s.Tick(.02f,Vector3.forward,Vector3.back);Check(s.NextGate==0,"Wrong-direction crossing rejected");s.Tick(.02f,new Vector3(15,0,-1),new Vector3(15,0,1));Check(s.NextGate==0,"Outside gate width rejected");s.Tick(.02f,new Vector3(0,5,-1),new Vector3(0,5,1));Check(s.NextGate==0,"Crossing above height window rejected");s.Tick(.02f,Vector3.back*80,Vector3.forward*80);Check(s.NextGate==0,"Teleport crossing rejected");Cross(s,2);Check(s.NextGate==0&&!s.ValidFinish,"Early finish rejected");Cross(s,0);Cross(s,2);Check(s.NextGate==1,"Skipped checkpoint rejected");Cross(s,1);Cross(s,2);Check(s.ValidFinish,"Ordered directional swept crossings finish");
   var reset=new TrialSession(d);reset.Tick(4,Vector3.zero,Vector3.zero);Cross(reset,0);float before=reset.Elapsed;var recovery=reset.Recover();Check(reset.NextGate==1&&Mathf.Abs(reset.Elapsed-before-3)<.001f&&recovery==d.checkpoints[0],"Recovery preserves next gate and adds 3 seconds");reset.Abandon();Check(reset.State==TrialState.Abandoned&&!reset.ValidFinish,"Abandon cannot finish/pay");
   var invalid=new TrialSession(d);invalid.Tick(4,Vector3.zero,Vector3.zero);invalid.Tick(.02f,Vector3.zero,new Vector3(float.NaN,0,1));Check(invalid.State==TrialState.Abandoned&&!invalid.ValidFinish,"Non-finite vehicle positions cannot produce a valid finish");
   var store=new RaceProgressStore(PathFor("progress"),d);var gold=Finish(d,90);var paid=store.Settle(gold,out var duplicate);Check(!duplicate&&paid.credits==1100&&paid.firstClear&&paid.reputation==50&&store.Profile.earnings==1100,"Gold first finish commits 700 + 400 credits and 50 reputation");store.Settle(gold,out duplicate);Check(duplicate&&store.Profile.earnings==1100&&store.Profile.receipts.Count==1,"Duplicate finish cannot pay twice");
   var loaded=new RaceProgressStore(PathFor("progress"),d);loaded.Settle(gold,out duplicate);Check(duplicate&&loaded.Profile.earnings==1100,"Duplicate protection survives reload");
   var bronze=Finish(d,165);var replay=loaded.Settle(bronze,out _);Check(!replay.firstClear&&replay.credits==455&&replay.reputation==0&&loaded.Profile.earnings==1555,"Bronze replay pays reduced credits without first-clear or repeated reputation");Check(loaded.Profile.records[0].bestMilliseconds<100000,"Slower replay preserves best time");
   bool refused=false;try{loaded.Settle(reset,out _);}catch(InvalidOperationException){refused=true;}Check(refused&&loaded.Profile.earnings==1555,"Aborted session settlement rejected");
   var improvement=new RaceProgressStore(PathFor("improve"),d);improvement.Settle(Finish(d,165),out _);var improved=improvement.Settle(Finish(d,90),out _);Check(improved.reputation==20&&improvement.Profile.reputation==50,"Improved medal grants only best-result reputation difference");
   var staleA=new RaceProgressStore(PathFor("instances"),d);var staleB=new RaceProgressStore(PathFor("instances"),d);staleA.Settle(Finish(d,90),out _);staleB.Settle(Finish(d,100),out _);Check(staleB.Profile.earnings==1800&&staleB.Profile.receipts.Count==2,"Stale second instance re-reads ledger under exclusive lock");
   var retuned=Fixture();retuned.goldSeconds=70;retuned.silverSeconds=100;retuned.bronzeSeconds=130;retuned.baseReward=900;Check(new RaceProgressStore(PathFor("progress"),retuned).Profile.earnings==1555,"Historical receipts survive later medal/reward tuning");
   var fault=new RaceProgressStore(PathFor("progress"),d,()=>{throw new IOException("Injected failure before replace");});int prior=fault.Profile.earnings;bool failed=false;try{fault.Settle(Finish(d,100),out _);}catch(IOException){failed=true;}Check(failed&&fault.Profile.earnings==prior&&new RaceProgressStore(PathFor("progress"),d).Profile.earnings==prior,"Failed write leaves memory and committed credits unchanged; temp ignored");
   File.WriteAllText(PathFor("progress"),"{broken");var backup=new RaceProgressStore(PathFor("progress"),d);Check(backup.RecoveredBackup&&backup.Profile.earnings==1100,"Corrupt primary recovers validated last backup");backup.Settle(Finish(d,100),out _);Check(new RaceProgressStore(PathFor("progress"),d).Profile.earnings==1800,"Settlement after backup recovery writes valid combined transaction");
   File.WriteAllText(PathFor("bad"),"{}");File.WriteAllText(PathFor("bad")+".bak","{bad");bool protectedSave=false;try{new RaceProgressStore(PathFor("bad"),d);}catch(IOException){protectedSave=true;}Check(protectedSave&&File.ReadAllText(PathFor("bad"))=="{}","Invalid saves are preserved and never silently reset");
   var actual=JsonUtility.FromJson<TrialDefinition>(Resources.Load<TextAsset>("FirstDocksideTrial").text);actual.Validate();var real=Finish(actual,100);Check(real.ValidFinish&&actual.lengthM>1000,"Authored closed Dockside route accepts all ordered checkpoints");
   File.WriteAllLines("Verification/Trial/rule-results.txt",lines);Debug.Log("DOCKSIDE_TRIAL_RULES_PASS");
  }catch(Exception e){lines.Add(e.ToString());Directory.CreateDirectory("Verification/Trial");File.WriteAllLines("Verification/Trial/rule-results.txt",lines);throw;}
 }
}
}
