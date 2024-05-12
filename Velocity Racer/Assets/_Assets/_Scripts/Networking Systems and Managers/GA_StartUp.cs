using UnityEngine;
using GameAnalyticsSDK;
public class GA_StartUp : MonoBehaviour {
    public static GA_StartUp Instance{get; private set;}
    private void Awake(){
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(Instance.gameObject);
        }
    }
    private void Start(){
        GameAnalytics.Initialize();
    }
}