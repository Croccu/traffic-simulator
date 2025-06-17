using UnityEditor;
using UnityEngine;

public static class RemoveConnectedNodesField
{
    [MenuItem("Tools/Cleanup/Remove connectedNodes from Waypoints")]
    public static void RemoveFieldFromAll()
    {
        int modified = 0;
        var allNodes = Object.FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);

        foreach (var node in allNodes)
        {
            SerializedObject so = new SerializedObject(node);
            SerializedProperty prop = so.FindProperty("connectedNodes");

            if (prop != null)
            {
                so.Update();
                prop.ClearArray();
                so.ApplyModifiedProperties();
                modified++;
            }
        }

        Debug.Log($"✅ Cleared 'connectedNodes' from {modified} WaypointNodes in scene.");
    }
}
