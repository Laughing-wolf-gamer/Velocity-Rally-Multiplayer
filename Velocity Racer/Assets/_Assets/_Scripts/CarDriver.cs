using TMPro;
using ArcadeVP;
using Photon.Pun;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using GamerWolf.Utils;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using System.Collections;
public class CarDriver : MonoBehaviourPunCallbacks {
    [SerializeField] private GameObject confetiEffect;
    [Header("Controlls")]
    [SerializeField] private GameObject leftRightStickControlls;
    [SerializeField] private GameObject steeringControlls,GyroControlls;
    [SerializeField] private GameObject firstRanksVisual;
    [SerializeField] private Volume nosVolume;
    [SerializeField] private SpriteRenderer miniMapTrackRender;
    [SerializeField] private CinemachineScreenShakeManager screenShakeManager;
    [SerializeField] private GameObject miniMapVisual;
    [SerializeField] private float fieldOfViewChangeSpeed = .2f;
    [SerializeField] private float normalFov,nosFov;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private GameProifileData profileData;
    [SerializeField] private LocalPlayerUiManager localPlayerUiManager;
    [SerializeField] private Image playerIndicatorImage;
    [SerializeField] private TextMeshProUGUI playerNickName,rankingVisualText;
    [SerializeField] private TextMeshPro playerNameShowingOnServer;
    [SerializeField] private GameObject mainCam;
    [SerializeField] private CinemachineVirtualCamera carCam,endCamera;
    [SerializeField] private ArcadeVehicleController arcadeVehicleController;
    [SerializeField] private int totalLaps;
    [SerializeField] private float nosUseSpeed = 2f;
    [SerializeField] private float maxNosTime = 20f;
    [SerializeField] private ParticleSystem nosEffect,smokeEffect;
    [SerializeField] private CarBodyFullWorld carBodyFullWorld;
    [SerializeField] private AudioSource boosterAudioSource;
    private bool isReady;
    private bool raceCompleted;
    private Vector2 input;
    private float brake;
    private int currentLap;
    private Color playerNamePlateColor;
    private bool holdingNos;
    private float lapTimer = 0;
    private int checkPointCrossCount;
    private int totalCheckPointPerLap;
    private float currentNosTime;
    private bool nos;
    private Vector3 startPoint;
    private float currentLenseFov;
    private float velocity;
    private bool isReversing;
    private float currentForwardAmount,currentSideDriveAmount,currentBrakeAmount;
    private bool isDrifing;
    private Rigidbody rb;
    private Quaternion startRotation;
    private Transform nextCheckPoint;
    private float moveAmount;
    private int currentRank;
    private bool alreadySet;
    private Coroutine checkRoutine;
    private void Awake(){
        confetiEffect.SetActive(false);
        rb = GetComponent<Rigidbody>();
    }
    public int GetCarTypeIndex(){
        return (int)carBodyFullWorld.GetCarTypeSO().carSaveData.carTypeShop;
    }
    public void SetInitPoint(Vector3 startPoint){
        this.startPoint = new Vector3(startPoint.x,rb.position.y,startPoint.z);
        startRotation = rb.rotation;
    }
    private IEnumerator CheckForChangeRoutine(){
        while(true){
            if(MatchHandler.Current?.GetGameState() == GameState.Playing){
                if(photonView.IsMine){
                    localPlayerUiManager.ShowHideTutorialUI(gameData.GetIsFirstRace());
                    int temp = raceCompleted ? 1 : 0;// 1 is true and 0 is false.........
                    MatchHandler.Current.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber,currentLap,checkPointCrossCount,(byte)temp,(byte)0,lapTimer/* ,GetCarTypeIndex() */);// 1 for true.....
                }else{
                    if(MatchHandler.Current?.GetGameState() == GameState.Playing){
                        if(MatchHandler.Current.GetPlayerRank(PhotonNetwork.LocalPlayer.ActorNumber) <= 1){
                            photonView.RPC(nameof(ShowHideFirstRankVisual),RpcTarget.AllBuffered,true);
                        }else{
                            photonView.RPC(nameof(ShowHideFirstRankVisual),RpcTarget.AllBuffered,false);
                        }
                    }
                    photonView.RPC(nameof(ShowPlayerNameToNonLocalPlayer),RpcTarget.AllBuffered);
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }
    private void Start(){
        firstRanksVisual.SetActive(false);
        currentLenseFov = normalFov;
        isReady = true;
        localPlayerUiManager.HideWrongCheckPointIndicator();
        Debug.LogError("Player Actor Number " + PhotonNetwork.LocalPlayer.ActorNumber);
        nosEffect.gameObject.SetActive(false);
        SetTotalCheckPointPerLapCounts(MatchHandler.Current.GetTotalCheckPointCount());
        endCamera.gameObject.SetActive(false);
        miniMapTrackRender.gameObject.SetActive(true);
        nosVolume.weight = 0f;
        if(!photonView.IsMine){
            miniMapVisual.SetActive(false);
            miniMapTrackRender.color = Color.green;
            carCam.transform.SetParent(transform);
            localPlayerUiManager.gameObject.SetActive(false);
            carCam.gameObject.SetActive(false);
            mainCam.SetActive(false);
            photonView.RPC(nameof(ShowPlayerNameToNonLocalPlayer),RpcTarget.All);
        }else{
            localPlayerUiManager.ShowHideTutorialUI(gameData.GetIsFirstRace());
            arcadeVehicleController.SetCarMovementData (
                carBodyFullWorld.GetCarTypeSO().GetCarData.maxSpeed,
                carBodyFullWorld.GetCarTypeSO().GetCarData.accelaration,
                carBodyFullWorld.GetCarTypeSO().GetCarData.turn
            );
            if(carCam.TryGetComponent(out CarCameraFollow carCameraFollow)){
                carCameraFollow.canZoom = carBodyFullWorld.GetCarTypeSO().GetCarData.canZoomCamera;
            }
            checkRoutine = StartCoroutine(CheckForChangeRoutine());
            miniMapVisual.SetActive(true);
            currentNosTime = maxNosTime / 2f;
            mainCam.SetActive(true);
            localPlayerUiManager.gameObject.SetActive(true);
            carCam.gameObject.SetActive(true);
            photonView.RPC(nameof(SetTotalLapsRpc),RpcTarget.All,MatchHandler.Current.GetTotalLapCount());
            checkPointCrossCount = 0;
            SetInitPoint(transform.position);
            playerNameShowingOnServer.gameObject.SetActive(false);
            arcadeVehicleController.StartCalculatingSpeed();
        }
        
    }
    private void OnApplicationQuit(){
        if(photonView.IsMine){
            carCam.transform.SetParent(transform);
        }
    }
    private void OnDestroy(){
        if(photonView.IsMine){
            if(checkRoutine != null){
                StopCoroutine(checkRoutine);
            }
            carCam.transform.SetParent(transform);
        }
    }
    
    private void Update(){
        if(photonView.IsMine){
            switch(MatchHandler.Current.GetGameState()){
                case GameState.Starting:
                    photonView.RPC(nameof(SetNickNameRpc),RpcTarget.AllBuffered,MatchHandler.Current.GetPlayerName(PhotonNetwork.LocalPlayer.ActorNumber));
                    mainCam.SetActive(true);
                    carCam.gameObject.SetActive(true);
                    localPlayerUiManager.gameObject.SetActive(false);
                    moveAmount = 0f;
                    currentForwardAmount = 0f;
                    carCam.transform.SetParent(this.transform);
                    rb.position = new Vector3(startPoint.x,rb.position.y,startPoint.z);
                    rb.rotation = startRotation;
                    smokeEffect.gameObject.SetActive(true);
                    nos = false;
                    input = Vector2.zero;
                    currentBrakeAmount = 1;
                    SetDrivingValues();
                    return;
                case GameState.WaitingForPlayer:
                    localPlayerUiManager.gameObject.SetActive(false);
                    currentForwardAmount = 0f;
                    moveAmount = 0;
                break;
                case GameState.Playing:

                    if(raceCompleted){
                        if(!alreadySet){
                            MatchHandler.Current.OnLocalPlayerFinishRace(PhotonNetwork.LocalPlayer.ActorNumber);
                            alreadySet = true;
                        }
                        carCam.transform.SetParent(this.transform);
                        localPlayerUiManager.ShowTrackMiniMap(false);
                        endCamera.gameObject.SetActive(true);
                        carCam.gameObject.SetActive(false);
                        smokeEffect.gameObject.SetActive(true);
                        nos = false;
                        currentLenseFov = normalFov;
                        input = Vector2.zero;
                        brake = 1;
                        SetDrivingValues();
                        localPlayerUiManager.gameObject.SetActive(false);
                        return;
                    }else{
                        lapTimer += Time.deltaTime;
                    }
                    localPlayerUiManager.gameObject.SetActive(true);
                    CheckControllsType();
                    switch(gameData.GetControllType()){
                        case ControllsSelectionManager.ControllType.Left_Right_Brake:
                        case ControllsSelectionManager.ControllType.SteeringWheel_Controlls:
                        case ControllsSelectionManager.ControllType.Accelrometer_Controlls:
                            moveAmount = isReversing ? -1f : 1f;
                        break;
                    }
                    if(moveAmount < 0){
                        currentForwardAmount = -1f;
                    }else{
                        currentForwardAmount = 1f;
                    }
                    carCam.transform.SetParent(null);
                    localPlayerUiManager.ShowTrackMiniMap(true);
                    smokeEffect.gameObject.SetActive(false);
                    if(nos){
                        nosVolume.weight = 1 - currentNosTime / maxNosTime;
                    }else{
                        nosVolume.weight = 0f;
                    }
                    if(nos){
                        screenShakeManager.Shake();
                        currentLenseFov = nosFov;
                    }else{
                        currentLenseFov = normalFov;
                    }
                    if(gameData.GetOnPc()){
                        isReversing = Input.GetAxisRaw("Vertical") < 0 ? true: false;
                        currentSideDriveAmount = Input.GetAxisRaw("Horizontal");
                        float threshHoldforDrift = 12f;
                        currentBrakeAmount = currentForwardAmount < 0 && velocity >= threshHoldforDrift && arcadeVehicleController.GetRigidBodyVelocity().z > 0f ? 1f: 0f;
                        isDrifing = currentBrakeAmount == 1f ? true: false;
                        if(Input.GetKeyDown(KeyCode.LeftShift)){
                            if(currentNosTime > 0){
                                AudioManager.Current?.PlayAudioAtPos(boosterAudioSource,Sounds.SoundType.Booster);
                            }
                        }
                        nos = Input.GetKey(KeyCode.LeftShift) && currentNosTime > 0f && currentForwardAmount>= 0.1f;
                        arcadeVehicleController.SetNOs(nos);
                    }
                    SetDrivingValues();
                break;
                case GameState.Ending:
                    confetiEffect.SetActive(true);
                    localPlayerUiManager.gameObject.SetActive(false);
                    currentForwardAmount = 0f;
                    moveAmount = 0;
                    SetDrivingValues();
                break;
            }            
        }/* else{
            if(MatchHandler.Current.GetGameState() == GameState.Playing){
                if(MatchHandler.Current.GetPlayerRank(PhotonNetwork.LocalPlayer.ActorNumber) <= 1){
                    photonView.RPC(nameof(ShowHideFirstRankVisual),RpcTarget.AllBuffered,true);
                }else{
                    photonView.RPC(nameof(ShowHideFirstRankVisual),RpcTarget.AllBuffered,false);
                }
            }
            photonView.RPC(nameof(ShowPlayerNameToNonLocalPlayer),RpcTarget.AllBuffered);
            currentRank = MatchHandler.Current.GetPlayerRank(PhotonNetwork.LocalPlayer.ActorNumber);
        } */
    }
    private void CheckControllsType(){
        leftRightStickControlls.SetActive(false);
        steeringControlls.SetActive(false);
        GyroControlls.SetActive(false);
        switch(gameData.GetControllType()){
            case ControllsSelectionManager.ControllType.Left_Right_Brake:
                leftRightStickControlls.SetActive(true);
            break;
            case ControllsSelectionManager.ControllType.SteeringWheel_Controlls:
                steeringControlls.SetActive(true);
            break;
            case ControllsSelectionManager.ControllType.Accelrometer_Controlls:
                GyroControlls.SetActive(true);
            break;
        }
    }

    public void CompleteFirstRaceTutorial(){
        gameData.ToggleFirstRace(false);
        localPlayerUiManager.ShowHideTutorialUI(gameData.GetIsFirstRace());
    }

    [PunRPC]
    private void ShowHideFirstRankVisual(bool show){
        firstRanksVisual.SetActive(show);
    }
    [PunRPC]
    private void ShowPlayerNameToNonLocalPlayer(){
        playerNameShowingOnServer.SetText(GetUserName());
    }
    private void LateUpdate(){
        if(photonView.IsMine){
            if(carCam.m_Lens.FieldOfView != currentLenseFov){
                carCam.m_Lens.FieldOfView = Mathf.Lerp(carCam.m_Lens.FieldOfView,currentLenseFov,fieldOfViewChangeSpeed * Time.deltaTime);
            }
            float smootingSpeed = 5f;
            if(input.x != currentSideDriveAmount){
                input.x = Mathf.Lerp(input.x,currentSideDriveAmount,smootingSpeed * Time.deltaTime);
            }
            if(input.y != currentForwardAmount){
                input.y = Mathf.Lerp(input.y,currentForwardAmount,smootingSpeed * Time.deltaTime);
            }
            if(brake != currentBrakeAmount){
                brake = Mathf.Lerp(brake,currentBrakeAmount,smootingSpeed * Time.deltaTime);
            }
        }
    }
    public void IncreaseNosTime(float incrementAmount){
        if(photonView.IsMine){
            photonView.RPC(nameof(IncrementNOsTimeRpc),RpcTarget.AllBuffered,incrementAmount);
        }
    }
    
    private void SetDrivingValues(){
        nosEffect.gameObject.SetActive(nos);
        if(nos){
            currentNosTime -= Time.deltaTime * nosUseSpeed;
        }
        localPlayerUiManager.SetTime(lapTimer);
        if(arcadeVehicleController.GetVelocity(gameData.GetSpeedType()) <= 0f){
            velocity = 0f;
        }else{
            velocity = arcadeVehicleController.GetVelocity(gameData.GetSpeedType());
        }
        localPlayerUiManager.SetCurrentSpeed(velocity);
        arcadeVehicleController.SetInput(input,brake);
        localPlayerUiManager.SetNos(currentNosTime / maxNosTime);
        localPlayerUiManager.SetPositions(MatchHandler.Current.GetPlayerRank(PhotonNetwork.LocalPlayer.ActorNumber),MatchHandler.Current.GetTotalPlayerCount());
    }
    public void SetUIInput(bool reverse,float sideWays,bool nosPressed) {
        if(gameData.GetOnPc()) return;
        this.isReversing = reverse;
        this.currentSideDriveAmount = sideWays;
        float threshHoldforDrift = 12f;
        this.currentBrakeAmount = currentForwardAmount < 0 && velocity >= threshHoldforDrift && arcadeVehicleController.GetRigidBodyVelocity().z > 0f ? 1f: 0f;
        this.nos = nosPressed && currentNosTime > 0f;
        if(this.nos){
            if(currentForwardAmount <= 0f){
                currentForwardAmount = 1f;
            }
        }
    }
    public void PlayNosPressSound(){
        if(currentNosTime > 0){
            AudioManager.Current?.PlayAudioAtPos(boosterAudioSource,Sounds.SoundType.Booster);
        }
    }
    public void SetSteeringInput(float steering){
        if(gameData.GetOnPc()) return;
        if(gameData.GetControllType() == ControllsSelectionManager.ControllType.SteeringWheel_Controlls){
            this.currentSideDriveAmount = steering;
        }
    }
    public void AccelerometerInputs(float steering){
        if(gameData.GetOnPc()) return;
        if(gameData.GetControllType() == ControllsSelectionManager.ControllType.Accelrometer_Controlls){
            this.currentSideDriveAmount = steering;
        }
    }

    public bool IsReady() {
        return isReady;
    }
    public string GetUserName(){
        return MatchHandler.Current.GetPlayerName(/* photonView.OwnerActorNr */PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void TrySync(){
        if(photonView.IsMine){
            photonView.RPC(nameof(SetRandomColorRPC),RpcTarget.AllBuffered);
            photonView.RPC(nameof(SyncRpc),RpcTarget.AllBuffered,gameSettings.gameProfileData.username);
        }
    }
    public void OnCorrectCheckPointCrossed(){
        if(photonView.IsMine){
            // localPlayerUiManager.ShowCheckPointIndicator();
            photonView.RPC(nameof(IncreaseCheckPointCountRpc),RpcTarget.AllBuffered);
        }
    }
    public void OnWrongCheckPointCrossed(){
        if(photonView.IsMine){
            localPlayerUiManager.ShowWrongCheckPointIndicator();
        }
    }
    public void IncreaseLapsCount(){
        if(photonView.IsMine){
            photonView.RPC(nameof(IncreaseLapsRpc),RpcTarget.AllBuffered);
        }
    }
    public void SetTotalCheckPointPerLapCounts(int totalChecksPointPerLap){
        this.totalCheckPointPerLap = totalChecksPointPerLap;
    }
    public int GetCurrentLapsCount(){
        return currentLap;
    }

    #region  RPC Functions..............
    [PunRPC]
    private void IncrementNOsTimeRpc(float incrementAmount){
        currentNosTime += incrementAmount;
        if(currentNosTime >= maxNosTime){
            currentNosTime = maxNosTime;
        }
    }
    [PunRPC]
    private void IncreaseCheckPointCountRpc(){
        checkPointCrossCount = (checkPointCrossCount + 1) % totalCheckPointPerLap;
        if(checkPointCrossCount == 0){
            localPlayerUiManager.ShowLapCompletedIndicator();
            if((totalCheckPointPerLap - currentLap) == 1){
                localPlayerUiManager.ShowFinalLapIndicator();
            }
        }
        int temp = raceCompleted ? 1 : 0;// 1 is true and 0 is false.........
        Debug.LogError("Check Point Count Crossed By : " + GetUserName() + " is " + checkPointCrossCount);
        // MatchHandler.Current.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber/* photonView.OwnerActorNr */,currentLap,checkPointCrossCount,(byte)temp,(byte)0,lapTimer/* ,GetCarTypeIndex() */);// 1 for true.....
    }
    [PunRPC]
    private void SetRandomColorRPC(){
        playerNamePlateColor = Random.ColorHSV();
    }

    [PunRPC]
	private void IncreaseLapsRpc(){
        currentLap ++;
        // Debug.Log("lap increased in " + GetUserName());
        if(currentLap >= totalLaps){
            currentLap = totalLaps;
            isReady = false;
            raceCompleted = true;
        }else{
            raceCompleted = false;
            // MatchHandler.Current.ChangeStat_S(photonView.OwnerActorNr,0,(byte) currentLap,(byte)checkPointCrossCount,1);// 1 for true.....
        }
        // MatchHandler.Current.ChangeStat_S(photonView.OwnerActorNr,0,(byte) currentLap,(byte)checkPointCrossCount,0);// 0 for False.....
        localPlayerUiManager.SetLapCounts(currentLap,totalLaps);
        int tempFlag = raceCompleted ? 1 : 0;// 1 is true and 0 is false.........
        // MatchHandler.Current.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber/* photonView.OwnerActorNr */,currentLap,checkPointCrossCount,(byte) tempFlag,(byte)0,lapTimer/* ,GetCarTypeIndex() */);// 1 for true.....
    }
    [PunRPC]
    private void SyncRpc(string userNameNew){
        Debug.LogError("Synced Data On " + GetUserName());
        playerIndicatorImage.color = playerNamePlateColor;
        miniMapTrackRender.color = playerNamePlateColor;
        profileData = new GameProifileData(userNameNew,GetCarTypeIndex());
    }
    [PunRPC]
    private void SetTotalLapsRpc(int maxLaps){
        raceCompleted = false;
        currentLap = 0;
        totalLaps = maxLaps;
        localPlayerUiManager.SetLapCounts(currentLap,totalLaps);
    }
    [PunRPC]
    private void SetNickNameRpc(string userName){
        playerNickName.SetText(userName.ToUpper());
        rankingVisualText.SetText(userName.ToUpper());
    }

    #endregion

}