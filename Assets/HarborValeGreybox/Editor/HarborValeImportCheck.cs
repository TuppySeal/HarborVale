using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HarborValeGreybox.Editor
{
    public static class HarborValeImportCheck
    {
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/HarborValeGreybox/Scenes/HarborVale_CityMap.unity");
            HarborValeMaterialAdapter.Adapt();
            var cam=Camera.main;
            if(cam==null)throw new Exception("Missing map camera");
            var explorer=cam.GetComponent<MapExplorer>();
            if(explorer==null)throw new Exception("Map script GUID or namespace did not import");
            foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
                if(r.sharedMaterial==null||r.sharedMaterial.shader==null||r.sharedMaterial.shader.name=="Hidden/InternalErrorShader")throw new Exception("Invalid material: "+r.name);
            typeof(MapExplorer).GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(explorer,null);
            string evidence=Path.Combine(Application.dataPath,"../Verification");Directory.CreateDirectory(evidence);
            var render=new RenderTexture(1600,1250,24);cam.targetTexture=render;cam.Render();RenderTexture.active=render;
            var image=new Texture2D(1600,1250,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,1250),0,0);image.Apply();
            File.WriteAllBytes(Path.Combine(evidence,"HarborVale_Imported.png"),image.EncodeToPNG());
            cam.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(image);UnityEngine.Object.DestroyImmediate(render);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            File.WriteAllText(Path.Combine(evidence,"import-check.txt"),"PASS: HarborVale project imported in Unity "+Application.unityVersion+". Namespaced map camera found, materials/shaders valid, scene reopened/saved/rendered.\n");
            Debug.Log("HARBOR_VALE_DESTINATION_IMPORT_PASS");
        }
    }
}
