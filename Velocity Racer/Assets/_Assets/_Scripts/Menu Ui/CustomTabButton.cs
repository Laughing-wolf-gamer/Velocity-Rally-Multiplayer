using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CustomTabButton : MonoBehaviour,IPointerClickHandler {
    [SerializeField] private TabGroup tabGroup;
    [SerializeField] private Image backGround;


    public void OnPointerClick(PointerEventData eventData) {
        tabGroup.OnTabSelected(this);
    }
    public void SetBackGroundSprite(Sprite sprite){
        backGround.sprite = sprite;
    }

    private void Start(){
        tabGroup.Subscribe(this);
    }
}