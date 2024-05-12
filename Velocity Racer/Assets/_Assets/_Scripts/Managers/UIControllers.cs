using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine.Rendering;
using DG.Tweening;
using GamerWolf.Utils;
public class UIControllers : MonoBehaviour {
    [SerializeField] private Image lowPingWindow;
    [SerializeField] private TextMeshProUGUI pingValueCurrent;
    [SerializeField] private Volume blurVolume;
    [SerializeField] private GameObject endGameWindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject startingTimeWindow,playerHud,waitingForPlayerToJoinTimeWindow,waitingForPlayersToFinishTheRace;
    [SerializeField] private TextMeshProUGUI endScreenWaitingTimeRankTxt,startingTimeText,waitingTimeText,pingTxt;
    [SerializeField] private TextMeshProUGUI finalRaceEndTimeWinnerPlayerHud,finalRaceEndTimePlayeOtherPlayerHud;
    [SerializeField] private GameObject rankWindow,finalDataWindow,finalLeaderBoardWindow;
    [SerializeField] private UIButtonCustom homeButton;
    [SerializeField] private TextMeshProUGUI showHomeBtnTimer;
    [SerializeField] private RankPreviewSlide firstRankViewTween,secondRankViewTween,thirdRankViewTween;
    [SerializeField] private List<CarTypeSO> allCarsList;
    [SerializeField] private FinalGainData finalGainData;

    [SerializeField] private LevelSystemSO levelSystem;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField,Range(0f,0.1f)] private float updateTimerMax = .16f;
    private float updateTimerLevel,updateTimerCoin,updateTimerCash;
    private float homeBtnShowTime;
    private int currentLevel,currentCashAmount,currentCoinAmount;
    private int currentExperiance;
    private int currentExperianceToNextLevel;
    private bool isAnimatingLevel,isAnimatingCash,isAnimatingCoin,showHomeBtn;//Amsp~#99

    private FunctionUpdater functionUpdaterLevelSystem,functionUpdaterCash,functionUpdaterCoin,functionUpdaterHomeBtn;
    private bool isSettingsOpen;
    private Vector3 startPos;

    [Serializable]
    public class FinalGainData{
        public TextMeshProUGUI levelIncrmentedAmountTxt;
        public TextMeshProUGUI cashIncrmentedAmountTxt;
        public TextMeshProUGUI coinIncrmentedAmountTxt;
        public TextMeshProUGUI finalRankText,levelAmount,cashAmount,coinAmount;
        public Image exPBar,cashGainBar,coinGainBar;
    }
    private void Awake(){
        startPos = lowPingWindow.rectTransform.anchoredPosition;
        homeBtnShowTime = 5f;
        showHomeBtnTimer.SetText(Mathf.CeilToInt(homeBtnShowTime).ToString());
        ShowLeaderBoard();
        blurVolume.weight = 1f;
        isAnimatingLevel = isAnimatingCash = isAnimatingCoin = false;
        settingsWindow.SetActive(false);
    }
    public void ShowLeaderBoard(){
        finalLeaderBoardWindow.SetActive(true);
        finalDataWindow.SetActive(false);
    }
    public void ShowDataWindow(){
        // call from LeaderBoard Window
        finalDataWindow.SetActive(true);
        finalLeaderBoardWindow.SetActive(false);
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            isSettingsOpen = !isSettingsOpen;
            settingsWindow.SetActive(isSettingsOpen);
        }
    }
    public void OpenSettingWindow(){
        isSettingsOpen = true;
        settingsWindow.SetActive(isSettingsOpen);
    }
    public void CloseSettingWindow(){
        isSettingsOpen = false;
        settingsWindow.SetActive(isSettingsOpen);
    }
    public void SetPing(float ping){
        if(gameData.GetShowPing()){
            if(ping > 150){
                pingTxt.SetText(string.Concat("Ping : ","<color=red>",ping,"</color>"));
            }else{
                pingTxt.SetText(string.Concat("Ping : ","<color=green>",ping,"</color>"));
            }
        }else{
            pingTxt.gameObject.SetActive(false);
        }
    }

    public void ShowHideStartingTimeWindow(bool waiting){
        blurVolume.weight = waiting ? 1f : 0f;
        startingTimeWindow.SetActive(waiting);
    }
    public void ShowHideWaitForOthersToFinishWindow(bool waiting,int rank = -1){
        if(rank > 0){
            endScreenWaitingTimeRankTxt.SetText(string.Concat(rank,KeysHolder.RankingSuffix(rank)));
        }
        waitingForPlayersToFinishTheRace.SetActive(waiting);
    }
    public void ShowStartTime(float time){
        startingTimeText.SetText(time.ToString());
    }
    public void SetWinnerGameEndTime(float time){
        finalRaceEndTimeWinnerPlayerHud.SetText(string.Concat("Wait for the Other Racers to Finishe.. \n Race Ending in ",time,"s"));
    }
    public void SetOtherGameEndTime(float time){
        finalRaceEndTimePlayeOtherPlayerHud.SetText(string.Concat("Race Ending in ",time,"s"));
    }
    public void ShowHideFinalRaceEndTimeWinnerPlayerHud(bool show){
        finalRaceEndTimeWinnerPlayerHud.gameObject.SetActive(show);
    }
    public void ShowHideFinalRaceEndTimeOtherPlayerHud(bool show){
        finalRaceEndTimePlayeOtherPlayerHud.gameObject.SetActive(show);
    }
    public void WaitforPlayerToJoinWindow(float timer,bool showWindow,bool isMasterPlayer = false){
        waitingForPlayerToJoinTimeWindow.SetActive(showWindow);
        if(isMasterPlayer){
            waitingTimeText.gameObject.SetActive(true);
            waitingTimeText.SetText(string.Concat("WAITING TIME : ",Mathf.CeilToInt(timer)));
        }else{
            waitingTimeText.gameObject.SetActive(false);
        }
    }
    public void ShowHideEndGameWindow(bool show){
        endGameWindow.SetActive(show);
    }
    public void ShowHidePlayHud(bool show){
        playerHud.SetActive(show);
    }
    public void ShowHideRankWindow(bool show){
        rankWindow.SetActive(show);
    }
    public void SetPositions(GamePlayerInfo first,GamePlayerInfo second,GamePlayerInfo third){
        firstRankViewTween.SetRankNames(KeysHolder.StringShorting(first.gameProifile.username,4).ToUpper(),IsLocalPlayer(first.gameProifile.username),GetCarIconSprite(first.gameProifile.carTypeShop));
        secondRankViewTween.SetRankNames(KeysHolder.StringShorting(second.gameProifile.username,4).ToUpper(),IsLocalPlayer(second.gameProifile.username),GetCarIconSprite(second.gameProifile.carTypeShop));
        if(third != null){
            thirdRankViewTween.SetRankNames(KeysHolder.StringShorting(third.gameProifile.username,4).ToUpper(),IsLocalPlayer(third.gameProifile.username),GetCarIconSprite(third.gameProifile.carTypeShop));
        }else{
            thirdRankViewTween.Hide();
        }
    }
    public bool IsLocalPlayer(string userName){
        return gameSettings.gameProfileData.username == userName; 
    }
    private Sprite GetCarIconSprite(int cartypeIndex){
        Array values = Enum.GetValues(typeof(CarTypeShop));
        for (int i = 0; i < allCarsList.Count; i++) {
            CarTypeShop carType = (CarTypeShop)values.GetValue(cartypeIndex);
            if(allCarsList[i].carSaveData.carTypeShop == carType){
                return allCarsList[i].carIcon;
            }
        }
        return null;
    }
    
    public void SetPlayerDetailsAfterGameEnd(int r) {
        blurVolume.weight = 1f;
        finalGainData.finalRankText.SetText(string.Concat(r,KeysHolder.RankingSuffix(r)));

    }
    public void ChangeLevelSystem(int newLevel,int addedCash,int addedCoin){
        currentCashAmount = gameData.GetTotalCash();
        currentCoinAmount = gameData.GetTotalCoins();

        finalGainData.coinGainBar.fillAmount = GetCoinAmountNormalized();
        finalGainData.cashGainBar.fillAmount = GetCashAmountNormalized();


        finalGainData.coinAmount.SetText(string.Concat("$",GetAmountNormalized(addedCoin)));
        finalGainData.cashAmount.SetText(string.Concat("$",GetAmountNormalized(addedCash)));


        currentLevel = levelSystem.GetLevelNumber();
        currentExperiance = levelSystem.experiance;
        currentExperianceToNextLevel = levelSystem.experianceToNextLevel;
        finalGainData.exPBar.fillAmount = GetExperianceNormalized();
        finalGainData.levelAmount.SetText(string.Concat("LEVEL ",GetLevelNumber()));
        finalGainData.levelIncrmentedAmountTxt.SetText(string.Concat("+",newLevel));
        levelSystem.AddExperiance(newLevel);
        gameData.IncreaseCash(addedCash);
        gameData.IncreaseCoin(addedCoin);
        
    }
    public void OnLeaveRoomBtnClick(){
        FunctionUpdater.DestroyUpdater(functionUpdaterCash);
        FunctionUpdater.DestroyUpdater(functionUpdaterCoin);
        FunctionUpdater.DestroyUpdater(functionUpdaterLevelSystem);
        FunctionUpdater.DestroyUpdater(functionUpdaterHomeBtn);
        SavingAndLoadingManager.Current?.SaveGame();
    }
    public void StartTheFinalRewards(){
        if(!isAnimatingLevel){
            isAnimatingLevel = true;
            finalGainData.levelIncrmentedAmountTxt.rectTransform.DOShakeScale(1,1,10,90,false).SetDelay(.7f).SetEase(Ease.OutQuad).SetLoops(5,LoopType.Yoyo);
            functionUpdaterLevelSystem = FunctionUpdater.Create(AnimatedLevelSystemUI);
        }
        if(!isAnimatingCoin){
            isAnimatingCoin = true;
            finalGainData.coinIncrmentedAmountTxt.SetText(string.Concat(gameData.GetTotalCoins() - currentCoinAmount,"+"));
            finalGainData.coinIncrmentedAmountTxt.rectTransform.DOShakeScale(1,1,10,90,false).SetDelay(.7f).SetEase(Ease.OutQuad).SetLoops(5,LoopType.Yoyo);
            functionUpdaterCoin = FunctionUpdater.Create(AnimateCoinAmountUI);
        }
        if(!isAnimatingCash){
            isAnimatingCash = true;
            finalGainData.cashIncrmentedAmountTxt.SetText(string.Concat("$",gameData.GetTotalCash() - currentCashAmount,"+"));
            finalGainData.cashIncrmentedAmountTxt.rectTransform.DOShakeScale(1,1,10,90,false).SetDelay(.7f).SetEase(Ease.OutQuad).SetLoops(5,LoopType.Yoyo);
            functionUpdaterCash = FunctionUpdater.Create(AnimateCashAmountUI);
        }
        if(!showHomeBtn){
            showHomeBtn = true;
            functionUpdaterHomeBtn = FunctionUpdater.Create(ShowHomeBtn_Updater);
        }
        AudioManager.Current?.PlayOneShotMusic(Sounds.SoundType.TrackDay_End_2);
    }

    private void ShowHomeBtn_Updater() {
        if(showHomeBtn){
            UpdateShowHomeBtn_Updater();
        }
    }
    private void UpdateShowHomeBtn_Updater(){
        if(homeBtnShowTime > 0f){
            homeBtnShowTime -= Time.deltaTime;
            showHomeBtnTimer.SetText(Mathf.CeilToInt(homeBtnShowTime).ToString() + "s");
            homeButton.interactable = false;
        }else{
            homeButton.interactable = true;
            showHomeBtnTimer.gameObject.SetActive(false);
            FunctionUpdater.DestroyUpdater(functionUpdaterHomeBtn);
        }
    }
    private void AnimatedLevelSystemUI(){
        if(isAnimatingLevel){
            updateTimerLevel += Time.deltaTime;
            while(updateTimerLevel > updateTimerMax){
                updateTimerLevel -= updateTimerMax;
                UpdateAddExperaince();
            }
        }
    }
    private void AnimateCoinAmountUI(){
        if(isAnimatingCoin){
            updateTimerCoin += Time.deltaTime;
            while(updateTimerCoin > updateTimerMax){
                updateTimerCoin -= updateTimerMax;
                UpdateCoinAmount();
            }
        }
    }
    private void AnimateCashAmountUI(){
        if(isAnimatingCash){
            updateTimerCash += Time.deltaTime;
            while(updateTimerCash > updateTimerMax){
                updateTimerCash -= updateTimerMax;
                UpdateCashAmount();
            }
        }
    }
    private void UpdateCashAmount(){
        if(currentCashAmount < gameData.GetTotalCash()){
            AddCash();
        }else{
            isAnimatingCash = false;
            FunctionUpdater.DestroyUpdater(functionUpdaterCash);
        }
    }
    private void UpdateCoinAmount(){
        if(currentCoinAmount < gameData.GetTotalCoins()){
            AddCoin();
        }else{
            isAnimatingCoin = false;
            FunctionUpdater.DestroyUpdater(functionUpdaterCoin);
        }
    }
    private void AddCoin(){
        currentCoinAmount ++;
        finalGainData.coinGainBar.fillAmount = GetCoinAmountNormalized();
        finalGainData.coinAmount.SetText(string.Concat("$",GetAmountNormalized(currentCoinAmount)));
    }
    private void AddCash(){
        currentCashAmount ++;
        finalGainData.cashGainBar.fillAmount = GetCashAmountNormalized();
        finalGainData.cashAmount.SetText(string.Concat("$",GetAmountNormalized(currentCashAmount)));
    }
    private void UpdateAddExperaince(){
        if(currentLevel < levelSystem.GetLevelNumber()){
            AddExperiance();
        }else{
            if(currentExperiance < levelSystem.experiance){
                AddExperiance();
            }else{
                isAnimatingLevel = false;
                FunctionUpdater.DestroyUpdater(functionUpdaterLevelSystem);
            }
        }
    }
    public void ShowHighPingWarningWindow(float pingValue){
        if(!lowPingWindow.gameObject.activeInHierarchy){
            pingValueCurrent.SetText(pingValue.ToString());
            lowPingWindow.gameObject.SetActive(true);
            lowPingWindow.color = Color.yellow;
            lowPingWindow.rectTransform.DOAnchorPosY(0f,1f,false).SetEase(Ease.OutBack).SetDelay(.4f).onComplete += () =>{
                CancelInvoke(nameof(HideHighPingWarningWindow));
                Invoke(nameof(HideHighPingWarningWindow),2f);
            };
        }
    }
    public void ShowHighPingWarningWindow(float pingValue,Color warningColor){
        if(!lowPingWindow.gameObject.activeInHierarchy){
            pingValueCurrent.SetText(pingValue.ToString());
            lowPingWindow.gameObject.SetActive(true);
            lowPingWindow.color = warningColor;
            lowPingWindow.rectTransform.DOAnchorPosY(0f,1f,false).SetEase(Ease.OutBack).SetDelay(.4f).onComplete += () =>{
                CancelInvoke(nameof(HideHighPingWarningWindow));
                Invoke(nameof(HideHighPingWarningWindow),2f);
            };
        }
    }
    public void HideHighPingWarningWindow(){
        CancelInvoke(nameof(HideHighPingWarningWindow));
        lowPingWindow.color = Color.black;
        lowPingWindow.rectTransform.DOAnchorPosY(startPos.y,1f,false).SetEase(Ease.InBack).SetDelay(.4f).onComplete += () =>{
            lowPingWindow.rectTransform.anchoredPosition = startPos;
            lowPingWindow.gameObject.SetActive(false);
        };
    }
    private void AddExperiance(){
        currentExperiance ++;
        if(currentExperiance >= currentExperianceToNextLevel){
            currentLevel++;
            currentExperiance = 0;
        }
        finalGainData.exPBar.fillAmount = GetExperianceNormalized();
        finalGainData.levelAmount.SetText(string.Concat("LEVEL ",GetLevelNumber()));
    }
    public int GetLevelNumber(){
        return currentLevel + 1;
    }
    public float GetExperianceNormalized(){
        return (float)currentExperiance / currentExperianceToNextLevel;
    }
    public float GetCashAmountNormalized(){
        return (float)currentCashAmount / gameData.GetTotalCash();
    }
    public float GetCoinAmountNormalized(){
        return (float)currentCoinAmount / gameData.GetTotalCoins();
    }
    private string GetAmountNormalized(int amount){
        if(amount >= 100000){
            int cashAmount = Mathf.RoundToInt((float)amount / 100000);
            return string.Concat(cashAmount.ToString("f1"),"M");
        }
        if(amount >= 1000){
            int cashAmount = Mathf.RoundToInt((float)amount / 1000);
            return string.Concat(cashAmount.ToString("f1"),"K");
        }
        return amount.ToString();
    }
}