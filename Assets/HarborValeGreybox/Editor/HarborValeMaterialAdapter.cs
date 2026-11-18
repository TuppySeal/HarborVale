using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace HarborValeGreybox.Editor
{
    public static class HarborValeMaterialAdapter
    {
        [InitializeOnLoadMethod]
        static void OnLoad() { EditorApplication.delayCall += Adapt; }

        [MenuItem("Harbor Vale/Adapt map materials")]
        public static void Adapt()
        {
            if(GraphicsSettings.defaultRenderPipeline==null && QualitySettings.renderPipeline==null)return;
            var shader=Shader.Find("Universal Render Pipeline/Unlit");
            if(shader==null)return;
            bool changed=false;
            foreach(string guid in AssetDatabase.FindAssets("t:Material",new[]{"Assets/HarborValeGreybox/Materials"}))
            {
                var material=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if(material==null || material.shader==shader)continue;
                Color color=material.HasProperty("_Color")?material.GetColor("_Color"):Color.gray;
                material.shader=shader;material.SetColor("_BaseColor",color);
                EditorUtility.SetDirty(material);changed=true;
            }
            if(changed){AssetDatabase.SaveAssets();Debug.Log("Harbor Vale: adapted only map-owned materials to the destination URP pipeline.");}
        }
    }
}
