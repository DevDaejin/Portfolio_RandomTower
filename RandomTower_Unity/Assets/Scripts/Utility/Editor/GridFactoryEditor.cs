using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(GridFactory))]
public class GridFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (GridFactory)target;

        if (GUILayout.Button("Create grid"))
        {
            script.CreateGrid();
            EditorUtility.SetDirty(script);
        }
    }
}
