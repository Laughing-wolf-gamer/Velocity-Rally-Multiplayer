using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
[CreateAssetMenu(fileName = "Game Data",menuName = "Utils/Game Data")]
public class GameDataSO : ScriptableObject {
    public Texture socialMediaProfilePicture;
    public string playerNameFromSocialMedia;
    public event EventHandler OnMoneyAmountChanged;
    [SerializeField] private bool OnPc;
    [SerializeField] private LevelSystemSO levelSystem;
    public PlayerSaveData saveData;
    [SerializeField] private List<TrackSO> trackToSaveList;
    [SerializeField] private List<CarTypeSO> carsDataToSaveList;
    public bool GetOnPc(){
        return OnPc;
    }

#region  In Game Save Data..................
    public void ToggleLogedIn(bool logged){
        saveData.isLoggedIn = logged;
    }
    public void ToggleFirstRace(bool racedFirst){
        saveData.settingsSaveData.firstRace = racedFirst;
    }
    public bool GetIsFirstRace(){
        return saveData.settingsSaveData.firstRace;
    }
    public bool GetIsLoggedIn(){
        return saveData.isLoggedIn;
    }
    public LevelSystemSO GetLevelSystem{
        get{
            return levelSystem;
        }
    }
    public void IncreaseCash(int amount){
        saveData.totalCash += amount;
        OnMoneyAmountChanged?.Invoke(this,EventArgs.Empty);
    }
    public void IncreaseCoin(int amount){
        saveData.totalCoins += amount;
        OnMoneyAmountChanged?.Invoke(this,EventArgs.Empty);
    }
    public void SpendCash(int amount){
        saveData.totalCash -= amount;
        if(saveData.totalCash <= 0){
            saveData.totalCash = 0;
        }
        OnMoneyAmountChanged?.Invoke(this,EventArgs.Empty);
    }
    public void SpendCoins(int amount){
        saveData.totalCoins -= amount;
        if(saveData.totalCoins <= 0){
            saveData.totalCoins = 0;
        }
        OnMoneyAmountChanged?.Invoke(this,EventArgs.Empty);
    }
    public void SetReviewd(){
        saveData.settingsSaveData.isReviewd = true;
    }
    public int GetTotalCash(){
        return saveData.totalCash;
    }
    public int GetTotalCoins(){
        return saveData.totalCoins;
    }

#endregion



#region Settings Save Data.......................


    public void SetHasAdsInGame(bool value){
        saveData.settingsSaveData.hasAdsInGame = value;
    }
    public bool GetHasAds(){
        return saveData.settingsSaveData.hasAdsInGame;
    }
    public bool IsRevived(){
        return saveData.settingsSaveData.isReviewd;
    }
    public void SetQualityLevel(int index){
        saveData.settingsSaveData.quaityIndex = index;
    }
    public void SetFrameRate(int index){
        saveData.settingsSaveData.frameRate = index;
    }
    public void ToggleSpeedType(){
        saveData.settingsSaveData.speedIsKmph = !saveData.settingsSaveData.speedIsKmph;
    }
    public void ToggleShowNetworkPing(){
        saveData.settingsSaveData.showNetworkPing = !saveData.settingsSaveData.showNetworkPing;
    }
    public void ToggleVFX(){
        saveData.settingsSaveData.isVFXOn = !saveData.settingsSaveData.isVFXOn;
    }
    public void ToggleHaptics(){
        saveData.settingsSaveData.isHapticsOn = !saveData.settingsSaveData.isHapticsOn;
    }
    public void SetMusicVolumeAmount(float amount){
        saveData.settingsSaveData.musicVolumeAmount = amount;
    }
    public void SetSfxVolumeAmount(float amount){
        saveData.settingsSaveData.sfxVolumeAmount = amount;
    }
    public int GetFrameRate(){
        return saveData.settingsSaveData.frameRate;
    }
    public int GetQualityIndex(){
        return saveData.settingsSaveData.quaityIndex;
    }
    public bool GetSpeedType(){
        return saveData.settingsSaveData.speedIsKmph;
    }
    public bool GetShowPing(){
        return saveData.settingsSaveData.showNetworkPing;
    }
    public float GetMusicVolumeAmount(){
        return saveData.settingsSaveData.musicVolumeAmount;
    }
    public float GetSfxVolumeAmount(){
        return saveData.settingsSaveData.sfxVolumeAmount;
    }
    public bool GetVFXState(){
        return saveData.settingsSaveData.isVFXOn;
    }
    public bool GetHapticState(){
        return saveData.settingsSaveData.isHapticsOn;
    }
    public void SetRegion(string regionName){
        saveData.settingsSaveData.regionName = regionName;
    }
    public void SetControllsSensitivity(float value){
        saveData.settingsSaveData.controllsSensitivity = value; 
    }
    public float GetControllsSensitivity(){
        return saveData.settingsSaveData.controllsSensitivity;
    }
    public string GetRegion(){
        return saveData.settingsSaveData.regionName;
    }
    public void SetCurrentControllType(ControllsSelectionManager.ControllType controllType){
        saveData.settingsSaveData.controllType = (int)controllType;
    }
    public ControllsSelectionManager.ControllType GetControllType(){
        return (ControllsSelectionManager.ControllType) saveData.settingsSaveData.controllType;
    }

#endregion
    

#region Profile Data...........
    public bool IsFirstRun(){
        return saveData.settingsSaveData.firstRun;
    }

    public void SetUntqueIdNumberRandom(char[] playerNameSufix){
        if(saveData.settingsSaveData.firstRun){
            string nameSufix = new string(playerNameSufix);
            char[] numarray = new char[9];
            for (int i = 0; i < numarray.Length; i++){
                int incrementAmount = Random.Range(1,10);
                numarray[i] = char.Parse(incrementAmount.ToString());
            }
            string numSuffix =  new string(numarray);
            saveData.profileSaveData.uniqueIdNumber = string.Concat("#",nameSufix,numSuffix);
            saveData.settingsSaveData.firstRun = false;
        }
    }
#region DailyRewrdsData...................
    public void SetClamedBonus(bool _value){
        saveData.dailyRewards.isClamedDailyBonus = _value;
    }
    public bool GetIsClamedBonus(){
        return saveData.dailyRewards.isClamedDailyBonus;
    }
    public void IncreaseCurrentDayNumber(){
        saveData.currentDay = (saveData.currentDay + 1) % 7;
        /* if(saveData.currentDay >= 7){
            saveData.currentDay = 0;
        } */
    }
    public void SetDailyBonusAlreadyShown(bool isShown) {
        saveData.dailyRewards.DailyBonusAlreadyShown = isShown;
    }
    public int GetcurrentDay(){
        return saveData.currentDay;
    }

#endregion
    public string GetUniqueIdNumber(){
        return saveData.profileSaveData.uniqueIdNumber;
    }
    public void DecreaseExperacne(int amount){
        levelSystem.DecreaseExp(amount);
    }

    public void IncreaseMatchWon(int amount){
        saveData.profileSaveData.matchWonNumber += amount;
    }
    public void DecreaseMatchWon(int amount){
        saveData.profileSaveData.matchWonNumber -= amount;
    }
    public void IncreaseMedalsCount(int amount){
        saveData.profileSaveData.medalsTotalRecived += amount;
    }
    public void DecreaseMedals(int amount){
        saveData.profileSaveData.medalsTotalRecived -= amount;
    }
    public void IncreaseGoldTropyCount(int amount){
        saveData.profileSaveData.totalGoldTropy += amount;
    }
    public void IncreaseTotalRacesJoinedCount(int amount){
        saveData.profileSaveData.totalRacesJoined += amount;
    }
    public void IncreaseGamePlayed(float time){
        saveData.profileSaveData.totalTimePlayed = time;
    }

    public int GetTotalMatchWon(){
        return saveData.profileSaveData.matchWonNumber;
    }
    public int GetTotalMedalsWon(){
        return saveData.profileSaveData.medalsTotalRecived;
    }
    public int GetTotalGoldTropyWon(){
        return saveData.profileSaveData.totalGoldTropy;
    }
    public int GetTotalRacesJoined(){
        return saveData.profileSaveData.totalRacesJoined;
    }
    public float GetGamePlayedTime(){
        return saveData.profileSaveData.totalTimePlayed;
    }
    public void SetCurrentCar(CarTypeShop carTypeShop){
        saveData.carType = (int) carTypeShop;
    }
    public CarTypeShop GetCarTypeShop(){
        return (CarTypeShop) saveData.carType;
    }
    


#endregion

#region Saving and Loading................

    public void Save() {
        for(int i = 0; i < saveData.trackSaveDatasList.Length; i++){
            saveData.trackSaveDatasList[i].trackIndex = (int) trackToSaveList[i].trackSaveData.tracks;
            saveData.trackSaveDatasList[i].locked = trackToSaveList[i].trackSaveData.isLocked;
        }
        for(int i = 0; i < saveData.carSaveDatas.Length; i++){
            saveData.carSaveDatas[i].carTypeShopIndex = (int) carsDataToSaveList[i].carSaveData.carTypeShop;
            saveData.carSaveDatas[i].isLocked = carsDataToSaveList[i].carSaveData.isLocked;
            saveData.carSaveDatas[i].cashUpgradeAmount = carsDataToSaveList[i].carSaveData.cashUpgradeAmount;
            saveData.carSaveDatas[i].coinUpgradeAmount = carsDataToSaveList[i].carSaveData.coinUpgradeAmount;
            saveData.carSaveDatas[i].carData = carsDataToSaveList[i].carSaveData.carData;

        }
        SaveSystemManager.Save<PlayerSaveData>(saveData,KeysHolder.GAMEDATA_SAVE_KEY);
        levelSystem.SetLevelSaveData();
    }

    public void Load(){
        PlayerSaveData loadedData = SaveSystemManager.Load<PlayerSaveData>(KeysHolder.GAMEDATA_SAVE_KEY);
        saveData = loadedData;
        levelSystem.Load();
        for(int i = 0; i < trackToSaveList.Count; i++){
            TrackTypes currentTrackType = (TrackTypes)saveData.trackSaveDatasList[i].trackIndex;
            if(trackToSaveList[i].trackSaveData.tracks == currentTrackType){
                trackToSaveList[i].trackSaveData.isLocked = saveData.trackSaveDatasList[i].locked;
            }
        }
        for(int i = 0; i < carsDataToSaveList.Count; i++){
            CarTypeShop currentType = (CarTypeShop) saveData.carSaveDatas[i].carTypeShopIndex;
            if(carsDataToSaveList[i].carSaveData.carTypeShop == currentType){
                carsDataToSaveList[i].carSaveData.isLocked = saveData.carSaveDatas[i].isLocked;
                carsDataToSaveList[i].carSaveData.cashUpgradeAmount = saveData.carSaveDatas[i].cashUpgradeAmount;
                carsDataToSaveList[i].carSaveData.coinUpgradeAmount = saveData.carSaveDatas[i].coinUpgradeAmount;
                carsDataToSaveList[i].carSaveData.carData = saveData.carSaveDatas[i].carData;
            }
        }
    }

   /*  public enum LogingType{
        FBLogin,GpGsLogin,guestLogin
    } */
    [Serializable]
    public class PlayerSaveData{
        public bool isLoggedIn;
        public int currentDay;
        public int totalCash;
        public int totalCoins;
        public int carType;// Car type Shop Enum...
        public ProifleSaveData profileSaveData;
        public SettingsSaveData settingsSaveData;
        public DailyRewards dailyRewards;
        public TrackData[] trackSaveDatasList;
        public CarCloudData[] carSaveDatas;
    }
    [Serializable]
    public class TrackData{
        public bool locked;
        public int trackIndex;// Track type Shop Enum...
    }
    [Serializable]
    public class CarCloudData {
        public bool isLocked = true;
        public int carTypeShopIndex;// Car type Shop Enum...
        public int cashUpgradeAmount;
        public int coinUpgradeAmount;
        public CarTypeSO.CarData carData;
    } 
    [Serializable]
    public class DailyRewards {
        public bool isClamedDailyBonus;
        public bool DailyBonusAlreadyShown;
    }
    [Serializable]
    public class SettingsSaveData{
        public bool firstRun = true,firstRace;
        public bool hasAdsInGame;
        public bool isReviewd;
        public bool isVFXOn;
        public bool isHapticsOn;
        public float musicVolumeAmount;
        public float sfxVolumeAmount;
        public int quaityIndex;
        public int frameRate;
        public bool speedIsKmph;
        public bool showNetworkPing;
        public string regionName = "in";
        public int controllType;// Controll type Shop Enum...
        public float controllsSensitivity = 10f;
    }
    [Serializable]
    public class ProifleSaveData {
        public string uniqueIdNumber = string.Empty;
        public int matchWonNumber = 10;
        public int totalRacesJoined = 45;
        public int medalsTotalRecived = 20,totalGoldTropy = 1;
        public float totalTimePlayed = 0;
    }


#endregion


}
