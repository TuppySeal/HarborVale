using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using HarborValeGreybox.Garage;
using HarborValeGreybox.Workshop;
namespace HarborValeGreybox.Racing {
public sealed class UnsupportedProgressVersionException:IOException {public UnsupportedProgressVersionException():base("This progress requires a newer game build.") {}}
[Serializable] public class TrialReceipt {public string runId,eventId,completedUtc;public int milliseconds,rank,credits,reputation;public bool firstClear;public int rewardBase,firstBonus,creditBps,reputationTarget,goldMs,silverMs,bronzeMs;}
[Serializable] public class TrialRecord {public string eventId;public int bestMilliseconds,bestReputation,completions;public bool firstClearPaid;}
[Serializable]public class TuneReceipt {public string transactionId,prototypeCarId,pathId,configId,paidUtc,kind,partId,setupJson,fleetJson;public int stage,credits;}
[Serializable] public class RaceProgress {public FleetState fleet;public WorkshopBuild workshop;public int version;public string profileId;public int earnings,reputation;public List<TrialReceipt> receipts=new List<TrialReceipt>();public List<TrialRecord> records=new List<TrialRecord>();public int availableCredits,engineStage,gearboxStage,intercoolerStage;public string prototypeCarId;public List<TuneReceipt> spending=new List<TuneReceipt>();}
// One atomic receipt transaction commits all earned credits and record changes.
// Prototype race earnings deliberately do not fabricate purchase/vehicle history.
public sealed partial class RaceProgressStore {
 readonly string path,legacyPath,olderLegacyPath,oldestLegacyPath,ancientLegacyPath,recentLegacyPath,fitmentLegacyPath,lineupLegacyPath,inventoryLegacyPath,componentsLegacyPath;readonly TrialDefinition definition;readonly Action beforeReplace;
 public RaceProgress Profile{get;private set;}public bool RecoveredBackup{get;private set;}
 public string PathName=>path;
 public RaceProgressStore(string file,TrialDefinition trial,Action faultBeforeReplace=null,string legacyFile=null,string olderLegacyFile=null,string oldestLegacyFile=null,string ancientLegacyFile=null,string recentLegacyFile=null,string fitmentLegacyFile=null,string lineupLegacyFile=null,string inventoryLegacyFile=null,string componentsLegacyFile=null){path=file;legacyPath=legacyFile;olderLegacyPath=olderLegacyFile;oldestLegacyPath=oldestLegacyFile;ancientLegacyPath=ancientLegacyFile;recentLegacyPath=recentLegacyFile;fitmentLegacyPath=fitmentLegacyFile;lineupLegacyPath=lineupLegacyFile;inventoryLegacyPath=inventoryLegacyFile;componentsLegacyPath=componentsLegacyFile;definition=trial;definition.Validate();beforeReplace=faultBeforeReplace;Directory.CreateDirectory(Path.GetDirectoryName(path));using(Lock()){Profile=Load();}}
 FileStream Lock()=>new FileStream(path+".lock",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);
 RaceProgress Read(string file){var p=JsonUtility.FromJson<RaceProgress>(File.ReadAllText(file));if(p!=null&&p.version>10)throw new UnsupportedProgressVersionException();Validate(p);return p;}
 public void Refresh(){using(Lock()){Profile=Load();}}
 RaceProgress Load(){
  if(!File.Exists(path)&&!File.Exists(path+".bak")){
   foreach(string legacy in new[]{componentsLegacyPath,inventoryLegacyPath,lineupLegacyPath,fitmentLegacyPath,recentLegacyPath,legacyPath,olderLegacyPath,oldestLegacyPath,ancientLegacyPath})if(legacy!=null&&(File.Exists(legacy)||File.Exists(legacy+".bak"))){using(new FileStream(legacy+".lock",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None)){try{var p=Read(legacy);RecoveredBackup=false;Write(p);return p;}catch(UnsupportedProgressVersionException){throw;}catch(Exception legacyError){try{var p=Read(legacy+".bak");RecoveredBackup=false;Write(p);return p;}catch(Exception){throw new IOException("Legacy race progress could not be imported. Existing files were preserved.",legacyError);}}}}
   string id=Guid.NewGuid().ToString("N");var created=new RaceProgress{version=10,workshop=new WorkshopBuild(),profileId=id,prototypeCarId=id+":finch",fleet=FleetLedger.Create(id+":finch",new WorkshopBuild())};Validate(created,true);Write(created);return created;
  }
  Exception primary=null;try{var p=Read(path);RecoveredBackup=false;return p;}catch(UnsupportedProgressVersionException){throw;}catch(Exception e){primary=e;}
  try{var p=Read(path+".bak");RecoveredBackup=true;return p;}catch(Exception){throw new IOException("Race progress could not be read. Existing saves were preserved.",primary);}
 }
 void Validate(RaceProgress p,bool deriveWorkshop=false){
  if(p==null||p.version<1||p.version>10||!Guid.TryParse(p.profileId,out _)||p.receipts==null||p.records==null||p.earnings<0||p.reputation<0||p.receipts.Count>100000)throw new InvalidDataException("Invalid race profile");
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
  if(p.version==2){if(p.gearboxStage!=0||p.intercoolerStage!=0||p.engineStage>1)throw new InvalidDataException("Invalid legacy build");p.version=3;}
  if(p.spending==null||p.prototypeCarId!=p.profileId+":finch"||p.engineStage<0||p.engineStage>2||p.gearboxStage<0||p.gearboxStage>1||p.intercoolerStage<0||p.intercoolerStage>1)throw new InvalidDataException("Invalid installed build");
  int originalVersion=p.version;bool migrate=p.version<10;var catalog=WorkshopCatalog.Load("car_finch");if(originalVersion<6)FitmentTuning.NormalizeLegacy(p.workshop);if(originalVersion<8)VehicleLineup.NormalizeLegacy(p.fleet);if(originalVersion<9)EngineInventoryLedger.ValidateLegacy(p.fleet);if(originalVersion<10){ComponentInventoryLedger.ValidateLegacy(p.fleet);if(CircuitLedger.Pending(p.fleet))throw new InvalidDataException("Legacy profile has unsupported circuit data");}var build=new WorkshopBuild();var fleet=FleetLedger.Create(p.prototypeCarId,build);int spent=0;var transactions=new HashSet<string>();foreach(var purchase in p.spending){
   if(purchase==null||!Guid.TryParse(purchase.transactionId,out _)||!transactions.Add(purchase.transactionId)||(purchase.credits<0&&purchase.kind!="duel_result"&&purchase.kind!="circuit_result")||(purchase.credits==0&&purchase.kind!="setup"&&purchase.kind!="fitment"&&!FleetLedger.IsFleet(purchase)&&!EngineInventoryLedger.IsReceipt(purchase)&&!ComponentInventoryLedger.IsReceipt(purchase)&&purchase.kind!="circuit_open"&&purchase.kind!="circuit_result"))throw new InvalidDataException("Invalid part receipt");
   if(purchase.kind=="circuit_open"||purchase.kind=="circuit_result"){if(originalVersion<10||purchase.prototypeCarId!=p.prototypeCarId)throw new InvalidDataException("Unexpected circuit receipt");ComponentInventoryLedger.Ensure(fleet);CircuitLedger.Apply(fleet,purchase,p.profileId);spent=checked(spent+purchase.credits);continue;}if(ComponentInventoryLedger.IsReceipt(purchase)){if(originalVersion<10||purchase.prototypeCarId!=p.prototypeCarId)throw new InvalidDataException("Unexpected component receipt");ComponentInventoryLedger.Replay(fleet,purchase,p.profileId);spent=checked(spent+purchase.credits);continue;}if(purchase.pathId=="workshop_v10"){if(originalVersion<10)throw new InvalidDataException("Unexpected component-era shop receipt");ComponentInventoryLedger.Ensure(fleet);}if(EngineInventoryLedger.IsReceipt(purchase)){if(originalVersion<9||purchase.prototypeCarId!=p.prototypeCarId)throw new InvalidDataException("Unexpected engine inventory receipt");EngineInventoryLedger.Replay(fleet,purchase);spent=checked(spent+purchase.credits);continue;}if(purchase.pathId=="workshop_v9"){if(originalVersion<9)throw new InvalidDataException("Unexpected inventory-era part receipt");EngineInventoryLedger.Ensure(fleet);}if(FleetLedger.IsFleet(purchase)){if(originalVersion<7||originalVersion<8&&purchase.kind=="fleet_car")throw new InvalidDataException("Unexpected fleet history");FleetLedger.Replay(fleet,purchase,p.profileId);EngineInventoryLedger.AfterFleet(fleet,purchase);spent=checked(spent+purchase.credits);continue;}var target=FleetLedger.Find(fleet,purchase.prototypeCarId);if(target.owner!="player"||fleet.HasPending)throw new InvalidDataException("Vehicle was not available for modification");var targetBuild=target.build;var targetCatalog=WorkshopCatalog.ForBuild(VehicleLineup.Model(target),targetBuild);string kind=string.IsNullOrEmpty(purchase.kind)?"engine":purchase.kind;string id=null;
   if(kind=="engine"&&purchase.stage==1&&purchase.pathId=="path_finch_na"&&purchase.configId=="cfg_finch_na_s1")id="na_1";
   else if(kind=="gearbox"&&purchase.stage==1&&purchase.configId=="tx_finch_s1")id="gearbox_1";
   else if(kind=="engine"&&purchase.stage==2&&purchase.pathId=="path_finch_turbo"&&purchase.configId=="cfg_finch_turbo_s2")id="induction_small_1";
   else if(kind=="intercooler"&&purchase.stage==1&&purchase.pathId=="path_finch_turbo"&&purchase.configId=="cfg_finch_turbo_s2_ic1")id="cooling_1";
   else if(kind=="workshop"&&originalVersion>=4&&purchase.configId==purchase.partId&&purchase.pathId=="workshop_v4"){id=purchase.partId;if(FitmentTuning.IsNew(targetCatalog.Find(id))||targetCatalog.Find(id).stage!=purchase.stage)throw new InvalidDataException("Receipt stage mismatch");}
   else if(kind=="workshop"&&originalVersion>=6&&purchase.configId==purchase.partId&&purchase.pathId=="workshop_v6"){id=purchase.partId;if(!FitmentTuning.IsNew(targetCatalog.Find(id))||targetCatalog.Find(id).stage!=purchase.stage)throw new InvalidDataException("Fitment receipt stage mismatch");}
   if(kind=="fitment"&&originalVersion>=6&&(purchase.pathId=="workshop_v6"||originalVersion>=9&&(purchase.pathId=="workshop_v9"||originalVersion>=10&&purchase.pathId=="workshop_v10"))&&purchase.configId=="tire_fit_v1"&&purchase.stage==0&&purchase.credits==0){var tire=targetCatalog.Find(purchase.partId);if(tire.slot!="tirecompound"||!FitmentTuning.Owns(targetBuild,tire.choice))throw new InvalidDataException("Cannot refit an unowned tire set");targetCatalog.Apply(targetBuild,tire);EngineInventoryLedger.Sync(fleet,target);continue;}
   if(kind=="setup"&&originalVersion>=5&&purchase.credits==0&&purchase.stage==0&&purchase.partId=="chassis_setup"&&purchase.configId=="setup_v1"&&purchase.pathId=="workshop_v5"){
    var setup=JsonUtility.FromJson<WorkshopSetup>(purchase.setupJson);if(setup==null||JsonUtility.ToJson(setup)!=purchase.setupJson)throw new InvalidDataException("Invalid setup receipt");ChassisTuning.ApplySetup(targetCatalog,targetBuild,setup);continue;
   }
   if(kind=="workshop"&&originalVersion>=9&&(purchase.pathId=="workshop_v9"||originalVersion>=10&&purchase.pathId=="workshop_v10")&&purchase.configId==purchase.partId){id=purchase.partId;if(targetCatalog.Find(id).stage!=purchase.stage)throw new InvalidDataException("Inventory-era receipt stage mismatch");}if(id==null)throw new InvalidDataException("Unsupported part receipt");if(purchase.credits<=0)throw new InvalidDataException("Paid receipt requires a positive amount");if(targetCatalog.Find(id).slot=="tirecompound"&&FitmentTuning.Owns(targetBuild,targetCatalog.Find(id).choice))throw new InvalidDataException("Owned tire refit cannot be charged");try{targetCatalog.Apply(targetBuild,targetCatalog.Find(id));}catch(Exception e){throw new InvalidDataException("Part receipt skips prerequisite or is incompatible",e);}EngineInventoryLedger.Sync(fleet,target);ComponentInventoryLedger.AfterWorkshop(fleet,target,targetCatalog.Find(id),purchase.transactionId);spent=checked(spent+purchase.credits);
  }
  int stage=build.induction!="none"?2:build.naStage>0?1:0,gearbox=Math.Min(1,build.gearboxStage),cooler=Math.Min(1,build.coolingStage);
  if(deriveWorkshop){p.engineStage=stage;p.gearboxStage=gearbox;p.intercoolerStage=cooler;}if(stage!=p.engineStage||gearbox!=p.gearboxStage||cooler!=p.intercoolerStage||p.availableCredits!=p.earnings-spent||p.availableCredits<0)throw new InvalidDataException("Spending/build balance mismatch");
  if(migrate&&originalVersion>=4&&!deriveWorkshop){if(p.workshop!=null&&p.workshop.setup==null)p.workshop.setup=new WorkshopSetup();if(p.workshop==null||JsonUtility.ToJson(p.workshop)!=JsonUtility.ToJson(build))throw new InvalidDataException("Legacy paid build does not match history");}
  if(migrate&&originalVersion>=7&&!deriveWorkshop&&(p.fleet==null||JsonUtility.ToJson(p.fleet)!=JsonUtility.ToJson(fleet)))throw new InvalidDataException("Legacy fleet ownership disagrees with history");EngineInventoryLedger.Ensure(fleet);ComponentInventoryLedger.Ensure(fleet);EngineInventoryLedger.Validate(fleet);ComponentInventoryLedger.Validate(fleet);if(migrate||deriveWorkshop){p.workshop=build;p.version=10;}else if(p.workshop==null||JsonUtility.ToJson(p.workshop)!=JsonUtility.ToJson(build))throw new InvalidDataException("Workshop state disagrees with immutable receipt history");if(originalVersion<10||deriveWorkshop)p.fleet=fleet;else if(p.fleet==null||JsonUtility.ToJson(p.fleet)!=JsonUtility.ToJson(fleet))throw new InvalidDataException("Fleet ownership disagrees with history");

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
  if(offer.Kind!="engine"||offer.stage!=1)throw new ArgumentException("Use a matching part request");return PurchasePart(transactionId,offer,quotedPrice,out duplicate);
 }
 public TuneReceipt PurchasePart(string transactionId,EngineTuneOffer offer,int quotedPrice,out bool duplicate){
  if(!Guid.TryParse(transactionId,out _))throw new ArgumentException("Invalid purchase request");offer.Validate();
  using(Lock()){
   var latest=Load();Profile=latest;foreach(var receipt in latest.spending)if(receipt.transactionId==transactionId){if(receipt.configId!=offer.configId||receipt.credits!=quotedPrice)throw new InvalidOperationException("Purchase request no longer matches its receipt.");duplicate=true;return receipt;}
   int current=offer.Kind=="gearbox"?latest.gearboxStage:offer.Kind=="intercooler"?latest.intercoolerStage:latest.engineStage;
   if(current>=offer.stage)throw new InvalidOperationException("This part is already installed.");
   if(current!=offer.requiredPreviousStage||latest.engineStage<offer.requiredEngineStage)throw new InvalidOperationException("Buy the preceding engine/hardware stage first.");
   if(latest.gearboxStage<offer.supportingTransmission)throw new InvalidOperationException("Gearbox support Stage 1 is required.");
   if(quotedPrice!=offer.price)throw new InvalidOperationException("The quote changed. Reopen the garage.");
   if(latest.reputation<offer.minimumReputation)throw new InvalidOperationException("More reputation is required.");
   if(latest.availableCredits<offer.price)throw new InvalidOperationException("Not enough credits for this package.");
   var candidate=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(latest));var paid=new TuneReceipt{transactionId=transactionId,prototypeCarId=candidate.prototypeCarId,pathId=offer.pathId,configId=offer.configId,stage=offer.stage,kind=offer.Kind,partId=offer.PartId,credits=offer.price,paidUtc=DateTime.UtcNow.ToString("o")};candidate.spending.Add(paid);candidate.availableCredits-=offer.price;if(offer.Kind=="gearbox")candidate.gearboxStage=offer.stage;else if(offer.Kind=="intercooler")candidate.intercoolerStage=offer.stage;else candidate.engineStage=offer.stage;Validate(candidate,true);Write(candidate);Profile=candidate;duplicate=false;return paid;
  }
 }
 public TuneReceipt PurchaseWorkshop(string request,string partId,int quotedPrice,out bool duplicate){
  if(!Guid.TryParse(request,out _))throw new ArgumentException("Invalid purchase request");
  using(Lock()){
   var latest=Load();var catalog=WorkshopCatalog.ForBuild(latest.fleet.selectedId==FleetLedger.Loaner?"car_finch":VehicleLineup.Model(FleetLedger.Find(latest.fleet,latest.fleet.selectedId)),ActiveWorkshopOf(latest));var offer=catalog.Find(partId);Profile=latest;foreach(var receipt in latest.spending)if(receipt.transactionId==request){if(receipt.partId!=partId||receipt.credits!=quotedPrice)throw new InvalidOperationException("Request no longer matches receipt");duplicate=true;return receipt;}
   var reason=catalog.Requirement(ActiveWorkshopOf(latest),offer);if(reason!=null)throw new InvalidOperationException(reason);
   int price=catalog.Price(ActiveWorkshopOf(latest),offer);if(quotedPrice!=price)throw new InvalidOperationException("Quote changed. Reopen workshop.");if(latest.availableCredits<price)throw new InvalidOperationException("Not enough credits.");
   FleetLedger.CanModify(latest.fleet);var candidate=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(latest));catalog.Apply(ActiveWorkshopOf(candidate),offer);candidate.engineStage=candidate.workshop.induction!="none"?2:candidate.workshop.naStage>0?1:0;candidate.gearboxStage=Math.Min(1,candidate.workshop.gearboxStage);candidate.intercoolerStage=Math.Min(1,candidate.workshop.coolingStage);
   var paid=new TuneReceipt{transactionId=request,prototypeCarId=candidate.fleet.selectedId,partId=partId,configId=price==0?"tire_fit_v1":partId,pathId="workshop_v10",kind=price==0?"fitment":"workshop",stage=price==0?0:offer.stage,credits=price,paidUtc=DateTime.UtcNow.ToString("o")};candidate.spending.Add(paid);candidate.availableCredits=checked(candidate.availableCredits-price);Validate(candidate,true);Write(candidate);Profile=candidate;duplicate=false;return paid;
  }
 }
 public TuneReceipt SaveWorkshopSetup(string request,WorkshopSetup setup,out bool duplicate){
  if(!Guid.TryParse(request,out _))throw new ArgumentException("Invalid setup request");string payload=JsonUtility.ToJson(setup);
  using(Lock()){var latest=Load();var catalog=WorkshopCatalog.ForBuild(latest.fleet.selectedId==FleetLedger.Loaner?"car_finch":VehicleLineup.Model(FleetLedger.Find(latest.fleet,latest.fleet.selectedId)),ActiveWorkshopOf(latest));Profile=latest;foreach(var r in latest.spending)if(r.transactionId==request){if(r.kind!="setup"||r.setupJson!=payload)throw new InvalidOperationException("Setup request conflicts with saved action");duplicate=true;return r;}
   ChassisTuning.Validate(catalog,ActiveWorkshopOf(latest),setup);if(JsonUtility.ToJson(ActiveWorkshopOf(latest).setup)==payload)throw new InvalidOperationException("This setup is already applied");FleetLedger.CanModify(latest.fleet);var candidate=JsonUtility.FromJson<RaceProgress>(JsonUtility.ToJson(latest));ChassisTuning.ApplySetup(catalog,ActiveWorkshopOf(candidate),setup);var receipt=new TuneReceipt{transactionId=request,prototypeCarId=candidate.fleet.selectedId,kind="setup",partId="chassis_setup",configId="setup_v1",pathId="workshop_v5",setupJson=payload,credits=0,stage=0,paidUtc=DateTime.UtcNow.ToString("o")};candidate.spending.Add(receipt);Validate(candidate,true);Write(candidate);Profile=candidate;duplicate=false;return receipt;
  }
 }
 void Write(RaceProgress p){
  string temp=path+".tmp";byte[] data=new UTF8Encoding(false).GetBytes(JsonUtility.ToJson(p,true));using(var stream=new FileStream(temp,FileMode.Create,FileAccess.Write,FileShare.None)){stream.Write(data,0,data.Length);stream.Flush(true);}beforeReplace?.Invoke();
  if(File.Exists(path))File.Replace(temp,path,RecoveredBackup?null:path+".bak");else File.Move(temp,path);RecoveredBackup=false;
 }
}
}
