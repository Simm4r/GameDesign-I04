using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshReadWriteCleaner : EditorWindow
{
    [MenuItem("Tools/Mesh Read/Write Cleaner")]
    public static void ShowWindow()
    {
        if (EditorUtility.DisplayDialog(
            "Disabilita Read/Write",
            "Vuoi davvero disabilitare il flag Read/Write su tutte le mesh del progetto? (solo file .fbx, .obj, ecc.)",
            "Sì, procedi",
            "Annulla"))
        {
            ProcessAllMeshes();
        }
    }

    private static void ProcessAllMeshes()
    {
        string[] meshGuids = AssetDatabase.FindAssets("t:Model"); // Cerca tutti i modelli
        int changedCount = 0;

        foreach (string guid in meshGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null && importer.isReadable)
            {
                importer.isReadable = false;
                importer.SaveAndReimport();
                changedCount++;
            }
        }

        EditorUtility.DisplayDialog("Completato",
            $"✅ Disabilitato Read/Write su {changedCount} mesh!", "OK");
    }
}