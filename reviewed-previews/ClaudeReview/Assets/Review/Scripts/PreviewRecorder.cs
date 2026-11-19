using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
namespace HarborValePreviewReview {
public class PreviewRecorder:MonoBehaviour {
 public string Folder{get;private set;}public bool Recording{get;private set;}
 public static bool SuppressNextFrame;
 List<double> times=new List<double>();double started;int frame;
 public void Begin(string folder){Folder=Path.GetFullPath(folder);Directory.CreateDirectory(Path.Combine(Folder,"frames"));started=Time.realtimeSinceStartupAsDouble;Recording=true;StartCoroutine(Frames());}
 IEnumerator Frames(){while(Recording){yield return new WaitForEndOfFrame();if(SuppressNextFrame){SuppressNextFrame=false;yield return new WaitForSecondsRealtime(.1f);continue;}times.Add(Time.realtimeSinceStartupAsDouble-started);ScreenCapture.CaptureScreenshot(Path.Combine(Folder,"frames","frame_"+(frame++).ToString("D5")+".png"));yield return new WaitForSecondsRealtime(.1f);}}
 public void Finish(){Recording=false;var lines=new List<string>{"ffconcat version 1.0"};for(int i=0;i<times.Count;i++){lines.Add("file 'frames/frame_"+i.ToString("D5")+".png'");double duration=i+1<times.Count?times[i+1]-times[i]:.1;lines.Add("duration "+duration.ToString("F6",CultureInfo.InvariantCulture));}if(times.Count>0)lines.Add("file 'frames/frame_"+(times.Count-1).ToString("D5")+".png'");File.WriteAllLines(Path.Combine(Folder,"frames.ffconcat"),lines);File.WriteAllText(Path.Combine(Folder,"capture.txt"),"Actual native window frames: "+times.Count+"; wall-clock timestamped, not a storyboard. No claim of subjective manual testing.\n");}
}
}
