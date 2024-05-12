using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TabGroup : MonoBehaviour {
    [SerializeField] private List<CustomTabButton> tabButtons;
    [SerializeField] private List<GameObject> objectsToSwap;
    [SerializeField] private CustomTabButton selectedTab;
    [SerializeField] private Sprite idle,selected;
    private void Start(){
        ResetTab();
    }
    public void Subscribe(CustomTabButton tabButton){
        if(tabButtons == null){
            tabButtons = new List<CustomTabButton>();
        }
        tabButtons.Add(tabButton);
    }
    public void OnTabSelected(CustomTabButton tabButton){
        selectedTab = tabButton;
        ResetTab();
        tabButton.SetBackGroundSprite(selected);
        int index = tabButton.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++) {
            if(i==index){
                objectsToSwap[i].SetActive(true);
            }else{
                objectsToSwap[i].SetActive(false);
            }
        }
    }
    public void ResetTab(){
        foreach(CustomTabButton customTab in tabButtons){
            if(customTab == selectedTab){continue;}
            customTab.SetBackGroundSprite(idle);
        }
    }
}