using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerUiManager : MonoBehaviour {
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private CarDriver driver;
    [SerializeField] private Image nosAmount;
    [SerializeField] private TextMeshProUGUI currentLapsText,timeText,speedText,speedUnitTxt,currentPositions;
    [SerializeField] private GameObject wrongCheckPointIndicator,finalLapCompleteIndicator,lapCompletedIndicator,miniMapTrack;
    [SerializeField] private SteeringWheelControllUI steeringWheelControllUi;
    [SerializeField] private AccelerometerInputs accelerometerInputs;
    [SerializeField] private GameObject tutorialUI;

    private float sideWays,forward;
    private bool nosPressed,reverse;
    private void Start(){
        sideWays = 0f;
        nosPressed = reverse = false;
        OnForwardRelease();
    }
    private void Update(){
        if(gameData.GetOnPc()) return;
        if(gameData.GetControllType() == ControllsSelectionManager.ControllType.SteeringWheel_Controlls){
            driver.SetSteeringInput(steeringWheelControllUi.GetSteeringValue());
        }
    }
    private void FixedUpdate(){
        if(gameData.GetOnPc()) return;
        if(gameData.GetControllType() == ControllsSelectionManager.ControllType.Accelrometer_Controlls){
            driver.AccelerometerInputs(accelerometerInputs.GetSteeringValue());
        }
    }
    public void OnForwardPress (){
        forward = 1f;
        reverse = false;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void OnForwardRelease(){
        forward = 0f;
        reverse = false;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void Reverse(){
        forward = -1f;
        reverse = true;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    
    public void LeftDrive(){
        sideWays = -1f;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void OnLeftDriveRelease(){
        sideWays = 0f;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void RightDrive(){
        sideWays = 1f;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    
    public void HandBrake(){
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void OnHandBrakeRelease(){
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void SetNos(float normalizedAmount){
        nosAmount.fillAmount = normalizedAmount;
    }
    public void OnNosPress(){
        
        nosPressed = true;
        driver.SetUIInput(reverse,sideWays,nosPressed);
        driver.PlayNosPressSound();
    }
    public void OnNosRelease(){
        nosPressed = false;
        driver.SetUIInput(reverse,sideWays,nosPressed);
    }
    public void ShowWrongCheckPointIndicator(){
        HideLapCompletedIndicator();
        HideWrongCheckPointIndicator();
        HideFinalLapIndicator();
        wrongCheckPointIndicator.SetActive(true);
        CancelInvoke(nameof(HideWrongCheckPointIndicator));
        Invoke(nameof(HideWrongCheckPointIndicator),1f);
    }
    public void HideWrongCheckPointIndicator(){
        CancelInvoke(nameof(HideWrongCheckPointIndicator));
        wrongCheckPointIndicator.SetActive(false);
    }
    public void ShowLapCompletedIndicator(){
        HideLapCompletedIndicator();
        HideWrongCheckPointIndicator();
        HideFinalLapIndicator();
        lapCompletedIndicator.SetActive(true);
        CancelInvoke(nameof(HideLapCompletedIndicator));
        Invoke(nameof(HideLapCompletedIndicator),1f);
    }

    public void ShowFinalLapIndicator(){
        HideLapCompletedIndicator();
        HideWrongCheckPointIndicator();
        HideFinalLapIndicator();
        finalLapCompleteIndicator.SetActive(true);
        CancelInvoke(nameof(HideFinalLapIndicator));
        Invoke(nameof(HideFinalLapIndicator),1f);
    }
    private void HideFinalLapIndicator(){
        CancelInvoke(nameof(HideFinalLapIndicator));
        finalLapCompleteIndicator.SetActive(false);
    }
    public void HideLapCompletedIndicator(){
        CancelInvoke(nameof(HideLapCompletedIndicator));
        lapCompletedIndicator.SetActive(false);
    }
    public void SetTime(float time){
        // float seconds = Mathf.FloorToInt(time % 60);
        // float minutes = Mathf.FloorToInt(time / 60);
        timeText.SetText(KeysHolder.NormalizedTime(time,false));
    }
    public void SetLapCounts(int currentLap,int maxLap){
        currentLapsText.SetText(string.Concat("LAPS : ",currentLap," / ",maxLap));
    } 
    public void SetCurrentSpeed(float speed){
        string speedUnit = gameData.GetSpeedType() ? "Km/h":"M/H";
        speedUnitTxt.SetText(speedUnit);
        speedText.SetText(Mathf.RoundToInt(speed + 200).ToString());
    }
    public void SetPositions(int currentPos,int maxPositions){
        currentPositions.SetText(string.Concat("Rank : ",currentPos, " / ",maxPositions));
    }
    public void ShowTrackMiniMap(bool show){
        miniMapTrack.SetActive(show);
    }

    public void ShowHideTutorialUI(bool show){
        tutorialUI.SetActive(show);
    }
    public void CompleteFirstRaceTutorial(){
        driver.CompleteFirstRaceTutorial();
    }
}