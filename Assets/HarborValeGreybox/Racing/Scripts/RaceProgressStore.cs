using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Garage;
namespace HarborValeGreybox.Racing {
public sealed class UnsupportedProgressVersionException:IOException {public UnsupportedProgressVersionException():base("This progress requires a newer game build.") {}}
[Serializable] public class TrialReceipt {public string runId,eventId,completedUtc;public int milliseconds,rank,credits,reputation;public bool firstClear;public int rewardBase,firstBonus,creditBps,reputationTarget,goldMs,silverMs,bronzeMs;}
[Serializable] public class TrialRecord {public string eventId;public int bestMilliseconds,bestReputation,completions;public bool firstClearPaid;}
[Serializable]public class TuneReceipt {public string transactionId,prototypeCarId,pathId,configId,paidUtc;public int stage,credits;}
[Serializable] public class RaceProgress {public int version;public string profileId;public int earnings,reputation;public List<TrialReceipt> receipts=new List<TrialReceipt>();public List<TrialRecord> records=new List<TrialRecord>();public int availableCredits,engineStage;public string prototypeCarId;public List<TuneReceipt> spending=new List<TuneReceipt>();}
// One atomic receipt transaction commits all earned credits and record changes.
// Prototype race earnings deliberately do not fabricate purchase/vehicle history.
public sealed class RaceProgressStore {
 readonly string path,legacyPath;readonly TrialDefinition definition;readonly Action beforeReplace;
 public RaceProgress Profile{get;private set;}public bool RecoveredBackup{get;private set;}
 public string PathName=>path;
 public RaceProgressStore(string file,TrialDefinition trial,Action faultBeforeReplace=null,string legacyFile=null){path=file;legacyPath=legacyFile;definition=trial;definition.Validate();beforeReplace=faultBeforeReplace;Directory.CreateDirectory(Path.GetDirectoryName(path));using(Lock()){Profile=Load();}}
 FileStream Lock()=>new FileStream(path+".lock",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);
 RaceProgress Read(string file){var p=JsonUtility.FromJson<RaceProgress>(File.ReadAllText(file));if(p!=null&&p.version>2)throw new UnsupportedProgressVersionException();Validate(p);return p;}
 public void Refresh(){using(Lock()){Profile=Load();}}
 RaceProgress Load(){
  if(!File.Exists(path)&&!File.Exists(path+".bak")){
   if(legacyPath!=null&&(File.Exists(legacyPath)||File.Exists(legacyPath+".bak"))){using(new FileStream(legacyPath+".lock",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None)){try{var p=Read(legacyPath);RecoveredBackup=false;return p;}catch(UnsupportedProgressVersionException){throw;}catch(Exception legacyError){try{var p=Read(legacyPath+".bak");RecoveredBackup=true;return p;}catch(Exception){throw new IOException("Legacy race progress could not be imported. Existing files were preserved.",legacyError);}}}}
   string id=Guid.NewGuid().ToString("N");return new RaceProgress{version=2,profileId=id,prototypeCarId=id+":finch"};
  }
  Exception primary=null;try{var p=Read(path);RecoveredBackup=false;return p;}catch(UnsupportedProgressVersionException){throw;}catch(Exception e){primary=e;}
  try{var p=Read(path+".bak");RecoveredBackup=true;return p;}catch(Exception){throw new IOException("Race progress could not be read. Existing saves were preserved.",primary);}
 }
 void Validate(RaceProgress p){
  if(p==null||(p.version!=1&&p.version!=2)||!Guid.TryParse(p.profileId,out _)||p.receipts==null||p.records==null||p.earnings<0||p.reputation<0||p.receipts.Count>100000)throw new InvalidDataException("Invalid race profile");
  var ids=new HashSet<string>();int credits=0,rep=0,count=0,best=0,bestRep=0;bool first=false;
  foreach(var receipt in p.receipts){
   if(receipt==null||!Guid.TryParse(receipt.runId,out _)||!ids.Add(receipt.runId)||receipt.eventId!=definition.id||receipt.milliseconds<=0||receipt.rank<0||receipt.rank>3)throw new InvalidDataException("Invalid race receipt");
   int rank=receipt.milliseconds<=receipt.goldMs?0:receipt.milliseconds<=receipt.silverMs?1:receipt.milliseconds<=receipt.bronzeMs?2:3;
   if(receipt.goldMs<=0||receipt.silverMs<=receipt.goldMs||receipt.bronzeMs<=receipt.silverMs||receipt.rewardBase<0||receipt.firstBonus<0||receipt.creditBps<0||receipt.creditBps>10000||receipt.reputationTarget<0||receipt.rank!=rank||receipt.firstClear!=!first)throw new InvalidDataException("Invalid first-clear/result history");
   int rankRep=receipt.reputationTarget;int expected=checked((int)((long)receipt.rewardBase*receipt.creditBps/10000)+(first?0:receipt.firstBonus));
   if(receipt.credits!=expected||receipt.reputation!=Mathf.Max(0,rankRep-bestRep))throw new InvalidDataException("Reward does not match receipt history");
   credits=checked(credits+receipt.credits);rep=checked(rep+receipt.reputation);count++;best=best==0?receipt.milliseconds:Math.Min(best,receipt.milliseconds);bestRep=Math.Max(bestRep,rankRep);first=true;
  }
  if(credits!=p.earnings||rep!=p.reputation||p.records.Count!=(count>0?1:0))throw new InvalidDataException("Race ledger total mismatch");
  if(count>0){var r=p.records[0];if(r.eventId!=definition.id||r.completions!=count||r.bestMilliseconds!=best||r.bestReputation!=bestRep||!r.firstClearPaid)throw new InvalidDataException("Best record mismatch");}
  if(p.version==1){if(p.spending!=null&&p.spending.Count>0)throw new InvalidDataException("Unexpected legacy spending");p.version=2;p.availableCredits=p.earnings;p.engineStage=0;p.prototypeCarId=p.profileId+":finch";p.spending=new List<TuneReceipt>();}
  if(p.spending==null||p.prototypeCarId!=p.profileId+":finch"||p.engineStage<0||p.engineStage>1)throw new InvalidDataException("Invalid installed build");
  int spent=0,stage=0;var transactions=new HashSet<string>();foreach(var purchase in p.spending){if(purchase==null||!Guid.TryParse(purchase.transactionId,out _)||!transactions.Add(purchase.transactionId)||purchase.prototypeCarId!=p.prototypeCarId||purchase.stage!=stage+1||purchase.stage!=1||purchase.credits<=0||purchase.pathId!="path_finch_na"||purchase.configId!="cfg_finch_na_s1")throw new InvalidDataException("Invalid paid tune receipt");spent=checked(spent+purchase.credits);stage=purchase.stage;}
  if(stage!=p.engineStage||p.availableCredits!=p.earnings-spent||p.availableCredits<0)throw new InvalidDataException("Spending/build balance mismatch");
 }
 public TrialReceipt Settle(TrialSession session,out bool duplicate){
  if(session==null||!session.ValidFinish||session.Definition.id!=definition.id)throw new InvalidOperationException("Only an ordered valid finish can pay");
  using(Lock()){
   // Re-read while locked so simultaneous game instances cannot overwrite rewards.
   var latest=Load();foreach(var r in latest.receipts)if(r.runId==session.RunId){Profile=latest;duplicate=true;return r;}
   var candidate=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(latest));int ms=Math.Max(1,Mathf.CeilToInt(session.Elapsed*1000));int rank=definition.Rank(ms/1000f);
   TrialRecord record=candidate.records.Find(r=>r.eventId==definition.id);bool first=record==null;if(first){record=new TrialRecord{eventId=definition.id};candidate.records.Add(record);}
   int rankRep=definition.Reputation(rank);var receipt=new TrialReceipt{runId=session.RunId,eventId=definition.id,completedUtc=DateTime.UtcNow.ToString("o"),milliseconds=ms,rank=rank,firstClear=first,credits=checked(definition.Credits(rank)+(first?definition.firstClearBonus:0)),reputation=Mathf.Max(0,rankRep-record.bestReputation),rewardBase=definition.baseReward,firstBonus=definition.firstClearBonus,creditBps=definition.creditBps[rank],reputationTarget=rankRep,goldMs=Mathf.RoundToInt(definition.goldSeconds*1000),silverMs=Mathf.RoundToInt(definition.silverSeconds*1000),bronzeMs=Mathf.RoundToInt(definition.bronzeSeconds*1000)};
   candidate.earnings=checked(candidate.earnings+receipt.credits);candidate.availableCredits=checked(candidate.availableCredits+receipt.credits);candidate.reputation=checked(candidate.reputation+receipt.reputation);candidate.receipts.Add(receipt);record.completions++;record.firstClearPaid=true;record.bestMilliseconds=record.bestMilliseconds==0?ms:Math.Min(ms,record.bestMilliseconds);record.bestReputation=Math.Max(record.bestReputation,rankRep);Validate(candidate);
   Write(candidate);Profile=candidate;duplicate=false;return receipt;
  }
 }
 public TuneReceipt PurchaseStageOne(string transactionId,EngineTuneOffer offer,int quotedPrice,out bool duplicate){
  if(!Guid.TryParse(transactionId,out _))throw new ArgumentException("Invalid purchase request");offer.Validate();
  using(Lock()){
   var latest=Load();Profile=latest;foreach(var receipt in latest.spending)if(receipt.transactionId==transactionId){if(receipt.configId!=offer.configId||receipt.credits!=quotedPrice)throw new InvalidOperationException("Purchase request no longer matches its receipt.");duplicate=true;return receipt;}
   if(latest.engineStage!=offer.requiredPreviousStage)throw new InvalidOperationException("Stage 1 is already installed.");
   if(quotedPrice!=offer.price)throw new InvalidOperationException("The quote changed. Reopen the garage.");
   if(latest.reputation<offer.minimumReputation)throw new InvalidOperationException("More reputation is required.");
   if(latest.availableCredits<offer.price)throw new InvalidOperationException("Not enough credits for this package.");
   var candidate=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(latest));var paid=new TuneReceipt{transactionId=transactionId,prototypeCarId=candidate.prototypeCarId,pathId=offer.pathId,configId=offer.configId,stage=1,credits=offer.price,paidUtc=DateTime.UtcNow.ToString("o")};candidate.spending.Add(paid);candidate.availableCredits-=offer.price;candidate.engineStage=1;Validate(candidate);Write(candidate);Profile=candidate;duplicate=false;return paid;
  }
 }
 void Write(RaceProgress p){
  string temp=path+".tmp";byte[] data=new UTF8Encoding(false).GetBytes(JsonUtility.ToJson(p,true));using(var stream=new FileStream(temp,FileMode.Create,FileAccess.Write,FileShare.None)){stream.Write(data,0,data.Length);stream.Flush(true);}beforeReplace?.Invoke();
  if(File.Exists(path))File.Replace(temp,path,RecoveredBackup?null:path+".bak");else File.Move(temp,path);RecoveredBackup=false;
 }
}
}

