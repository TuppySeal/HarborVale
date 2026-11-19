using System;
using System.IO;
using UnityEditor;
namespace HarborValeGreybox.Boost.Editor {
public static class ExportFinchVisuals {
 public static void Run(){string root=Path.GetFullPath("Verification/Boost/ClaudeVisualKit");Directory.CreateDirectory(root);string prefab="Assets/HarborValeGreybox/Expansion/Prefabs/AsterFinch.prefab";var dependencies=AssetDatabase.GetDependencies(prefab,true);foreach(var path in dependencies)if(path.Contains("Driving/")||path.Contains("Racing/")||path.Contains("Garage/"))throw new Exception("Visual kit unexpectedly contains gameplay dependency: "+path);AssetDatabase.ExportPackage(prefab,root+"/AsterFinch_VisualOnly.unitypackage",ExportPackageOptions.IncludeDependencies|ExportPackageOptions.Recurse);File.WriteAllLines(root+"/dependencies.txt",dependencies);File.WriteAllText(root+"/README.md","Original static Aster Finch visual prefab and dependency-preserving assets, exported from Unity 6000.6.0f1. Import only into Claude's independent GaragePreview project. No driving/racing/garage/purchase/save scripts included. StarterCarSpecification is visual dimension metadata. Use original built-in render pipeline. Main project must remain untouched.\n");}
}
}
