using TMPro;
using DG.Tweening;
using UnityEngine;

public class GameEventManager : MonoBehaviour {
    [SerializeField] private MenuSystem menuSystem;
    [SerializeField] private TextMeshProUGUI dayNumberText;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private DailyBonuUIButton[] rewardClameButtonArray;
    [SerializeField] private TimeManager timeManager;
    private DailyBonuUIButton CurrentBonusButton;
    private int currentDay;
    private bool isRewarded;
    private void Start() {

        currentDay = gameData.GetcurrentDay();
        dayNumberText.SetText(string.Concat("Day ",(currentDay + 1)));
        CheckRewardButton();
        CheckForDailyReward();
        menuSystem.OnDailyRewardWindowOpen += MenuSystem_CheckRewardButton;
    }

    private void MenuSystem_CheckRewardButton(object sender, System.EventArgs e) {
        CheckRewardButton();
        CheckForDailyReward();
    }

    private void CheckRewardButton(){
        currentDay = gameData.GetcurrentDay();

        if(timeManager.Ready()){
            gameData.SetClamedBonus(false);
        }else{
            gameData.SetClamedBonus(true);
        }
        for (int i = 0; i < rewardClameButtonArray.Length; i++){
            if(timeManager.Ready()) {

                if(currentDay == rewardClameButtonArray[i].GetCurrentRewardNumber()){
                    rewardClameButtonArray[i].SetIsActive(true);
                    CurrentBonusButton = rewardClameButtonArray[i];
                    CurrentBonusButton.SetClamedText("CLAIM");
                }else{
                    if(i < currentDay){
                        rewardClameButtonArray[i].SetClamedText("CLAIMED");
                    }else {
                        rewardClameButtonArray[i].SetClamedText("NOT AVILABLE YET");
                    }
                    rewardClameButtonArray[i].SetIsActive(false);
                }
            }else{
                if(i < currentDay){
                    rewardClameButtonArray[i].SetClamedText("CLAIMED");
                }else {
                    rewardClameButtonArray[i].SetClamedText("NOT AVILABLE YET");
                }
                rewardClameButtonArray[i].SetIsActive(false);
            }
        }
        CheckForDailyReward();
    }
    private void CheckForDailyReward(){
        if(gameData.GetIsClamedBonus()){
            // noitificationIcon.SetActive(false);
        }else{
            // noitificationIcon.SetActive(true);
        }
    }
    
    public void Claime(){
        CurrentBonusButton.ClamReward();
        timeManager.Click();
        CheckRewardButton();
        CheckForDailyReward();
        gameData.SetClamedBonus(true);
        gameData.IncreaseCurrentDayNumber();
        CurrentBonusButton.SetIsActive(false);
        int randomSeed = Random.Range(6,10);
        transform.DOShakeScale(0.3f,2f,randomSeed,78,false);
    }

    /* //Change this According to 5X button & call it in AdManager Script.
    public void SetRewarded(bool value){
        isRewarded = value;
        if(!isRewarded){
            //Change Below function
            gameData.SetClamedBonus(true);
            gameData.IncreaseCurrentDayNumber();
            CurrentBonusButton.SetIsActive(false);
            // claimDoubleButton.interactable = false;
            int randomSeed = Random.Range(6,10);
            transform.DOShakeScale(0.3f,2f,randomSeed,78,true);
        }
    } */
    
}