using TMPro;
using UnityEngine;
using DG.Tweening;
public class PlayerJoinedCard : MonoBehaviour {
    [SerializeField] private Ease moveingEaseType;
    [SerializeField] private TextMeshProUGUI usernameText,rankText;
    [SerializeField] private RectTransform cardBody;
    [SerializeField] private Vector3 moveToPoint,startingPoint;
    public void SetCardsData(int rank,string username){
        transform.SetSiblingIndex(rank - 1);
        usernameText.color = Color.white;
        usernameText.SetText(username);
        SetRank(rank);
    }
    public void Show(){
        cardBody.DOKill(false);
        cardBody.anchoredPosition = startingPoint;
        cardBody.DOAnchorPos(moveToPoint,.8f,false).SetDelay(.5f).SetEase(moveingEaseType);
    }
    public void Hide(){
        // cardBody.anchoredPosition = startingPoint;
        cardBody.DOAnchorPos(startingPoint,.2f,false).SetEase(moveingEaseType).onComplete += ()=>{gameObject.SetActive(false);};
    }
    public void SetRank(int rank) {
        rankText.SetText(string.Concat("#",rank));
    }
    public void ShowCard(){
        gameObject.SetActive(true);
        Show();
    }
    public void HideCard(){
        Hide();
    }
}