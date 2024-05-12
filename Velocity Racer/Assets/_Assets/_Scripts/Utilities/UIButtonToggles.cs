using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class UIButtonToggles : MonoBehaviour {
    
    [SerializeField] private Sprite enableColor;
    [SerializeField] private Sprite desableColor;


    [SerializeField] private Button graphics;

    private void Awake(){
        if(graphics == null){
            graphics = GetComponent<Button>();
        }
    }
    public void Toggle(bool on){

        if(on){
            graphics.image.sprite = enableColor;
        }else{
            graphics.image.sprite = desableColor;
        }
    }
    
}
