using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeMemoryCleaner
{
    static PlayModeMemoryCleaner()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Quando si esce dal play mode (PlayModeStateChange.ExitingPlayMode)
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Play Mode is exiting, cleaning memory...");
            
            // Libera risorse non più referenziate (texture, mesh, ecc.)
            EditorUtility.UnloadUnusedAssetsImmediate();
            
            // Forza garbage collection del managed heap
            System.GC.Collect();
        }
    }
}