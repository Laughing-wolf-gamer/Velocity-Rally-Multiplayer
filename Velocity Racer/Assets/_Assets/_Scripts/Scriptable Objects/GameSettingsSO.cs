using UnityEngine;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "GameSettings", menuName = "Utils/New GameSettings")]
public class GameSettingsSO : ScriptableObject {
    public TrackSO currentTrack;
    public int currentLap = 0;
    public float maxPlayer;
    public bool hasBots = false;
    public float maxWaitingTime = 10;// TODO Make this const before Release
    public GameProifileData gameProfileData;
    public CarBodySO playerCarBody;
    public List<CarBodyFullWorld> AllCarTypes;
    public GameDataSO gameData;
    public string GetCurrentPlayerCar(){
        string carName = string.Empty;
        for (int i = 0; i < AllCarTypes.Count; i++) {
            if(AllCarTypes[i].GetCarTypeShop() == playerCarBody.currentCarType){
                carName = AllCarTypes[i].name;
                break;
            }
        }
        return carName;
    }
    #region Saving and Loading................

    public void Save(){
        gameData.SetCurrentCar(playerCarBody.currentCarType);
        gameProfileData.carTypeShop = (int)gameData.GetCarTypeShop();
        gameData.Save();
        SaveSystemManager.Save<GameProifileData>(gameProfileData,KeysHolder.GAMESETTINGS_SAVE_KEY);
    }

    public void Load(){
        gameData.Load();
        gameProfileData = SaveSystemManager.Load<GameProifileData>(KeysHolder.GAMESETTINGS_SAVE_KEY);
    }

    public void DeleteAllData() {
        SaveSystemManager.DeleteFile(KeysHolder.TIMER_SAVE_KEY);
        SaveSystemManager.DeleteFile(KeysHolder.GAMESETTINGS_SAVE_KEY);
        SaveSystemManager.DeleteFile(KeysHolder.GAMEDATA_SAVE_KEY);
        SaveSystemManager.DeleteFile(KeysHolder.LEVEL_DATA_SAVE_KEY);
    }

    #endregion

}