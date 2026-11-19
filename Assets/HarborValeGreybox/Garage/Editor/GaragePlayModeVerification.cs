using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HarborValeGreybox.Driving;
using HarborValeGreybox.Racing;
namespace HarborValeGreybox.Garage.Editor {
[InitializeOnLoad]public static class GaragePlayModeVerification {
 const string Armed="HarborVale.Garage.PlayCheck";static int phase;static string report="";static bool failed;static double start;
 static GaragePlayModeVerification(){if(SessionState.GetBool(Armed,false)){DocksideTrialDirector.SavePathOverrideForTests=SessionState.GetString(Armed+".Path","");EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;Application.logMessageReceived+=Log;start=EditorApplication.timeSinceStartup;}}
 public static void Begin(){Directory.CreateDirectory("Verification/Garage");SessionState.SetBool(Armed,true);SessionState.SetString(Armed+".Path",Path.GetFullPath("Verification/Garage/Play_"+Guid.NewGuid().ToString("N")+".json"));EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");EditorApplication.isPlaying=true;}
 static void Log(string text,string stack,LogType type){if(type!=LogType.Error&&type!=LogType.Exception)return;if(stack.Contains("UnityEditor.Search.SearchDatabase")){report+="EDITOR-ONLY ISSUE: K001 search indexing.\n";return;}failed=true;report+="ERROR "+text+"\n";}
 static void Check(bool ok,string text){report+=(ok?"PASS ":"FAIL ")+text+"\n";if(!ok)failed=true;}
 static void State(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredEditMode)return;SessionState.SetBool(Armed,false);File.WriteAllText("Verification/Garage/playmode-results.txt",report);EditorApplication.Exit(failed?1:0);}
 static TrialSession Finish(TrialDefinition d){var s=new TrialSession(d);s.Tick(4,Vector3.zero,Vector3.zero);foreach(var g in d.checkpoints)s.Tick(.02f,g.Position-g.Forward,g.Position+g.Forward);return s;}
 static void Tick(){
  if(!EditorApplication.isPlaying){if(EditorApplication.timeSinceStartup-start>120){failed=true;report+="FAIL startup timeout\n";State(PlayModeStateChange.EnteredEditMode);}return;}EditorApplication.QueuePlayerLoopUpdate();var garage=UnityEngine.Object.FindAnyObjectByType<FinchGarage>();if(garage==null||garage.Catalog==null||garage.Trial.Store==null)return;var trial=garage.Trial;var car=trial.vehicle;car.externalInput=true;car.commandedPedal=car.commandedSteer=0;car.commandedHandbrake=false;
  if(phase==0&&Time.timeSinceLevelLoad>2){Check(garage.NearGarage&&car.Engine.definition.SampleTorque(4500)==119,"Stock startup is near the garage with authored curve");Check(garage.Open()&&car.controlsLocked&&trial.ExternalModalOpen,"Garage opens and locks driving/race interaction");Check(!trial.BeginRun(),"Race cannot start over a garage modal");Check(!garage.Purchase()&&trial.Store.Profile.availableCredits==0&&car.InstalledPrototypeStage==0,"Unfunded garage purchase cannot install");garage.Close();trial.Store.Settle(Finish(trial.Definition),out _);Check(garage.Open(),"Garage refresh sees earned credits");Check(garage.Purchase()&&trial.Store.Profile.availableCredits==700&&trial.Store.Profile.engineStage==1,"Purchase commits 400 credits and installs paid stage");Check(car.Engine.definition.SampleTorque(4500)==125&&car.InstalledPrototypeStage==1,"Vehicle uses Stage 1 curve and HUD stage");garage.Purchase();Check(trial.Store.Profile.availableCredits==700,"Duplicate UI request cannot charge again");garage.Close();car.Recover();Check(car.Engine.definition.SampleTorque(4500)==125,"Free-roam recovery preserves installed torque");Check(trial.BeginRun(),"Driving/race input unlock after leaving garage");Check(car.Engine.definition.SampleTorque(4500)==125&&!garage.Open(),"Race reset keeps tune and active race blocks garage");trial.Abandon();trial.DismissResult();Check(car.Engine.definition.SampleTorque(4500)==125,"Abandon/retry does not reset paid build");Check(new RaceProgressStore(trial.Store.PathName,trial.Definition).Profile.engineStage==1,"Installed build reloads from disk");phase=1;}
  if(phase==1&&Time.timeSinceLevelLoad>4){Check(!failed,"No gameplay runtime exceptions");EditorApplication.isPlaying=false;}if(Time.timeSinceLevelLoad>40){failed=true;report+="FAIL test timeout\n";EditorApplication.isPlaying=false;}
 }
}
}
