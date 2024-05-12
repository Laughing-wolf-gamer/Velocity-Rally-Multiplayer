using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Profile : MonoBehaviour {
    [SerializeField] private RawImage[] profileImage;
    [SerializeField] private List<string> badWordsList;
    [SerializeField] private int maxProfileNameLength = 6;
    [SerializeField] private GameSettingsSO gameSettingsSo;
    [SerializeField] private GameDataSO gameDataSo;
    [SerializeField] private WorldLeaderboardSystem worldLeaderboardSystem;
    [SerializeField] private TMP_InputField newUserNameTextField;
    [SerializeField] private MenuSystem menuSystem;
    [SerializeField] private TextMeshProUGUI[] tropyCountTmPro,levelNumberTmPro;
    [SerializeField] private TextMeshProUGUI totalMedalsCountText,totalRacesJoinedText,totalMatchWonText,totalTimePlayed;
    [SerializeField] private TextMeshProUGUI idNumberTmPro;
    [SerializeField] private TextMeshProUGUI userNameMenuTmPro,userNameProfileTmpro;
    [SerializeField] private Image[] experianceAmountBar;
    [SerializeField] private LevelSystemSO levelSystem;
    [SerializeField] private UIButtonCustom saveNewNameButton;
    [SerializeField] private TextMeshProUGUI warningMessageTxt;
    [SerializeField] private UIButtonCustom facebookLoggInButton,faceBookLogoutButton;
    private bool changingName;
    private void Awake(){
        RefreshProfileData();
    }
    public void RefreshProfileData(){
        facebookLoggInButton.interactable = !gameDataSo.GetIsLoggedIn();
        faceBookLogoutButton.interactable = gameDataSo.GetIsLoggedIn();
        if(gameDataSo.socialMediaProfilePicture != null) {
            foreach(RawImage rImage in profileImage){
                rImage.texture = gameDataSo.socialMediaProfilePicture;
            }
        }
        HideWarningText();
        ShowProfileName();
        SetProfileData();
        SetExpAndLevelDatas();
        SavingAndLoadingManager.Current?.SaveToCloud();
    }
    private void SetExpAndLevelDatas(){
        foreach(TextMeshProUGUI levelNum in levelNumberTmPro){
            levelNum.SetText(string.Concat("LEVEL ",levelSystem.GetLevelNumber()));
        }
        foreach(Image expImage in experianceAmountBar){
            expImage.fillAmount = levelSystem.GetExperianceNormalized();
        }
    }
    private void SetProfileData(){
        totalMedalsCountText.SetText(KeysHolder.GetAmountNormalized(gameDataSo.GetTotalMedalsWon()));
        totalRacesJoinedText.SetText(KeysHolder.GetAmountNormalized(gameDataSo.GetTotalRacesJoined()));
        totalMatchWonText.SetText(KeysHolder.GetAmountNormalized(gameDataSo.GetTotalMatchWon()));
        totalTimePlayed.SetText(KeysHolder.NormalizedTime(gameDataSo.GetGamePlayedTime()));
    }
    private void ShowProfileName(){
        userNameMenuTmPro.SetText(KeysHolder.StringShorting(gameSettingsSo.gameProfileData.username),maxProfileNameLength);
        userNameProfileTmpro.SetText(gameSettingsSo.gameProfileData.username);
        idNumberTmPro.SetText(gameDataSo.GetUniqueIdNumber());
        foreach(TextMeshProUGUI tropy in tropyCountTmPro){
            tropy.SetText(KeysHolder.GetAmountNormalized(gameDataSo.GetTotalGoldTropyWon()));
        }
    }
    public void ChangeName(){
        // Call from Save Name Button.
        if(!string.IsNullOrEmpty(newUserNameTextField.text)){
            if(newUserNameTextField.text.Length >= KeysHolder.MIN_PLAYER_NAME_CHAR_LENGHT && newUserNameTextField.text.Length < KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT){
                HideWarningText();
                gameSettingsSo.gameProfileData.username = newUserNameTextField.text;
                worldLeaderboardSystem.UpgradeEntryUserName();
                SavingAndLoadingManager.Current.SaveGame();
                ShowProfileName();
                newUserNameTextField.text = string.Empty;
                menuSystem.OpenProfile();
            }else{
                ShowWarningText($"Username Must be atleast {KeysHolder.MIN_PLAYER_NAME_CHAR_LENGHT} Character Long \n Shorter than {KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT}");
                saveNewNameButton.interactable = true;
            }
        }else{
            ShowWarningText("User Name Cannot be Empty");
        }
    }
    public void CheckUniequeName(string newName){
        if(string.IsNullOrEmpty(newName)){
            ShowWarningText($"UserName Cannot Be Nothing");
            saveNewNameButton.interactable = false;
            return;
        }
        if(IsBadWords(newName.ToLower())){
            ShowWarningText($"<color=yellow>[WARNNING]</color> : You Cannot Use this UserName ");
            saveNewNameButton.interactable = false;
            return;
        }
        if(newName.Length < KeysHolder.MIN_PLAYER_NAME_CHAR_LENGHT){
            saveNewNameButton.interactable = false;
            ShowWarningText("User name Must be atleast 6 Character Long");
            return;
        }
        if(newName.Length > KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT){
            saveNewNameButton.interactable = false;
            ShowWarningText("User name Cannot be Longer then 12 Character Long");
            return;
        }
        saveNewNameButton.interactable = false;
        HideWarningText();
        worldLeaderboardSystem.CheckNewNameIsAlreadyPresent(newName,(isPresent) =>{
            if(isPresent){
                ShowWarningText($"<color=yellow>[{newName}]</color> UserName Already Exist");
                saveNewNameButton.interactable = false;
            }else{
                saveNewNameButton.interactable = true;
                HideWarningText();
            }
        });
    }
    private void ShowWarningText(string warning){
        warningMessageTxt.gameObject.SetActive(true);
        warningMessageTxt.SetText(warning);
        CancelInvoke(nameof(HideWarningText));
        Invoke(nameof(HideWarningText),5f);
    }
    private void HideWarningText(){
        warningMessageTxt.gameObject.SetActive(false);
        warningMessageTxt.SetText(string.Empty);
        CancelInvoke(nameof(HideWarningText));
    }

    private bool IsBadWords(string word){
        return badWordsList.Contains(word);
    }
    public void LogOutFacebook(){
        // find a way to LogOut...
    }

}