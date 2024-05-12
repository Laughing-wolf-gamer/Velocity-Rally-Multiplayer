using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardPlayerCard : MonoBehaviour {
    [SerializeField] private Ease moveingEaseType;
    [SerializeField] private Image cardImage,rankHeader;
    [SerializeField] private TextMeshProUGUI usernameText,rankText,finalTimeText;
    [SerializeField] private Sprite firstRankBodySprite,firstRankHeader,normalRankBodySprite,normalRankHeader;
    [SerializeField] private Vector3 moveToPoint;
    [SerializeField] private Vector3 startingPoint;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject youIndicator;
    private int rank;
    public void SetCardsData(int rank,string username,bool isLocalPlayer,float finalTime = 0f){
        transform.SetSiblingIndex(rank - 1);
        youIndicator.SetActive(isLocalPlayer);
        if(rank == 1){
            finalTimeText.color = Color.blue;
            usernameText.color = Color.white;
            cardImage.sprite = firstRankBodySprite;
            rankHeader.sprite = firstRankHeader;
        }else {
            finalTimeText.color = Color.white;
            usernameText.color = Color.white;
            cardImage.sprite = normalRankBodySprite;
            rankHeader.sprite = normalRankHeader;
        }
        finalTimeText.SetText(KeysHolder.NormalizedTime(finalTime,false));
        usernameText.SetText(username);
        SetRank(rank);
    }
    

    public void SetRank(int rank) {
        this.rank = rank;
        this.rankText.SetText(string.Concat("#",rank));
    }
    public void ShowCard(){
        gameObject.SetActive(true);
        Show();
    }
    public void HideCard(){
        Hide();
    }
    public void Show(){
        rectTransform.DOKill(false);
        rectTransform.anchoredPosition = startingPoint;
        float delay = ((float)rank + .5f) / 6;// (1 + .5)/6=0.25
        rectTransform.DOAnchorPos(moveToPoint,1f,false).SetDelay(delay).SetEase(moveingEaseType);
    }
    public void Hide(){
        rectTransform.DOAnchorPos(startingPoint,.1f,false).SetEase(moveingEaseType).onComplete += ()=>{gameObject.SetActive(false);};
    }
    
}