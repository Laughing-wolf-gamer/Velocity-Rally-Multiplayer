using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectionUiBtn : MonoBehaviour {
    [SerializeField] private LevelSystemSO levelSystem;
    [SerializeField] private MultiplayerMatchMakingManager multiplayerMatchMakingManager;
    [SerializeField] private RawImage trackImage;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TrackSO track;
    [SerializeField] private TextMeshProUGUI selectedText,requirdLevelNumberAmountTxt;
    [SerializeField] private Button selectionBtn;
    [SerializeField] private Color selectedColor = Color.green,nonSelected = Color.red;
    private void Start(){
        trackImage.texture = track.trackTrackTexture;
        if(track.trackSaveData.isLocked){
            if(!track.CanUnlock(levelSystem.GetLevelNumber())){
                Debug.Log("Cannot Unlock");
                requirdLevelNumberAmountTxt.gameObject.SetActive(true);
                requirdLevelNumberAmountTxt.SetText(string.Concat("Required Level ", track.trackSaveData.levelRequiredToUnlock));
                Color trackAlphaColor = trackImage.color;
                trackAlphaColor.a = .2f;
                trackImage.color = trackAlphaColor;
                lockIcon.SetActive(true);
            }else{
                Debug.Log("Can Unlock");
                Color trackAlphaColor = trackImage.color;
                trackAlphaColor.a = 1f;
                trackImage.color = trackAlphaColor;
                lockIcon.SetActive(false);
                selectionBtn.interactable = true;
                selectedText.SetText("UNLOCKED");
                requirdLevelNumberAmountTxt.gameObject.SetActive(false);
            }
        }else{
            requirdLevelNumberAmountTxt.gameObject.SetActive(false);
        }
    }
    public void SelectCurrentTrack(){
        multiplayerMatchMakingManager.SetCurrentTrack(track);
    }
    public TrackTypes GetTracks(){
        return track.trackSaveData.tracks;
    }
    public void ShowHideTrackBtn(bool show){
        if(track.trackSaveData.isLocked){
            selectionBtn.interactable = false;
            Color trackAlphaColor = trackImage.color;
            trackAlphaColor.a = .2f;
            trackImage.color = trackAlphaColor;
            lockIcon.SetActive(true);
            selectedText.SetText("LOCKED");
            
        }else{
            Color trackAlphaColor = trackImage.color;
            trackAlphaColor.a = 1f;
            trackImage.color = trackAlphaColor;
            lockIcon.SetActive(false);
            selectionBtn.interactable = show;
            if(!show){
                selectedText.color = selectedColor;
            }else{
                selectedText.color = nonSelected;
            }
            selectedText.SetText(!show ? "SELECTED" : "NOT SELECTED");
        }
    }
}