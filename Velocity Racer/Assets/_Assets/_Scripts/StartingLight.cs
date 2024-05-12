using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class StartingLight : MonoBehaviour {
    [SerializeField] private Image[] lights;
    public void SetLightColor(Color lightsColor){
        foreach(Image rend in lights){
            rend.color = lightsColor;
        }
    }
}