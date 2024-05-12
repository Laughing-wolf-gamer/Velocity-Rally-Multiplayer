using TMPro;
using Dan.Main;
using UnityEngine;
using System.Collections.Generic;
public class FirstRunSytemManager : MonoBehaviour {
    [SerializeField] private Profile profile;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private GameObject setUsernameWindow;
    [SerializeField] private UIButtonCustom saveNewNameButton;
    [SerializeField] private TMP_InputField newUserNameTextField;
    [SerializeField] private TextMeshProUGUI warningMessageTxt;
    [SerializeField] private List<string> badWordsList;
    
    private void Awake(){
        setUsernameWindow.SetActive(false);
        HideWarningText();
        saveNewNameButton.interactable = false;
        if(gameData.IsFirstRun()){
            if(string.IsNullOrEmpty(gameData.playerNameFromSocialMedia)){
                setUsernameWindow.SetActive(true);
            }else{
                SetFaceBookNameAndData();
            }
        }
    }
    private void SetFaceBookNameAndData(){
        HideWarningText();
        string currentFacebookName = gameData.playerNameFromSocialMedia;
        LeaderboardCreator.GetLeaderboard(KeysHolder.leadBoardPublicKey,(recivingData) =>{
            if(IsAvailableName(recivingData,gameData.playerNameFromSocialMedia)){
                gameSettings.gameProfileData.username = currentFacebookName;
                /* HideWarningText();
                string uName = gameSettings.gameProfileData.username;
                char[] first3Letters = new char[3]{uName[0],uName[1],uName[2]};
                gameSettings.gameData.SetUntqueIdNumberRandom(first3Letters);
                saveNewNameButton.interactable = false;
                gameSettings.Save();
                ShowWarningText("<color=white>Saving.....</color>");
                int scoreAveraging = Mathf.RoundToInt((gameData.GetTotalMatchWon() + gameData.GetTotalMedalsWon() + gameData.GetTotalRacesJoined() + gameData.GetTotalGoldTropyWon()) / 4);
                LeaderboardCreator.UploadNewEntry(KeysHolder.leadBoardPublicKey,gameSettings.gameProfileData.username,scoreAveraging,gameSettings.gameData.GetUniqueIdNumber(),((receivingData)=>{ 
                    setUsernameWindow.SetActive(false);
                    profile.RefreshProfileData();
                    SavingAndLoadingManager.Current?.SaveToCloud();
                })); */
            }else{
                currentFacebookName = string.Concat(gameData.playerNameFromSocialMedia,Random.Range(100,20000));
                gameSettings.gameProfileData.username = currentFacebookName;
            }
            string uName = gameSettings.gameProfileData.username;
            char[] first3Letters = new char[3]{uName[0],uName[1],uName[2]};
            gameSettings.gameData.SetUntqueIdNumberRandom(first3Letters);
            saveNewNameButton.interactable = false;
            gameSettings.Save();
            ShowWarningText("<color=white>Saving.....</color>");
            int scoreAveraging = Mathf.RoundToInt((gameData.GetTotalMatchWon() + gameData.GetTotalMedalsWon() + gameData.GetTotalRacesJoined() + gameData.GetTotalGoldTropyWon()) / 4);
            LeaderboardCreator.UploadNewEntry(KeysHolder.leadBoardPublicKey,gameSettings.gameProfileData.username,scoreAveraging,gameSettings.gameData.GetUniqueIdNumber(),(receivingData)=>{ 
                setUsernameWindow.SetActive(false);
                profile.RefreshProfileData();
                SavingAndLoadingManager.Current?.SaveToCloud();
            });
        });
    }
    public void ChangeName() {
        // Call from Save Name Button.
        if(!string.IsNullOrEmpty(newUserNameTextField.text)){
            if(newUserNameTextField.text.Length >= KeysHolder.MIN_PLAYER_NAME_CHAR_LENGHT && newUserNameTextField.text.Length < KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT){
                HideWarningText();
                gameSettings.gameProfileData.username = newUserNameTextField.text;
                string uName = gameSettings.gameProfileData.username;
                char[] first3Letters = new char[3]{uName[0],uName[1],uName[2]};
                gameSettings.gameData.SetUntqueIdNumberRandom(first3Letters);
                saveNewNameButton.interactable = false;
                gameSettings.Save();
                ShowWarningText("<color=white>Saving.....</color>");
                int scoreAveraging = Mathf.RoundToInt((gameData.GetTotalMatchWon() + gameData.GetTotalMedalsWon() + gameData.GetTotalRacesJoined() + gameData.GetTotalGoldTropyWon()) / 4);
                LeaderboardCreator.UploadNewEntry(KeysHolder.leadBoardPublicKey,gameSettings.gameProfileData.username,scoreAveraging,gameSettings.gameData.GetUniqueIdNumber(),((receivingData)=>{ 
                    setUsernameWindow.SetActive(false);
                    profile.RefreshProfileData();
                    SavingAndLoadingManager.Current?.SaveToCloud();
                }));
            }else{
                ShowWarningText($"Username Must be atleast {KeysHolder.MIN_PLAYER_NAME_CHAR_LENGHT} Character Long \n Shorter than {KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT}");
                saveNewNameButton.interactable = true;
            }
        }else{
            ShowWarningText("User Name Cannot be Empty");
            saveNewNameButton.interactable = true;
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
            ShowWarningText($"User name Must be atleast {KeysHolder.MIN_PLAYER_NAME_CHAR_LENGHT} Character Long");
            return;
        }
        if(newName.Length > KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT){
            saveNewNameButton.interactable = false;
            ShowWarningText($"User name Cannot be Longer then {KeysHolder.MAX_PLAYER_NAME_CHAR_LENGHT} Character Long");
            return;
        }
        Debug.Log("Checking New User Name Validity");
        saveNewNameButton.interactable = false;
        HideWarningText();
        LeaderboardCreator.GetLeaderboard(KeysHolder.leadBoardPublicKey,(recivingData) =>{
            if(IsAvailableName(recivingData,newName)){
                HideWarningText();
                saveNewNameButton.interactable = true;
            }else{
                ShowWarningText($"<color=yellow>[{newName}]</color> UserName Already Exist");
                saveNewNameButton.interactable = false;
            }
        });
    }
    private bool IsAvailableName(Dan.Models.Entry[] receivingData,string currentUserName){
        bool available = true;
        for (int i = 0; i < receivingData.Length; i++) {
            if(receivingData[i].Username == currentUserName){
                available = false;
                break;
            }
        }
        return available;
    }
    private bool IsBadWords(string word){
        return badWordsList.Contains(word);
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
}