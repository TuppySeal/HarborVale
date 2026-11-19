using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ClaudeGaragePreview;
namespace HarborValePreviewReview.Editor {
[InitializeOnLoad]public static class GaragePlayCheck {
 const string Armed="HarborVale.GarageReview.Play";static bool done,failed;static string result="";static double start;
 static GaragePlayCheck(){if(SessionState.GetBool(Armed,false)){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;Application.logMessageReceived+=Log;start=EditorApplication.timeSinceStartup;}}
 public static void Begin(){SessionState.SetBool(Armed,true);EditorSceneManager.OpenScene(ReviewBuild.GarageScene);EditorApplication.isPlaying=true;}
 static void Log(string m,string s,LogType t){if(t==LogType.Error||t==LogType.Exception){if(s.Contains("UnityEditor.Search.SearchDatabase")){result+="EDITOR K001 search indexing excluded.\n";return;}failed=true;result+="ERROR "+m+"\n";}}
 static void Tick(){if(!EditorApplication.isPlaying||done)return;var m=UnityEngine.Object.FindAnyObjectByType<GaragePreviewManager>();if(m==null)return;try{for(int i=0;i<4;i++){m.Select(i);if(!m.displays[i].activeSelf)throw new Exception("Selection failed");m.ToggleFitment();if(!m.fitted[i])throw new Exception("Mock fit failed");m.ToggleFitment();}m.cameraOrbit.Orbit(15,4);m.cameraOrbit.Zoom(-1);m.cameraOrbit.ResetView();if(m.cameraOrbit.target==null)throw new Exception("Camera target missing");result+="PASS actual Editor Play Mode: four part selections/mock fitments and camera orbit/zoom/reset; no car physics or gameplay bootstrap.\n";}catch(Exception e){failed=true;result+=e+"\n";}done=true;EditorApplication.isPlaying=false;}
 static void State(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredEditMode)return;SessionState.SetBool(Armed,false);File.WriteAllText("Verification/Garage/playmode.txt",result);EditorApplication.Exit(failed?1:0);}
}
}
