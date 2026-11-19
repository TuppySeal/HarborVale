using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Garage;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Boost.Editor {
[InitializeOnLoad]public static class BoostPlayModeVerification {
 const string Armed="HarborVale.Boost.PlayCheck";static int phase;static bool failed;static string report="";static double started;
 static BoostPlayModeVerification(){if(SessionState.GetBool(Armed,false)){DocksideTrialDirector.SavePathOverrideForTests=SessionState.GetString(Armed+".Path","");EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;Application.logMessageReceived+=Log;started=EditorApplication.timeSinceStartup;}}
 public static void Begin(){Directory.CreateDirectory("Verification/Boost");SessionState.SetBool(Armed,true);SessionState.SetString(Armed+".Path",Path.GetFullPath("Verification/Boost/Play_"+Guid.NewGuid().ToString("N")+".json"));EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");EditorApplication.isPlaying=true;}
 static void Log(string text,string stack,LogType type){if(type!=LogType.Error&&type!=LogType.Exception)return;if(stack.Contains("UnityEditor.Search.SearchDatabase")){report+="EDITOR-ONLY ISSUE K001 search indexing.\n";return;}failed=true;report+="ERROR "+text+"\n";}
 static void Check(bool ok,string text){report+=(ok?"PASS ":"FAIL ")+text+"\n";if(!ok)failed=true;}
 static void State(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredEditMode)return;SessionState.SetBool(Armed,false);File.WriteAllText("Verification/Boost/playmode-results.txt",report);EditorApplication.Exit(failed?1:0);}
 static TrialSession Finish(TrialDefinition d){var s=new TrialSession(d);s.Tick(4,Vector3.zero,Vector3.zero);s.Tick(90,Vector3.zero,Vector3.zero);foreach(var g in d.checkpoints)s.Tick(.02f,g.Position-g.Forward,g.Position+g.Forward);return s;}
 static void Tick(){
  if(!EditorApplication.isPlaying){if(EditorApplication.timeSinceStartup-started>120){failed=true;report+="FAIL startup timeout\n";State(PlayModeStateChange.EnteredEditMode);}return;}EditorApplication.QueuePlayerLoopUpdate();var garage=UnityEngine.Object.FindAnyObjectByType<FinchGarage>();if(garage==null||garage.Catalog==null||garage.Trial.Store==null)return;var trial=garage.Trial;var car=trial.vehicle;car.externalInput=true;car.commandedPedal=car.commandedSteer=0;car.commandedHandbrake=false;
  if(phase==0&&Time.timeSinceLevelLoad>2){for(int i=0;i<4;i++)trial.Store.Settle(Finish(trial.Definition),out _);Check(garage.Open()&&car.controlsLocked&&!trial.BeginRun(),"One-location shop locks car and excludes races");garage.Select(2);Check(!garage.BuySelected()&&trial.Store.Profile.engineStage==0,"UI rejects turbo without paid prerequisites");garage.Select(0);Check(garage.BuySelected(),"Stage 1 bought");garage.Select(1);Check(garage.BuySelected()&&Mathf.Abs(car.Engine.definition.shiftSeconds-.396f)<.0001f,"Gearbox purchase changes actual shift duration");garage.Select(2);Check(garage.BuySelected()&&car.Engine.definition.IsTurbo&&car.Engine.definition.SampleTorque(4500)==158,"Turbo kit installs complete boosted/off-boost configuration");garage.Select(3);Check(garage.BuySelected()&&car.Engine.definition.SampleTorque(4500)==162&&trial.Store.Profile.availableCredits==250,"Independent street intercooler installs matched curve and correct debit");garage.BuySelected();Check(trial.Store.Profile.availableCredits==250,"Duplicate selected-part request cannot debit again");garage.Close();car.Recover();Check(car.Engine.definition.IsTurbo&&car.Engine.definition.SampleTorque(4500)==162&&Mathf.Abs(car.Engine.definition.shiftSeconds-.396f)<.0001f,"Recovery preserves turbo, cooler and supporting gearbox");Check(trial.BeginRun()&&!garage.Open(),"Race start preserves build and excludes shop");Check(car.Engine.definition.SampleTorque(4500)==162&&car.Engine.TurboAvailability==0,"Race reset clears transient spool, not paid hardware");trial.Abandon();trial.DismissResult();var p=new RaceProgressStore(trial.Store.PathName,trial.Definition).Profile;Check(p.engineStage==2&&p.gearboxStage==1&&p.intercoolerStage==1&&p.availableCredits==250,"Disk profile reload retains complete build and receipts");phase=1;}
  if(phase==1&&Time.timeSinceLevelLoad>4){Check(!failed,"No gameplay runtime exceptions");EditorApplication.isPlaying=false;}if(Time.timeSinceLevelLoad>45){failed=true;report+="FAIL runtime timeout\n";EditorApplication.isPlaying=false;}
 }
}
}
