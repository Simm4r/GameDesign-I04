using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering.Universal;

public class ConvertAllWallMaterialsToTransparent : EditorWindow
{
    [MenuItem("Tools/Fix Wall Materials to Transparent")]
    public static void ConvertWallMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int converted = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat != null && mat.shader.name == "Universal Render Pipeline/Lit")
            {
                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    EditorUtility.SetDirty(mat);
                    converted++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($" Converted {converted} materials to Transparent.");
    }
}
