using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TrackCheckVisual))]
public class TrackCheckPointEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        TrackCheckVisual trackCheckPoint = (TrackCheckVisual)target;
        if(GUILayout.Button("Randomize Visual Colors")){
            trackCheckPoint.GetRandomColorForVisual();
        }
        serializedObject.ApplyModifiedProperties();

    }
}