using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
public class DailyBonuUIButton : MonoBehaviour {
    [SerializeField] private GameObject coinRewardImage,cashRewardImage;
    [SerializeField] private UnityEvent OnRewardCollected;
    [SerializeField] private GameObject lockedView,UnlockedView;
    [SerializeField] private TextMeshProUGUI cashRewardAmountTxt,coinRewardAmountTxt;
    [SerializeField] private TextMeshProUGUI unloackTimeLeft;
    [SerializeField] private DailyRewardSO todayReward;
    [SerializeField] private GameDataSO playerDataSO;
    [SerializeField] private TextMeshProUGUI dayNumber,rewardDayText;
    [SerializeField] private TextMeshProUGUI claimText;
    [SerializeField] private TimeManager timeManager;
    private int coinRewardAmount,cashRewardAmount;

    private void Start(){
        unloackTimeLeft.SetText(string.Concat("Unlocking in ",timeManager.GetTimeLeftToUnlock(todayReward.dayNumber)," hours"));
        timeManager.OnClick += TimeManager_OnClick;
        timeManager.OnTimeOver += TimeManager_OnTimeOver;
        timeManager.OnTimeTicking += TimeManager_OnTimeTicking;
        coinRewardImage.SetActive(false);
        cashRewardImage.SetActive(false);
        for (int i = 0; i < todayReward.rewardArrays.Length; i++) {
            switch(todayReward.rewardArrays[i].rewardType){
                case DailyRewardSO.RewardType.Cash:
                    cashRewardImage.SetActive(true);
                    cashRewardAmountTxt.SetText(string.Concat("$",todayReward.rewardArrays[i].rewardAmount));
                break;
                case DailyRewardSO.RewardType.Coins:
                    coinRewardImage.SetActive(true);
                    coinRewardAmountTxt.SetText(string.Concat("$",todayReward.rewardArrays[i].rewardAmount));
                break;
            }
            
        }
        SetClamedText("Available");
        rewardDayText.SetText(string.Concat("Day " ,(todayReward.dayNumber + 1),"Reward"));
        dayNumber.SetText(string.Concat("Day " ,(todayReward.dayNumber + 1)));
    }

    private void TimeManager_OnTimeTicking(object sender, TimeManager.OnTimeTickingArgs e) {
        unloackTimeLeft.SetText(string.Concat("Unlocking in : ",e.hours * (todayReward.dayNumber + 1)," hours"));
    }


    private void TimeManager_OnTimeOver(object sender, EventArgs e) {
        unloackTimeLeft.SetText(string.Concat("Unlocking in ",todayReward.dayNumber + 1," days"));
    }


    private void TimeManager_OnClick(object sender, EventArgs e) {
        // unloackTimeLeft.SetText(string.Concat("Unlocking in : ",timeManager.GetTimeLeftToUnlock(todayReward.dayNumber)," hours"));
    }


    public void SetIsActive(bool value){
        lockedView.gameObject.SetActive(false);
        UnlockedView.gameObject.SetActive(false);
        if(value){
            UnlockedView.SetActive(true);
        }else{
            lockedView.SetActive(true);
        }
    }
    
    public void SetClamedText(string text){
        claimText.SetText(text);
    }
    // This function you have to change accordingly for double.
    // It's calling in GameEventManager.
    public void ClamReward(){
        GetRewards();
        playerDataSO.IncreaseCoin(coinRewardAmount);
        playerDataSO.IncreaseCash(cashRewardAmount);
        SetClamedText("Claimed");
        SetIsActive(false);
        // rewardNameText.SetText("REWARD CLAMED... \n No Reward For Today.. \n Come Back Tommorow");
        // todayNameText.SetText(" ");
        playerDataSO.SetDailyBonusAlreadyShown(true);
        playerDataSO.SetClamedBonus(true);
        // AudioManager.Current.PlayOneShotMusic(Sounds.SoundType.ItemOpen);
        OnRewardCollected?.Invoke();
    }
    public void GetRewards(){
        cashRewardAmount = 0;
        coinRewardAmount = 0;
        for (int i = 0; i < todayReward.rewardArrays.Length; i++) {
            switch(todayReward.rewardArrays[i].rewardType){
                case DailyRewardSO.RewardType.Cash:
                    cashRewardAmount = todayReward.rewardArrays[i].rewardAmount;
                break;
                case DailyRewardSO.RewardType.Coins:
                    coinRewardAmount = todayReward.rewardArrays[i].rewardAmount;
                break;
            }
            
        }
    }

    public int GetCurrentRewardNumber(){
        return todayReward.dayNumber;
    }
    
    
    
    
}