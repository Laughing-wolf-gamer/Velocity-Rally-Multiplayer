using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using GamerWolf.Utils;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class MultiplayerMatchMakingManager : MonoBehaviourPunCallbacks {
    [SerializeField] private MenuSystem menuSystem;
    [SerializeField] private TrackSO[] trackArray;
    [SerializeField] private TrackSelectionUiBtn[] trackSelectionUiBtns;
    [SerializeField] private GameSettingsSO gameSettingsSo;
    [SerializeField] private Slider maxPlayersSlider,lapCountSlider;
    [SerializeField] private TextMeshProUGUI maxPlayersValue,lapCountSliderValue;
    [SerializeField] private ConnectionScreenUI connectingWindow;
    [SerializeField] private OpponnentSerachingWindow competitorsSearchingWindow;
    [SerializeField] private UIButtonCustom quickStartBtn;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Toggle hasBotsToggle;
    private List<RoomInfo> currentRoomList;
    private bool isRankingMatch;
    private byte rankingMatchPlayerCount;
    private int rankingMatchLapCount;
    private TrackTypes rankingTrackType;
    private Coroutine CheckInternetConnectionRoutine;
    private IEnumerator PingServer() {
        while (true) {
            float ping = PhotonNetwork.GetPing();
            if(ping > 500){
                menuSystem.ShowHighPingWarningWindow(ping,Color.red);
            }else if(ping > 200){
                menuSystem.ShowHighPingWarningWindow(ping,Color.yellow);
            }else{
                menuSystem.HideHighPingWarningWindow();
            }
            yield return new WaitForSeconds(1f);
        }
    }
    private void Awake(){
        PhotonNetwork.PhotonServerSettings.AppSettings.EnableLobbyStatistics = true;
        gameSettingsSo.gameData.SetRegion(PhotonNetwork.CloudRegion);
        hasBotsToggle.isOn = gameSettingsSo.hasBots;
        lapCountSlider.value = gameSettingsSo.currentLap;
        lapCountSliderValue.SetText(string.Concat("LAPS : ",gameSettingsSo.currentLap));
        maxPlayersSlider.value = gameSettingsSo.maxPlayer;
        maxPlayersValue.SetText(string.Concat("MAX PLAYER : ",gameSettingsSo.maxPlayer));
        quickStartBtn.interactable = true;
        connectingWindow.HideScreen();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    public override void OnRegionListReceived(RegionHandler regionHandler) {
        Debug.Log("Region" + regionHandler.BestRegion);
        List<Region> regionList = regionHandler.EnabledRegions;
        Debug.Log("Regions List " + regionList.Count);
        base.OnRegionListReceived(regionHandler);
    }
    private void Start(){
        // CheckInternetConnectionRoutine = StartCoroutine(CheckNetworkConnection());
        RefreshTrackTypesUi();
        if(!PhotonNetwork.IsConnectedAndReady){
            Debug.Log("Starting To Connect");
            Connect();
        }
        StartCoroutine(PingServer());
        competitorsSearchingWindow.onSerachComplete += FindRankingMatchRoom;
        SavingAndLoadingManager.Current?.SaveToCloud();
    }
    private IEnumerator CheckNetworkConnection(){
        while(Application.internetReachability == NetworkReachability.NotReachable){
            Debug.LogError("NetworkConnection Status = <color=red> Disconnected </color>");
            yield return null;
        }
        Debug.Log("NetworkConnection Status = <color=green> Connected </color>");
        WaitForSeconds waitTime = new WaitForSeconds(2f);
        while(!PhotonNetwork.IsConnectedAndReady){
            Debug.Log("Starting To Connect");
            Connect();
            yield return waitTime;
        }
    }
    private bool CanRecoverFromDisconnect(DisconnectCause cause) {
        switch (cause) {
            // the list here may be non exhaustive and is subject to review
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    private void TryRecover() {
        if (!PhotonNetwork.ReconnectAndRejoin()) {
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            if (!PhotonNetwork.Reconnect()) {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!PhotonNetwork.ConnectUsingSettings()) {
                    Debug.LogError("ConnectUsingSettings failed");
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
        }
    }
    private void Connect(){
        connectingWindow.ShowScreen();
        quickStartBtn.interactable = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Trying To Connect to Master");
        TryRecover();
    }
    public override void OnConnectedToMaster() {
        quickStartBtn.interactable  = true;
        connectingWindow.HideScreen();
        Debug.LogError("Connected To Master");
        base.OnConnectedToMaster();
    }
    public void UpdateLapCountSlider(float value){
        gameSettingsSo.currentLap = Mathf.RoundToInt(value);
        lapCountSliderValue.SetText(string.Concat("LAPS : ",value));
    }
    public void UpdateHasBots(bool value){
        gameSettingsSo.hasBots = value;
    }
    public void MakePersonalRoom() {
        isRankingMatch = false;
        AudioManager.Current.StopAudio(Sounds.SoundType.Menu_BGM);
        connectingWindow.ShowScreen("Joined a Race....",Color.white);
        quickStartBtn.interactable = false;
        if(PhotonNetwork.IsConnectedAndReady){
            float roomPlayer = maxPlayersSlider.value;
            gameSettingsSo.currentLap = Mathf.RoundToInt(lapCountSlider.value);
            Debug.LogError("Is Ranking Match " + isRankingMatch);
            Debug.LogError($"Tying To Find a Room with Laps Count of {gameSettingsSo.currentLap} at {(TrackTypes)gameSettingsSo.currentTrack.trackSaveData.tracks} Track , With Player Count of {roomPlayer}");
            Hashtable properties = new Hashtable {
                { KeysHolder.TRACK_KEY, gameSettingsSo.currentTrack.trackSaveData.tracks},
                { KeysHolder.LAP_KEY, gameSettingsSo.currentLap },
                {KeysHolder.RANKING_MATCH,isRankingMatch}
            };
            PhotonNetwork.JoinRandomRoom(properties,(byte)roomPlayer);
        }else{
            connectingWindow.ShowScreen();
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("Trying To Connect to Master");
            TryRecover();
        }
    }
    public void OpenOppnonentFindWindow(){
        competitorsSearchingWindow.StartFinding();
    }
    private void FindRankingMatchRoom(){
        AudioManager.Current?.StopAudio(Sounds.SoundType.Menu_BGM);
        isRankingMatch = true;
        Debug.LogError("Finding Random Game Rooms");
        Debug.Log("Total Rooms Counts" + PhotonNetwork.CountOfRooms);
        if(currentRoomList != null){
            Debug.LogError("Found an an Open Game Rooms");
            foreach(RoomInfo info in currentRoomList){
                RoomInfo currentInfo = info;
                if(currentInfo.PlayerCount < currentInfo.MaxPlayers){
                    PhotonNetwork.JoinRoom(currentInfo.Name);
                    break;
                }
            }
        }else{
            Debug.LogError("Not Found an an Open Game Rooms");
            RankingMatchRoom();
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("Room List Updated " + roomList.Count);
        currentRoomList = roomList;
        Debug.LogError("Current Avaialble Room Amount is " + roomList.Count);
        foreach(RoomInfo roomInfos in roomList){
            Debug.LogError("Room Info " + roomInfos.Name);
        }
        base.OnRoomListUpdate(roomList);
    }
    /* public override void OnFriendListUpdate(List<FriendInfo> friendList) {
        Debug.Log("FriendInfo List Updated");
        Debug.Log("FriendInfo Count: " + friendList.Count);
        base.OnFriendListUpdate(friendList);
    } */
    public override void OnJoinedRoom() {
        if(!PhotonNetwork.IsConnectedAndReady){
            TryRecover();
        }
        VerifyUsername();
        connectingWindow.ShowScreen("Entering the Race...",Color.yellow);
        Debug.LogError(PhotonNetwork.LocalPlayer.NickName + " Joinning A room in " + (TrackTypes)PhotonNetwork.CurrentRoom.CustomProperties[KeysHolder.TRACK_KEY] + " Track With Laps of" + (int)PhotonNetwork.CurrentRoom.CustomProperties[KeysHolder.LAP_KEY] + " Player Count of " + PhotonNetwork.CurrentRoom.PlayerCount);
        StartGame();
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        connectingWindow.ShowScreen($"<color=white> Disconnected Due to: </color> <color=yellow> [{cause}] </color>");

        quickStartBtn.interactable = false;
        Debug.LogError("Disconnected Due to: " + cause);
        if (this.CanRecoverFromDisconnect(cause)) {
            this.TryRecover();
        }else{
            if(CheckInternetConnectionRoutine != null){
                StopCoroutine(CheckInternetConnectionRoutine);
            }
            CheckInternetConnectionRoutine = StartCoroutine(CheckNetworkConnection());
        }
        base.OnDisconnected(cause);
    }
    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.LogError("Failed To Connect Random Room");
        if(isRankingMatch){
            CreateRankingMatchRoom();
        }else{
            CreateNewCustomRoom();
        }
        base.OnJoinRandomFailed(returnCode, message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError("Failed To Connect Random Room");
        base.OnJoinRoomFailed(returnCode, message);
    }

    #region Expersiance Gain Match.............................
    private void RankingMatchRoom(){
        Debug.Log("Total Rooms Counts: " + PhotonNetwork.CountOfRooms);
        if(PhotonNetwork.IsConnectedAndReady){
            connectingWindow.ShowScreen("Joining Race... ",Color.green);
            rankingMatchPlayerCount = GetMaxPlayersCount()/* KeysHolder.MAX_PLAYERS */;
            rankingMatchLapCount = GetMaxLaps() /* 1 */;
            rankingTrackType = GetTrackTypes() /* TrackTypes.TRACK1 */;
            SetTrackIcon(rankingTrackType);
            Debug.LogError($"Tying To Find Random Room with {rankingTrackType} Track = Player Count of {rankingMatchPlayerCount} = Total Laps Count {rankingMatchLapCount}");
            Hashtable roomProperties = new Hashtable {
                { KeysHolder.TRACK_KEY, rankingTrackType},
                { KeysHolder.LAP_KEY, rankingMatchLapCount},
                {KeysHolder.RANKING_MATCH,isRankingMatch},
            };
            PhotonNetwork.JoinRandomRoom(roomProperties,rankingMatchPlayerCount,MatchmakingMode.FillRoom,TypedLobby.Default,null,null);
        }else{
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("Disconnected : Trying To Reconnect to Master");
            TryRecover();
        }
    }
    private byte GetMaxPlayersCount(){
        int currentAmount = KeysHolder.MAX_PLAYERS;
        int currentLevel = gameSettingsSo.gameData.GetLevelSystem.level;
        if(currentLevel <= 10){
            currentAmount = KeysHolder.MAX_PLAYERS;
        }
        if(currentLevel <= 20 && currentLevel > 10){
            currentAmount = Random.Range(4, KeysHolder.MAX_PLAYERS);
        }
        if(currentLevel <= 80  && currentLevel > 20){
            currentAmount = Random.Range(3, KeysHolder.MAX_PLAYERS);
        }
        if(currentLevel > 80){
            currentAmount = Random.Range(1, KeysHolder.MAX_PLAYERS);
        }
        return (byte)currentAmount;
    }
    private int GetMaxLaps(){
        int laps = 1;
        int currentLevel = gameSettingsSo.gameData.GetLevelSystem.level;
        if(currentLevel <= 10){
            laps = 1;
        }
        if(currentLevel <= 20 && currentLevel > 10){
            laps = Random.Range(1, 3);
        }
        if(currentLevel <= 80  && currentLevel > 20){
            laps = Random.Range(2, KeysHolder.MAX_LAPS);
        }
        if(currentLevel > 80){
            laps = Random.Range(1, KeysHolder.MAX_LAPS);
        }
        return laps;
    }
    private void CreateRankingMatchRoom(){
        Debug.Log("Total Rooms Counts" + PhotonNetwork.CountOfRooms);
        Debug.LogError($"Creating a Room with Laps Count of {rankingMatchLapCount} at {rankingTrackType} Track , With Player Count of {rankingMatchPlayerCount}");
        Hashtable roomProperties = new Hashtable {
            { KeysHolder.TRACK_KEY, rankingTrackType},
            { KeysHolder.LAP_KEY, rankingMatchLapCount},
            {KeysHolder.RANKING_MATCH,isRankingMatch},
        };
        RoomOptions roomOptions = new RoomOptions {
            CustomRoomPropertiesForLobby = new string[] { 
                KeysHolder.TRACK_KEY, 
                KeysHolder.LAP_KEY,
                KeysHolder.RANKING_MATCH
            },
            IsVisible = true,
            IsOpen = true,
            CleanupCacheOnLeave = true,
            PlayerTtl = KeysHolder.PLAYER_TIME_TO_LIVE,
            MaxPlayers = rankingMatchPlayerCount,
            CustomRoomProperties = roomProperties
        };
        string randomRoomName = string.Concat("MRoom",Random.Range(10,2000));
        Debug.LogError("room Name " + randomRoomName);
        PhotonNetwork.CreateRoom(null,roomOptions,TypedLobby.Default);
    }
    private TrackTypes GetTrackTypes(){
        List<TrackTypes> trackTypesList = new List<TrackTypes>();
        for (int i = 0; i < trackArray.Length; i++) {
            if(!trackArray[i].trackSaveData.isLocked){
                if(!trackTypesList.Contains(trackArray[i].trackSaveData.tracks)){
                    trackTypesList.Add(trackArray[i].trackSaveData.tracks);
                }
            }
        }
        int randomTrackIndex = Random.Range(0,trackTypesList.Count);
        return trackTypesList[randomTrackIndex];
    }
    private void SetTrackIcon(TrackTypes trackTypes){
        for (int i = 0; i < trackArray.Length; i++) {
            if(trackArray[i].trackSaveData.tracks == trackTypes){
                gameSettingsSo.currentTrack = trackArray[i];
                break;
            }
        }
    }

#endregion


    private void CreateNewCustomRoom(){
        float roomPlayer = maxPlayersSlider.value;
        Debug.LogError($"Creating a Room with Laps Count of {gameSettingsSo.currentLap} at {(TrackTypes)gameSettingsSo.currentTrack.trackSaveData.tracks} Track , With Player Count of {roomPlayer}");
        gameSettingsSo.currentLap = Mathf.RoundToInt(lapCountSlider.value);
        Hashtable properties = new Hashtable {
            { KeysHolder.TRACK_KEY, gameSettingsSo.currentTrack.trackSaveData.tracks },
            { KeysHolder.LAP_KEY, gameSettingsSo.currentLap },
            {KeysHolder.RANKING_MATCH,isRankingMatch},
        };
        RoomOptions roomOptions = new RoomOptions {
            IsVisible = true,
            IsOpen = true,
            CleanupCacheOnLeave = true,
            MaxPlayers = (byte)roomPlayer,
            PlayerTtl = KeysHolder.PLAYER_TIME_TO_LIVE,
            CustomRoomProperties = properties,
            CustomRoomPropertiesForLobby = new string[] { KeysHolder.TRACK_KEY, KeysHolder.LAP_KEY,KeysHolder.RANKING_MATCH}
        };
        PhotonNetwork.CreateRoom(roomNameField.text,roomOptions,TypedLobby.Default);
    }
    public void UpdateMaxPlayerSliderView(float value){
        gameSettingsSo.maxPlayer = value;
        maxPlayersValue.SetText(string.Concat("MAX PLAYER : ",value));
    }
    private void VerifyUsername () {
        SavingAndLoadingManager.Current?.SaveToCloud();
        SavingAndLoadingManager.Current?.SaveGame();
        PhotonNetwork.LocalPlayer.NickName = gameSettingsSo.gameProfileData.username;
    }
    public void StartGame () {
        connectingWindow.ShowScreen("Entering the Races....",Color.green);
        VerifyUsername();
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.IsMessageQueueRunning = true;
            PhotonNetwork.LoadLevel(KeysHolder.TRACK_SCENE_INDEX);
        }
    }

    public void SetCurrentTrack(TrackSO tracks) {
        gameSettingsSo.currentTrack = tracks;
        RefreshTrackTypesUi();
    }
    private void RefreshTrackTypesUi(){
        for (int i = 0; i < trackSelectionUiBtns.Length; i++) {
            if(gameSettingsSo.currentTrack.trackSaveData.tracks != trackSelectionUiBtns[i].GetTracks()){
                trackSelectionUiBtns[i].ShowHideTrackBtn(true);
            }else{
                trackSelectionUiBtns[i].ShowHideTrackBtn(false);
            }
        }
    }
    
}