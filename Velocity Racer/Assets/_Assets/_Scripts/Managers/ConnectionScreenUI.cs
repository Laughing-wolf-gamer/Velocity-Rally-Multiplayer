using TMPro;
using UnityEngine;
public class ConnectionScreenUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI connectionText;
    [SerializeField] private RectTransform refreshingIconUi;


    public void ShowScreen(){
        gameObject.SetActive(true);
    }
    public void ShowScreen(string screenTxt){
        gameObject.SetActive(true);
        connectionText.SetText(screenTxt);
        // SetConnectionText(screenTxt,Color.white);
    }
    public void ShowScreen(string screenTxt,Color textColor){
        gameObject.SetActive(true);
        SetConnectionText(screenTxt,textColor);
    }

    private void SetConnectionText(string screenTxt,Color textColor){
        connectionText.color = textColor;
        connectionText.SetText(screenTxt);
    }
    public void HideScreen(){
        gameObject.SetActive(false);
    }
}