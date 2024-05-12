using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class InitializingManager : MonoBehaviourPunCallbacks {
    [SerializeField] private Image loadingBar;
    [SerializeField] private ConnectionScreenUI connectionScreen;
    [SerializeField] private TextMeshProUGUI versionNum;
    [SerializeField] private GameDataSO gameData;
    public void Awake(){
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        versionNum.SetText(string.Concat("v",Application.version,PhotonNetwork.PunVersion));
    }
    private void Start(){
        GPGSAuthentication.gPGSAuthenticationInstance.OnGPGSOperationCompleted += (object sender,GPGSAuthentication.GPGSOperationArgs e)=>{
            if(e.signInComplete && e.profilePhotoLoaded){
                Debug.Log("Authentication Result " + e.signInComplete);
                Debug.Log("Profile Data Recived And Google Play, Results : " + e.profilePhotoLoaded);
            }
            connectionScreen.HideScreen();
            /* 
            if(!e.profilePhotoLoaded && !gameData.IsFirstRun()){
                LoadToMenu();
            } */
        };
        FB_Manager.fB_Manager_Instance.OnFacbookLoginComplete += (object sender, bool Successful)=>{
            Debug.Log("Facebook Login Result " + Successful);
            connectionScreen.HideScreen();
            /* 
            if(!Successful && !gameData.IsFirstRun()){
                LoadToMenu();
            } */
        };
        FB_Manager.fB_Manager_Instance.OnProfilePictureRecived += (object sender,bool Successful) =>{
            Debug.Log("Facebook Profile Pic Recived Result " + Successful);
            // Load to the Menu after Profile Pic recived....
            connectionScreen.HideScreen();
            GPGSAuthentication.gPGSAuthenticationInstance.SignIn();
        };
        CloudSavingSystem.CloudInstance.OnAuthSuccessFull += (success)=>{
            Debug.Log("Unity Authentication with Google Play Games Result : " + success);
            connectionScreen.HideScreen();
            LoadToMenu();
        };
        if(gameData.IsFirstRun()){
            // Load to the Menu Scene. if Not on Mobile.
            if(gameData.GetOnPc()){
                LoadToMenu();
            }
        }else{
            if(gameData.GetOnPc()){
                LoadToMenu();
            }else{
                // Auto Login... if Not a first Run....
                connectionScreen.ShowScreen("AUTO LOGIN");
                FB_Manager.fB_Manager_Instance.Facebook_LogIn();
            }
        }
    }
    
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }
    public void LoadToMenu(){
        StartCoroutine(GetLoadSceneProgress(1));
    }
    private IEnumerator GetLoadSceneProgress(int SceneIndex){
        connectionScreen.ShowScreen();
        float totalProgress;
        loadingBar.fillAmount = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneIndex,LoadSceneMode.Single);
        while(!operation.isDone){
            totalProgress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = totalProgress;
            yield return null;
        }
        connectionScreen.HideScreen();
    }
}