using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SavingAndLoadingManager))]
public class SavingAndLoadingEditor : Editor{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        SavingAndLoadingManager saveAndLoad = (SavingAndLoadingManager)target;
        if(GUILayout.Button("Save Data")){
            saveAndLoad.SaveGame();
        }
        if(GUILayout.Button("Load Data")){
            saveAndLoad.LoadGame();
        }
        if(GUILayout.Button("Delete Saved Data")){
            saveAndLoad.DeleteAllData();
        }
        serializedObject.ApplyModifiedProperties();

    }
    public override void SaveChanges() {
        SavingAndLoadingManager saveAndLoad = (SavingAndLoadingManager)target;
        if(saveAndLoad != null){
            saveAndLoad.SaveGame();
        }
        base.SaveChanges();
        Debug.Log("Data Changed");

    }
}
