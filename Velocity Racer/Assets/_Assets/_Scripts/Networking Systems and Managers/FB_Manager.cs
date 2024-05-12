using TMPro;
using System;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class FB_Manager : MonoBehaviour {
    [SerializeField] private ConnectionScreenUI connectionScreen;
    [SerializeField] private GameDataSO gameData;
    public static FB_Manager fB_Manager_Instance{get; private set;}
    public event EventHandler<bool> OnFacbookLoginComplete,OnProfilePictureRecived;
    public TextMeshProUGUI FB_userName;
    public RawImage rawImg;
    #region Initialize

    private void Awake()  {
        if(fB_Manager_Instance != null){
            Destroy(fB_Manager_Instance);
        }else{
            fB_Manager_Instance = this;
        }
        if(!gameData.GetOnPc()){
            StartFacebookInitilaizations();
        }
    }
    public void StartFacebookInitilaizations(){
        FB.Init(SetInit, OnHidenUnity);
        if (!FB.IsInitialized)  {
            FB.Init(() =>  {
                if (FB.IsInitialized){
                    FB.ActivateApp();
                }else{
                    Debug.LogError("Couldn't initialize");
                }
            },
            isGameShown =>  {
                if (!isGameShown){
                    Time.timeScale = 0;
                } else{
                    Time.timeScale = 1;
                    connectionScreen.HideScreen();
                }
            });
        }
        else{
            FB.ActivateApp();
        }
    }

    private void SetInit() {
        bool succes;
        if (FB.IsLoggedIn) {
            Debug.Log("Facebook is Login!");
            string s = "client token- " + FB.ClientToken + "User Id- " + AccessToken.CurrentAccessToken.UserId + "token string- " + AccessToken.CurrentAccessToken.TokenString;
            Debug.Log(s);
            succes = true;
        } else {
            Debug.Log("Facebook is not Logged in!");
            succes = false;
        }
        DealWithFbMenus(FB.IsLoggedIn);
        OnFacbookLoginComplete?.Invoke(this,succes);
    }

    void OnHidenUnity(bool isGameShown)  {
        if (!isGameShown) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
            connectionScreen.HideScreen();
        }
    }

    private void DealWithFbMenus(bool isLoggedIn) {
        if (isLoggedIn) {
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        } else {
            Debug.Log("Not logged in");
            OnFacbookLoginComplete?.Invoke(this,false);
        }
    }
    private void DisplayUsername(IResult result) {
        if (result.Error == null) {
            string name = "" + result.ResultDictionary["first_name"];
            if (FB_userName != null){
                FB_userName.SetText(name);
                gameData.playerNameFromSocialMedia = name;
                // set username...
            } 
            FB_userName.text = name;
            Debug.Log("" + name);
        } else {
            Debug.Log(result.Error);
        }
    }
    private void DisplayProfilePic(IGraphResult result) {
        bool succes = false;
        if (result.Texture != null) {
            Debug.Log("Profile Pic Recived");
            rawImg.texture = result.Texture;
            gameData.socialMediaProfilePicture = result.Texture;

            succes = true;
           /* //if (FB_profilePic != null) FB_profilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
            JSONObject json = new JSONObject(result.RawResult);

            StartCoroutine(DownloadTexture(json["picture"]["data"]["url"].str, profile_texture));*/
        } else {
            succes = false;
            Debug.Log(result.Error);
        }
        OnProfilePictureRecived?.Invoke(this,succes);
    }



    #endregion


    //login
    public void Facebook_LogIn() {
        connectionScreen.ShowScreen("<color=white> Logging In....</color>");
        List<string> permissions = new List<string> {
            "public_profile","user_friends","gaming_profile", "gaming_user_picture"
        };
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }
    private void AuthCallBack(IResult result) {
        if (FB.IsLoggedIn) {
            gameData.ToggleLogedIn(true);
            SetInit();
            Debug.Log("Facebook Login Complete");
            //AccessToken class will have session details
            AccessToken aToken = AccessToken.CurrentAccessToken;
            Debug.Log("Token String= " + aToken.TokenString);
            Debug.Log(aToken.UserId);
            foreach (string perm in aToken.Permissions) {
                Debug.Log(perm);
            }
            // CloudSavingSystem.CloudInstance.SighInFromFacebook(aToken.TokenString);
        } else {
            Debug.Log("Failed to log in");
            connectionScreen.HideScreen();
            OnFacbookLoginComplete?.Invoke(this,false);
        }

    }




    //logout
    public void Facebook_LogOut() {
        StartCoroutine(LogOut());
    }
    IEnumerator LogOut() {
        FB.LogOut();
        while (FB.IsLoggedIn) {
            Debug.Log("Logging Out");
            yield return null;
        }
        Debug.Log("Logout Successful");
       // if (FB_profilePic != null) FB_profilePic.sprite = null;
        if (FB_userName != null) /* FB_userName.text = ""; */
        if (rawImg != null) rawImg.texture = null;
    }


    #region other

    public void FacebookSharefeed() {
        string url = "https:developers.facebook.com/docs/unity/reference/current/FB.ShareLink";
        FB.ShareLink(
            new Uri(url),
            "Checkout COCO 3D channel",
            "I just watched " + "22" + " times of this channel",
            null,
            ShareCallback);

    }

    private static void ShareCallback(IShareResult result) {
        Debug.Log("ShareCallback");
        SpentCoins(2, "sharelink");
        if (result.Error != null) {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);
    }
    public static void SpentCoins(int coins, string item) {
        var param = new Dictionary<string, object>();
        param[AppEventParameterName.ContentID] = item;
        FB.LogAppEvent(AppEventName.SpentCredits, (float)coins, param);
    }

    /*public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            Debug.Log("the raw" + result.RawResult);
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];

            foreach (var dict in friendsList)
            {
                GameObject go = Instantiate(friendstxtprefab);
                go.GetComponent<Text>().text = ((Dictionary<string, object>)dict)["name"].ToString();
                go.transform.SetParent(GetFriendsPos.transform, false);
                FriendsText[1].text += ((Dictionary<string, object>)dict)["name"];
            }
        });
    }*/

    #endregion
}