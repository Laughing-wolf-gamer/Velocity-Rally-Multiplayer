using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WorldLeaderboardCard : MonoBehaviour {
    [SerializeField] private Sprite firstRankCardVisual,secondRankCardVisual,thridRankCardVisual,normalRankCardVisual;
    [SerializeField] private Sprite firstRankIcon,secondRankIcon,thridRankIcon;
    [SerializeField] private Image rankIcon;
    [SerializeField] private Image rankCardVisual;
    [SerializeField] private TextMeshProUGUI rankText,userNameTxt,scoreTxt,extraText;
    private void Awake(){
        HideCard();
    }
    public void ShowCard(){
        gameObject.SetActive(true);
    }
    public void SetCardData(int rank,string username,int score){
        transform.SetSiblingIndex(rank - 1);
        if(rankIcon != null){
            rankIcon.gameObject.SetActive(rank <= 3);
        }
        string rankSufix = " th";
        if(rank == 1){
            rankIcon.sprite = firstRankIcon;
            rankCardVisual.sprite = firstRankCardVisual;
            rankSufix = " st";
        }else if(rank == 2){
            rankIcon.sprite = secondRankIcon;
            rankCardVisual.sprite = secondRankCardVisual;
            rankSufix = " nd";
        }else if(rank == 3){
            rankIcon.sprite = thridRankIcon;
            rankCardVisual.sprite = thridRankCardVisual;
            rankSufix = " rd";
        }else{
            rankCardVisual.sprite = normalRankCardVisual;
        }
        rankText.SetText(string.Concat(rank,rankSufix));
        userNameTxt.SetText(username);
        scoreTxt.SetText(score.ToString());
    }
    public void HideCard(){
        gameObject.SetActive(false);
    }
}