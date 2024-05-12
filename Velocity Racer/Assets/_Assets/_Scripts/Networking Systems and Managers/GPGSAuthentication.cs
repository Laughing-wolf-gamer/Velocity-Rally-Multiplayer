using System;
using TMPro;
using System.IO;
using UnityEngine;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Runtime.Serialization.Formatters.Binary;
using Baracuda.Monitoring;
using System.Collections;
using UnityEngine.UI;
public class GPGSAuthentication : MonoBehaviour {
    public event EventHandler<GPGSOperationArgs> OnGPGSOperationCompleted;
    public class GPGSOperationArgs : EventArgs{
        public bool signInComplete,profilePhotoLoaded;
    }
    public static GPGSAuthentication gPGSAuthenticationInstance{get;private set;}
    // [SerializeField] private RawImage rawImg;
    // [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private LevelSystemSO levelData;
    [SerializeField] private CarBodySO playerCarBody;
    [Monitor] public string Token;
    [Monitor] public string Error;
    // private Texture2D savedImage;

    private bool signInComplete,profilePhotoLoaded;

    private void Awake(){
        Monitor.StartMonitoring(this);
        // Or use this extension method:
        this.StartMonitoring();
        if(gPGSAuthenticationInstance == null){
            gPGSAuthenticationInstance = this;
        }else{
            Destroy(gPGSAuthenticationInstance.gameObject);
        }
    }
    private void OnDestroy() {
        Monitor.StopMonitoring(this);
        // Or use this extension method:
        this.StopMonitoring();
    }
    public void SignIn() {
        //Initialize PlayGamesPlatform
        // PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    

    

    private  void ProcessAuthentication(SignInStatus status) {
        if (status == SignInStatus.Success) {
            signInComplete = true;
            // Continue with Play Games Services
            Debug.Log("SucceccFully Logged In");
            Debug.Log("Login with Google Play games successful.");
            // StartCoroutine(LoadProfilePhoto());
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => {
                Token = code;
                Debug.Log($"Authorization code Recived = {code}");
                // This token serves as an example to be used for SignInWithGooglePlayGames
                CloudSavingSystem.CloudInstance.SignInGooglePlay(Token);
            });
            // OpenSavedGame(KeysHolder.CLOUD_SAVE_DATA_KEY);
            // ShowSelectUI();
        } else {
            signInComplete = false;
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            Debug.Log("Not SucceccFully Logged In " + status);
            Debug.Log("Trying to Mannualy Authenticate");
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }
        OnGPGSOperationCompleted?.Invoke(this,new GPGSOperationArgs{signInComplete = signInComplete,profilePhotoLoaded = profilePhotoLoaded});
    }
    /* private IEnumerator LoadProfilePhoto(){
        profilePhotoLoaded = false;
        Debug.Log("User Name Set....");
        if(PlayGamesPlatform.Instance.localUser.userName != string.Empty){
            userNameText.SetText(PlayGamesPlatform.Instance.localUser.userName);
            gameData.playerNameFromSocialMedia = PlayGamesPlatform.Instance.localUser.userName;
        }
        while(PlayGamesPlatform.Instance.localUser.image == null){
            Debug.Log("Image Not Found");
            gameData.socialMediaProfilePicture = null;
            yield return null;
        }
        Debug.Log("Image Found");
        rawImg.texture = PlayGamesPlatform.Instance.localUser.image;
        gameData.socialMediaProfilePicture = PlayGamesPlatform.Instance.localUser.image;
        profilePhotoLoaded = true;
    } */
    /* private void ShowSelectUI() {
        uint maxNumToDisplay = 5;
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI(KeysHolder.CLOUD_SAVE_DATA_KEY,
            maxNumToDisplay,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
    }
    public void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata game) {
        if (status == SelectUIStatus.SavedGameSelected) {
            Debug.Log("Google Play Save Data Recived");
            // handle selected game save
            LoadGameData(game);
        } else {
            // handle cancel or error
        }
    }
    public void OpenSavedGame(string filename) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            // handle reading or writing of saved game.
            // handle error
            Debug.Log("Save Game Open");
            gameData.SetCurrentCar(playerCarBody.currentCarType);
            gameSettings.gameProfileData.carTypeShop = (int)playerCarBody.currentCarType;
            GPGSCloudSaveData dataToSave = new GPGSCloudSaveData {
                cloudGameSaveData = gameData.saveData,
                levelSystemSaveData = levelData.levelSystemSaveData,
                cloudGameProfileSaveData = gameSettings.gameProfileData,
            };
            // SaveGame(game,ObjectToByteArray(dataToSave),TimeSpan.Zero);
            LoadGameData(game);
        } else {
            // LoadGameData(game);
            Debug.Log("Error Opeing Save Game");
        }
    }
    public void SaveGame (ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder.WithUpdatedPlayedTime(totalPlaytime).WithUpdatedDescription("Saved game at " + DateTime.Now);
        savedImage = GetScreenshot();
        if (savedImage != null) {
            // This assumes that savedImage is an instance of Texture2D
            // and that you have already called a function equivalent to
            // getScreenshot() to set savedImage
            // NOTE: see sample definition of getScreenshot() method below
            byte[] pngData = savedImage.EncodeToPNG();
            builder = builder.WithUpdatedPngCoverImage(pngData);
        }
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }
    public static byte[] ObjectToByteArray(object obj) {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream()) {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
    

    public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            // handle reading or writing of saved game.
            Debug.Log("Game Data Saved On Cloud");
            profilePhotoLoaded = true;
        } else {
            // handle error
            Debug.Log("Save Game Is Not Successfull");
            profilePhotoLoaded = false;
        }
        OnGPGSOperationCompleted?.Invoke(this,new GPGSOperationArgs{signInComplete = signInComplete,profilePhotoLoaded = profilePhotoLoaded});
    }

    public Texture2D GetScreenshot() {
        // Create a 2D texture that is 1024x700 pixels from which the PNG will be
        // extracted
        Texture2D screenShot = new Texture2D(1024, 700);

        // Takes the screenshot from top left hand corner of screen and maps to top
        // left hand corner of screenShot texture
        screenShot.ReadPixels(
            new Rect(0, 0, Screen.width, Screen.width/1024*700), 0, 0);
        return screenShot;
    }
    public void LoadGameData (ISavedGameMetadata game) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
        if (status == SavedGameRequestStatus.Success) {
            // handle processing the byte array data
            // JsonUtility.FromJsonOverwrite(ByteArrayToObject(data),RecivingData);
            GPGSCloudSaveData RecivingData = (GPGSCloudSaveData) ByteArrayToObject(data);
            // From byte array to string
            string s = Encoding.UTF8.GetString(data, 0, data.Length);
            Debug.Log("Set Data " + s);
            gameData.saveData = RecivingData.cloudGameSaveData;
            gameSettings.gameProfileData = RecivingData.cloudGameProfileSaveData;
            levelData.levelSystemSaveData = RecivingData.levelSystemSaveData;
            if(playerCarBody.currentCarType != gameData.GetCarTypeShop()){
                playerCarBody.currentCarType = gameData.GetCarTypeShop();
            }
            profilePhotoLoaded = true;
        } else {
            profilePhotoLoaded = false;
            // handle error
            // OpenSavedGame(KeysHolder.CLOUD_SAVE_DATA_KEY);
        }
        OnGPGSOperationCompleted?.Invoke(this,new GPGSOperationArgs{signInComplete = signInComplete,profilePhotoLoaded = profilePhotoLoaded});

    }

    // Convert a byte array to an Object
    public static object ByteArrayToObject(byte[] arrBytes) {
        var memStream = new MemoryStream();
        var binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        var obj = binForm.Deserialize(memStream);
        return obj;
    } */
    /* [Serializable]
    private class GPGSCloudSaveData{
        public GameDataSO.PlayerSaveData cloudGameSaveData;
        public LevelSystemSO.LevelSystemSaveData levelSystemSaveData;
        public GameProifileData cloudGameProfileSaveData;
    } */
}