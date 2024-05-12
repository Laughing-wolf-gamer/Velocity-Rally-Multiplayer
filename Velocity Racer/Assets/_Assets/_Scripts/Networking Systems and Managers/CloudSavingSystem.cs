using System;
using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave.Models;
public class CloudSavingSystem : MonoBehaviour {
    public Action<bool> OnAuthSuccessFull;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private LevelSystemSO levelData;
    [SerializeField] private CarBodySO playerCarBody;
    public static CloudSavingSystem CloudInstance{get;private set;}
    private async void Awake(){
        if(CloudInstance == null){
            CloudInstance = this;
        }else{
            Destroy(CloudInstance.gameObject);
        }
        DontDestroyOnLoad(CloudInstance.gameObject);
        if(UnityServices.State != ServicesInitializationState.Initialized){
            await UnityServices.InitializeAsync();
        }
    }
    public async void SignInAnonymus(){
        await SiginAnonymusTask();
    }
    private async Task SiginAnonymusTask(){
        bool signinSucces = false;
        try{
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed In Anonymuly by " + AuthenticationService.Instance.PlayerId);
            LoadCloudData();
            signinSucces = true;
        }catch (AuthenticationException aEx) {
            Debug.Log("Signed In Failed");
            Debug.LogException(aEx);
            signinSucces = false;
        }finally{
            OnAuthSuccessFull?.Invoke(signinSucces);
        }
    }

    /* #region Sginin..using Facebook....
    public async void SighInFromFacebook(string fbToken){
        await SignInWithFB(fbToken);
    }
    private async Task SignInWithFB(string fbToken){
        bool signinSucces = false;
        try{
            SignInOptions options = new SignInOptions { CreateAccount = true };
            await AuthenticationService.Instance.SignInWithFacebookAsync(fbToken,options);
            Debug.Log("Signed In Anonymuly by " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Logged in Successfully from Facebook to Unity Game Services.");
            LoadCloudData();
            signinSucces = true;
        }catch (AuthenticationException aEx) {
            Debug.Log("Signed In Failed");
            Debug.LogException(aEx);
            signinSucces = false;
        }catch(RequestFailedException rEx){
            signinSucces = false;
            Debug.LogException(rEx);
        }finally{
            OnAuthSuccessFull?.Invoke(signinSucces);
        }
    }

    #endregion */


    public async void SignInGooglePlay(string authCode){
        await SignInWithGooglePlayGamesAsync(authCode);
    }
    private async Task SignInWithGooglePlayGamesAsync(string authCode) {
        bool signinSucces = false;
        try {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            signinSucces = true;
            LoadCloudData();
            Debug.Log("SignIn is successful.");
        } catch (AuthenticationException ex) {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            signinSucces = false;
            Debug.LogException(ex);
        } catch (RequestFailedException ex) {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            signinSucces = false;
            Debug.LogException(ex);
        }finally{
            OnAuthSuccessFull?.Invoke(signinSucces);
        }
    }



    #region CLOUD SAVING.......

    public async void SaveCloudData(){
        gameData.SetCurrentCar(playerCarBody.currentCarType);
        gameSettings.gameProfileData.carTypeShop = (int)playerCarBody.currentCarType;
        CloudSaveData dataToSave = new CloudSaveData {
            cloudGameSaveData = gameData.saveData,
            levelSystemSaveData = levelData.levelSystemSaveData,
            cloudGameProfileSaveData = gameSettings.gameProfileData,
        };
        
        await SaveToCloudTask(dataToSave);
        
    }
    private async Task SaveToCloudTask(CloudSaveData data){
        try{
            var gameDataSave = new Dictionary<string,object>{{KeysHolder.CLOUD_SAVE_DATA_KEY,data}};
            await CloudSaveService.Instance.Data.Player.SaveAsync(gameDataSave);
            Debug.Log("Save Successfull");
        }catch(CloudSaveException ex){
            Debug.Log("Data Save UnSucceccFull");
            Debug.LogException(ex);
        }
    }
    public async void LoadCloudData(){
        Dictionary<string,Item> serverData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>{KeysHolder.CLOUD_SAVE_DATA_KEY});
        if(serverData.ContainsKey(KeysHolder.CLOUD_SAVE_DATA_KEY)){
            Debug.Log("Data Recived From Server " + serverData[KeysHolder.CLOUD_SAVE_DATA_KEY].Value.GetAsString());
            CloudSaveData RecivingData = new CloudSaveData();
            JsonUtility.FromJsonOverwrite(serverData[KeysHolder.CLOUD_SAVE_DATA_KEY].Value.GetAsString(),RecivingData);
            Debug.Log("Set Data " + RecivingData.ToString());
            gameData.saveData = RecivingData.cloudGameSaveData;
            gameSettings.gameProfileData = RecivingData.cloudGameProfileSaveData;
            levelData.levelSystemSaveData = RecivingData.levelSystemSaveData;
            if(playerCarBody.currentCarType != gameData.GetCarTypeShop()){
                playerCarBody.currentCarType = gameData.GetCarTypeShop();
            }
        }else{
            Debug.Log("Data Key Not Found");
            SaveCloudData();
        }
    }

    #endregion
    [System.Serializable]
    private class CloudSaveData{
        public GameDataSO.PlayerSaveData cloudGameSaveData;
        public LevelSystemSO.LevelSystemSaveData levelSystemSaveData;
        public GameProifileData cloudGameProfileSaveData;
    }
}