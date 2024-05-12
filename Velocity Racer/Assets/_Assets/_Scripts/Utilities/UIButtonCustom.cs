using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIInputReciver),typeof(UIInputHandler))]
public class UIButtonCustom : Button{
    private UIInputReciver reciver;
    protected override void Awake(){
        base.Awake();
        reciver = GetComponent<UIInputReciver>();
        onClick.AddListener(() =>{
            reciver.OnInputRecives();
        });
    }
    
}
