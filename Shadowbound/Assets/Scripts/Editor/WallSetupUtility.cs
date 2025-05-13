using UnityEditor;
using UnityEngine;

public class WallSetupUtility : EditorWindow
{
    [MenuItem("Tools/Setup Wall Fading")]
    public static void SetupWallFading()
    {
        string wallLayerName = "WallFade";
        int wallLayer = LayerMask.NameToLayer(wallLayerName);

        if (wallLayer == -1)
        {
            Debug.LogError("Layer 'WallFade' not found. Please create it first.");
            return;
        }

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.TryGetComponent<MeshRenderer>(out _) && obj.TryGetComponent<MeshFilter>(out _))
            {
                Undo.RegisterCompleteObjectUndo(obj, "Setup Wall Fade");

                obj.layer = wallLayer;

                if (!obj.GetComponent<WallFadeController>())
                    obj.AddComponent<WallFadeController>();

                count++;
            }
        }

        Debug.Log($"Wall fade setup completed for {count} objects.");
    }
}
