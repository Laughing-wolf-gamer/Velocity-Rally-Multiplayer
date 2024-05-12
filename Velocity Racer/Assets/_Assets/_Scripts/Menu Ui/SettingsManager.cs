using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour {
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private Slider musicSlider,sfxSlider,qualitySettingsSlider,framRateSettingSlider,controllsSensitivtySlider;
    [SerializeField] private TextMeshProUGUI speedUnit,frameRateText,graphicsQualityTxt,senstivityAmountTxt;
    [SerializeField] private Toggle hapticToggle,showNetworkPing;
    private void Start(){
        gameData.SetControllsSensitivity(gameData.GetControllsSensitivity());
        senstivityAmountTxt.SetText(string.Concat(Mathf.RoundToInt(gameData.GetControllsSensitivity())));
        controllsSensitivtySlider.value = gameData.GetControllsSensitivity();
        regionText.SetText(string.Concat("Region: ",gameData.GetRegion()));
        musicSlider.value = gameData.GetMusicVolumeAmount();
        sfxSlider.value = gameData.GetSfxVolumeAmount();
        qualitySettingsSlider.value = gameData.GetQualityIndex();
        framRateSettingSlider.value = gameData.GetFrameRate();
        showNetworkPing.isOn = gameData.GetShowPing();
        hapticToggle.isOn = gameData.GetHapticState();
        speedUnit.SetText(gameData.GetSpeedType() ? "Km/h" : "M/h");
        frameRateText.SetText(string.Concat("FRAME RATE : ",Mathf.RoundToInt(gameData.GetFrameRate())));
        Application.targetFrameRate = gameData.GetFrameRate();
        gameData.SetQualityLevel(gameData.GetQualityIndex());

    }
    public void SetMusicVolumeAmount(float amount){
        gameData.SetMusicVolumeAmount(amount);
    }
    public void SetSfxVolumeAmount(float amount){
        gameData.SetSfxVolumeAmount(amount);
    }
    public void OnHapticTogglePress(){
        gameData.ToggleHaptics();
    }
    public void ToggleSpeedType(){
        gameData.ToggleSpeedType();
        speedUnit.SetText(gameData.GetSpeedType() ? "Km/h" : "M/h");
    }
    public void ToggleShowNetworkPing(){
        gameData.ToggleShowNetworkPing();
    }
    public void SetQuality(float index){
        gameData.SetQualityLevel(Mathf.RoundToInt(index));
        QualitySettings.SetQualityLevel(gameData.GetQualityIndex());
        graphicsQualityTxt.SetText(GetQualtiyType(gameData.GetQualityIndex()));
    }
    private string GetQualtiyType(int index){
        switch(index){
            case 0: return "Low";
            case 1 : return "Good";
            case 2 : return "Performant";
            case 3 : return "Best";
        }
        return "";
    }
    public void OnSensitivtyChange(float amount){
        senstivityAmountTxt.SetText(string.Concat(Mathf.RoundToInt(amount)));
        gameData.SetControllsSensitivity(amount);
    }
    public void OnFrameRateChangeSlider(float frameRate){
        frameRateText.SetText(string.Concat("FRAME RATE : ",Mathf.RoundToInt(frameRate)));
        gameData.SetFrameRate(Mathf.RoundToInt(frameRate));
        Application.targetFrameRate = gameData.GetFrameRate();

    }
}