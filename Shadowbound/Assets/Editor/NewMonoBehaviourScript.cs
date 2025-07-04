using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MeshReadWriteEditor : EditorWindow
{
    private List<Object> meshAssets = new List<Object>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Mesh/Manual Read/Write Toggle")]
    public static void ShowWindow()
    {
        GetWindow<MeshReadWriteEditor>("Mesh Read/Write Toggle");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Trascina qui i tuoi asset mesh o FBX", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Area di trascinamento
        var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Trascina qui gli asset");

        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var dragged in DragAndDrop.objectReferences)
                    {
                        if (!meshAssets.Contains(dragged) && IsMeshAsset(dragged))
                            meshAssets.Add(dragged);
                    }
                    evt.Use();
                }
            }
        }

        // Lista degli asset
        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var asset in meshAssets)
        {
            DrawMeshToggle(asset);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("Svuota lista"))
        {
            meshAssets.Clear();
        }
    }

    private void DrawMeshToggle(Object asset)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

        if (importer == null)
        {
            EditorGUILayout.LabelField($"{asset.name} - ⚠️ Non è un asset importabile");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(asset.name, GUILayout.Width(200));
        bool newReadable = EditorGUILayout.Toggle("Read/Write", importer.isReadable);

        if (newReadable != importer.isReadable)
        {
            importer.isReadable = newReadable;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            Debug.Log($"🔁 {(newReadable ? "Abilitato" : "Disabilitato")} Read/Write su {asset.name}");
        }

        EditorGUILayout.EndHorizontal();
    }

    private bool IsMeshAsset(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        return path.EndsWith(".fbx") || path.EndsWith(".obj") || path.EndsWith(".blend");
    }
}
