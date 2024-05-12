using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RCLPG))]
public class RCLPGEditor : Editor
{
    RCLPG RCLPGLocal;
    public override void OnInspectorGUI()
    {
        RCLPGLocal = (RCLPG)target as RCLPG;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            RCLPGLocal.Generate();
        }
    }

    private void OnSceneGUI()
    {
        if (RCLPGLocal == null)
        {
            RCLPGLocal = (RCLPG)target as RCLPG;
        }

        RCLPGLocal.ProbeVolume.center = Handles.PositionHandle(RCLPGLocal.ProbeVolume.center, Quaternion.identity);
        Undo.RecordObject(RCLPGLocal, "RCLPGLocal");
    }
}