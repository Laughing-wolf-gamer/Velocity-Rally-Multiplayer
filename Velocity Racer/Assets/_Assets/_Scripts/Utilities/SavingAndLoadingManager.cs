using UnityEngine;
[DefaultExecutionOrder(-20)]
public class SavingAndLoadingManager : MonoBehaviour {

    public static SavingAndLoadingManager Current{get;private set;}
    [SerializeField] private GameSettingsSO gameSettings;
    private void Awake(){
        if(Current == null){
            Current = this;
        }else{
            Destroy(Current.gameObject);
        }
        DontDestroyOnLoad(Current.gameObject);
        LoadGame();
    }
    public void SaveGame(){
        gameSettings.Save();
    }
    public void SaveToCloud(){
        CloudSavingSystem.CloudInstance?.SaveCloudData();
    }
    public void LoadGame(){
        gameSettings.Load();
    }
    public void LoadFromCloud(){
        CloudSavingSystem.CloudInstance?.LoadCloudData();
    }
    public void DeleteAllData(){
        gameSettings.DeleteAllData();
    }
    private void OnApplicationPause(bool pause){
        if(pause){
            SaveGame();
        }
    }
    
    private void OnApplicationQuit(){
        SaveGame();
    }

}