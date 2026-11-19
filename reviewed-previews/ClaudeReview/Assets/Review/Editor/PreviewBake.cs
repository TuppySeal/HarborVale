using System.IO;
using UnityEngine;
using UnityEditor;
namespace HarborValePreviewReview.Editor {
public static class PreviewBake {
 public static void Materials(string folder){Directory.CreateDirectory(folder);int count=0;foreach(var r in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))foreach(var mat in r.sharedMaterials)if(mat!=null&&!AssetDatabase.Contains(mat))AssetDatabase.CreateAsset(mat,AssetDatabase.GenerateUniqueAssetPath(folder+"/Material_"+(count++).ToString("D4")+".mat"));AssetDatabase.SaveAssets();}
}
}
