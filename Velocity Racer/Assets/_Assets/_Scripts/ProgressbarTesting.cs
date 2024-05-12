using CodeMonkey.Utils;
using UnityEngine;

public class ProgressbarTesting : MonoBehaviour {
    [SerializeField] private LevelSystemSO levelSystem;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private float updateTimerMax = .16f;
    [SerializeField] private float updateTimerLevel,updateTimerCoin,updateTimerCash;

    [SerializeField] private int currentLevel,currentCashAmount,currentCoinAmount;
    [SerializeField] private int currentExperiance;
    [SerializeField] private int currentExperianceToNextLevel;
    [SerializeField] private bool isAnimatingLevel,isAnimatingCash,isAnimatingCoin;
    [SerializeField] private UIControllers.FinalGainData finalGainData;

    private FunctionUpdater functionUpdaterLevelSystem,functionUpdaterCash,functionUpdaterCoin;
    private void Awake() {
        currentCashAmount = gameData.GetTotalCash();
        currentCoinAmount = gameData.GetTotalCoins();
        currentLevel = levelSystem.GetLevelNumber();
        currentExperiance = levelSystem.experiance;
        currentExperianceToNextLevel = levelSystem.experianceToNextLevel;

        finalGainData.coinGainBar.fillAmount = GetCoinAmountNormalized();
        finalGainData.cashGainBar.fillAmount = GetCashAmountNormalized();
    }
    [ContextMenu("Start Progress")]
    public void StartProgress(){
        finalGainData.coinAmount.SetText(string.Concat("$",GetAmountNormalized(currentCoinAmount)));
        finalGainData.cashAmount.SetText(string.Concat("$",GetAmountNormalized(currentCashAmount)));
        finalGainData.exPBar.fillAmount = GetExperianceNormalized();
        finalGainData.levelAmount.SetText(string.Concat("LEVEL ",GetLevelNumber()));
        finalGainData.levelIncrmentedAmountTxt.SetText(string.Concat("+",200));
        finalGainData.coinIncrmentedAmountTxt.SetText(string.Concat("$",200,"+"));
        finalGainData.cashIncrmentedAmountTxt.SetText(string.Concat("$",200,"+"));
        
        levelSystem.AddExperiance(200);
        gameData.IncreaseCash(200);
        gameData.IncreaseCoin(200);
        StartTheFinalRewards();
    }
    public void StartTheFinalRewards(){
        isAnimatingLevel = isAnimatingCoin = isAnimatingCash = true;
        functionUpdaterLevelSystem = FunctionUpdater.Create(AnimatedLevelSystemUI);
        functionUpdaterCoin = FunctionUpdater.Create(AnimateCoinAmount);
        functionUpdaterCash = FunctionUpdater.Create(AnimateCashAmount);
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
    private void AnimateCoinAmount(){
        if(isAnimatingCoin){
            updateTimerCoin += Time.deltaTime;
            while(updateTimerCoin > updateTimerMax){
                updateTimerCoin -= updateTimerMax;
                UpdateCoinAmount();
            }
        }
    }
    private void AnimateCashAmount(){
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