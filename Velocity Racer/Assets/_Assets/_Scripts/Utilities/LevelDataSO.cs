using System.IO;
using UnityEngine;
using GamerWolf.Utils;
using System.Runtime.Serialization.Formatters.Binary;
[CreateAssetMenu(fileName = "Level Data ",menuName = "ScriptableObject/LevelData")]
public class LevelDataSO : ScriptableObject{
    [SerializeField] private LevelData saveData;
    public void SetIsCompleteLevel(bool value){
        saveData.isCompleted = value;
        if(saveData.isCompleted){
            // AnalyticsManager.Current.SeLevelPorgressions(GameAnalyticsSDK.GAProgressionStatus.Complete,this.name,(int)GetCurrentLevelIndex());
        }
    }
    public bool IsCompleted(){
        return saveData.isCompleted;
    }
    public SceneIndex GetCurrentLevelIndex(){
        return saveData.levelIndex;
    }
    #region Saving and Loading................

    [ContextMenu("Save")]
    public void Save(){
        string data = JsonUtility.ToJson(saveData,true);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath,"/",name,"LevelData",".dat"));
        formatter.Serialize(file,data);
        file.Close();
    }

    [ContextMenu("Load")]
    public void Load(){
        if(File.Exists((string.Concat(Application.persistentDataPath,"/",name,"LevelData",".dat")))){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = File.Open(string.Concat(Application.persistentDataPath,"/",name,"LevelData",".dat"),FileMode.Open);
            JsonUtility.FromJsonOverwrite(formatter.Deserialize(Stream).ToString(),saveData);
            Stream.Close();
        }
    }

    #endregion
    [System.Serializable]
    private struct LevelData{
        public bool isCompleted;
        public SceneIndex levelIndex;
    }
}
