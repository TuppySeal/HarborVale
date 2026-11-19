using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Garage;
using HarborValeGreybox.Racing;
using HarborValeGreybox.SportsTest;
namespace HarborValeGreybox.Workshop {
public class ExpandedWorkshop:MonoBehaviour {
 public WorkshopCatalog Catalog{get;private set;}public WorkshopBuild Build=>IsTest?testBuild:garage?.Trial?.Store?.Profile.workshop;public FinchVehicle Car{get;private set;}public bool IsTest{get;private set;}public bool IsOpen{get;private set;}public bool Ready{get;private set;}public int Credits=>IsTest?0:garage?.Trial?.Store?.Profile.availableCredits??0;public string Message{get;private set;}public string SelectedId{get;set;}="na_1";public string Signature=>JsonUtility.ToJson(Build);
 public TurboReleaseMode TurboRelease{get;private set;}public string TurboAudioError{get;private set;}string audioPreferencePath;
 public ExpandedWorkshopView View{get;private set;}FinchGarage garage;ApexTestDrive sports;WorkshopBuild testBuild;bool hudEnabled;WorkshopEffects effects;
 public static void Hook(){var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;if(scene=="HarborVale_CityMap"||scene=="HarborVale_SportsTest")new GameObject("Expanded workshop | one parts counter").AddComponent<ExpandedWorkshop>();}
 IEnumerator Start(){IsTest=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="HarborVale_SportsTest";if(IsTest){while((sports=UnityEngine.Object.FindAnyObjectByType<ApexTestDrive>())==null||!sports.Ready)yield return null;Car=sports.Car;testBuild=new WorkshopBuild();}else{while((garage=UnityEngine.Object.FindAnyObjectByType<FinchGarage>())==null||garage.Catalog==null)yield return null;Car=garage.Trial.vehicle;if(garage.Trial.Store==null)yield break;}
  Catalog=WorkshopCatalog.Load(Car.PowertrainCarId);View=new ExpandedWorkshopView(this);effects=Car.gameObject.AddComponent<WorkshopEffects>();Car.gameObject.AddComponent<BurnoutEffects>();if(!IsTest){audioPreferencePath=garage.Trial.Store.PathName+".turbo-audio.json";try{TurboRelease=TurboAudioPreferenceStore.Load(audioPreferencePath);}catch(Exception e){TurboAudioError=e.Message;}}effects.SetReleaseMode(TurboRelease);Ready=true;Apply();
 }
 public bool Open(){if(!Ready||Car.SpeedKph>=5||!Car.playerInput||Car.controlsLocked)return false;if(!IsTest&&(garage.IsOpen||!garage.NearGarage||garage.Trial.Active||garage.Trial.ShowingResult))return false;
  if(!IsTest){try{garage.Trial.Store.Refresh();Apply();}catch(Exception e){Message=e.Message;return false;}garage.Trial.ExternalModalOpen=true;}
  var hud=Car.GetComponent<FinchDrivingHUD>();hud?.SetInspection(false);if(hud!=null){hudEnabled=hud.enabled;hud.enabled=false;}Car.controlsLocked=true;IsOpen=true;Message=null;View.Open();return true;
 }
 public void Close(){if(!IsOpen)return;IsOpen=false;effects?.StopPreview();View?.Close();if(Car!=null){Car.controlsLocked=false;var hud=Car.GetComponent<FinchDrivingHUD>();if(hud!=null)hud.enabled=hudEnabled;}if(garage!=null&&garage.Trial!=null)garage.Trial.ExternalModalOpen=false;}
 public string Requirement(WorkshopOffer o){var reason=Catalog.Requirement(Build,o);if(reason!=null)return reason;if(!IsTest&&Credits<o.price)return "Need "+(o.price-Credits).ToString("N0")+" more credits";return null;}
 public int TotalPathCost(WorkshopOffer o)=>Catalog.MissingPath(Build,o).Sum(x=>x.price);
 public bool InstallVisibleSelection()=>View!=null&&View.Visible().Any(o=>o.id==SelectedId)&&Install(SelectedId);
 public bool GuideToRequirement(){var offer=Catalog.Find(SelectedId);var path=Catalog.MissingPath(Build,offer);if(path.Count<2)return false;SelectedId=path[0].id;View.Reveal(SelectedId);Message=null;return true;}
 public bool Install(string id){if(!IsOpen||Car.SpeedKph>=5)return false;try{var offer=Catalog.Find(id);if(IsTest){Catalog.Apply(testBuild,offer);}else{garage.Trial.Store.PurchaseWorkshop(Guid.NewGuid().ToString("N"),id,offer.price,out _);}Apply();Message=offer.name+" installed. "+(IsTest?"Temporary test configuration.":"Payment and build saved.");return true;}catch(Exception e){Message=e.Message;return false;}}
 void Apply(){float grip=Catalog.car.gripModifiers[Build.tireStage];Car.forwardGrip=(IsTest?1.70f:1.15f)*grip;Car.lateralGrip=(IsTest?1.65f:1.10f)*grip;Car.InstallPowertrain(Catalog.Resolve(Build),Build.induction!="none"?2:Build.naStage>0?1:0);ChassisTuning.ApplyVehicle(Catalog,Build,Car);effects?.Configure(Car,Build);}
 public bool ChooseTurboRelease(TurboReleaseMode mode){if(!IsOpen||Car.SpeedKph>=5)return false;try{if(!IsTest)TurboAudioPreferenceStore.Save(audioPreferencePath,mode);TurboRelease=mode;effects.SetReleaseMode(mode);TurboAudioError=null;Message=IsTest?"Turbo sound selected for this test visit.":"Turbo sound saved. No credits charged.";return true;}catch(Exception e){TurboAudioError=e.Message;Message=e.Message;return false;}}
 public void StopTurboPreview()=>effects?.StopPreview();
 public void PreviewTurboRelease(TurboReleaseMode mode){if(IsOpen)effects.PreviewRelease(mode);}
 public bool SaveSetup(WorkshopSetup draft){if(!IsOpen||Car.SpeedKph>=5)return false;try{if(IsTest)ChassisTuning.ApplySetup(Catalog,testBuild,draft);else garage.Trial.Store.SaveWorkshopSetup(Guid.NewGuid().ToString("N"),draft,out _);Apply();Message=IsTest?"Test setup applied.":"Setup saved. No credits charged.";return true;}catch(Exception e){Message=e.Message;return false;}}
 public void SyncCareer(){if(Ready&&!IsTest)Apply();}
 public void SyncSports(int step){if(!Ready||!IsTest)return;testBuild=Catalog.FromLegacy(step>=3?2:step>=1?1:0,step>=2?1:0,step>=4?1:0);Apply();}
 void Update(){if(!Ready)return;if(IsOpen&&View.Typing){if(FinchInput.Shortcut(KeyCode.Escape))View.EndSearch();return;}if(IsOpen&&(View.EditingSetup||View.EditingTurboAudio)){if(FinchInput.Shortcut(KeyCode.Escape)||FinchInput.Shortcut(KeyCode.G))Close();return;}if(FinchInput.Shortcut(KeyCode.G)){if(IsOpen)Close();else Open();}else if(IsOpen&&FinchInput.Shortcut(KeyCode.Escape))Close();else if(IsOpen&&FinchInput.Shortcut(KeyCode.DownArrow))View.Advance(1);else if(IsOpen&&FinchInput.Shortcut(KeyCode.UpArrow))View.Advance(-1);else if(IsOpen&&FinchInput.Shortcut(KeyCode.Return))InstallVisibleSelection();}
 void OnGUI(){if(!Ready)return;if(IsOpen){View.Draw();return;}if(IsTest||(!garage.IsOpen&&garage.NearGarage&&!garage.Trial.Active&&!garage.Trial.ShowingResult))GUI.Box(new Rect(16,128,690,27),"G — WORKSHOP | "+(IsTest?"All staged parts free for this test visit":"Staged parts, tuning and your saved build"));}
 void OnDestroy(){Close();View?.Dispose();}
}
}
