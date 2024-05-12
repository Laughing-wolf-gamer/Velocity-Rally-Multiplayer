using TMPro;
using System;
using ArcadeVP;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class CarDriverAI : MonoBehaviourPunCallbacks {
    [SerializeField] private float frontCheckLength,upOffset;
    [SerializeField] private LayerMask checkMask;
    [SerializeField] private GameObject firstRanksVisual;
    [SerializeField] private SpriteRenderer miniMapTrackRender;
    [SerializeField] private GameProifileData profileData;
    
    [SerializeField] private float turnSpeed = 6f;
    [SerializeField] private Image playerIndicatorImage;
    [SerializeField] private TextMeshProUGUI playerNickName;
    [SerializeField] private ArcadeVehicleController arcadeVehicleController;
    [SerializeField] private int totalLaps;
    [SerializeField] private float maxNosTime;
    [SerializeField] private ParticleSystem nosEffect,smokeEffect;
    [SerializeField] private int aiActorNumber = 3;
    [SerializeField] private List<CheckPointSingle> checkPointSinglesList;
    [SerializeField] private bool isReady = false;
    [SerializeField] private TextMeshPro playerNameShowingOnServer;
    [SerializeField,Range(30,60f)] private float turnThreshold = 50f;
    [SerializeField] private float blockSpeedThreshold = 3f,maxBlockCheckTime = 3f;


    private bool raceCompleted;
    private Vector2 input;
    private float brake;
    private int currentLap;
    private Color randomColor;
    private bool holdingNos;
    private float lapTimer = 0;
    private int checkPointCrossCount;
    private int totalCheckPointPerLap;
    private float currentNosTime;
    private bool nos;
    private Vector3 startPoint;
    private Transform currentTargetPoint;
    private float randomNosTimeing;
    private float forward = 0f;
    private float turnAmount = 0f;
    private Rigidbody rb;
    private float currentBrakeAmount;
    [SerializeField] private CarBodyFullWorld carBodyFullWorld;
    [SerializeField] private float checkBlockTimer = 4f;
    [SerializeField,TextArea(5,5)] private string logString;
    private int currentRank;
    private Coroutine checkRoutine;
    private bool isBlocked;
    private void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    
    private void Start(){
        if(photonView.ViewID < 1){
            PhotonNetwork.AllocateViewID(photonView);
        }
        miniMapTrackRender.color = Color.green;
        miniMapTrackRender.gameObject.SetActive(true);
        lapTimer = 0f;
        playerNameShowingOnServer.gameObject.SetActive(true);
        playerNickName.gameObject.SetActive(true);
        photonView.RPC(nameof(ShowPlayerNameToNonLocalPlayer),RpcTarget.All);
        arcadeVehicleController.StartCalculatingSpeed();
        checkRoutine = StartCoroutine(RefreshDataRoutine());
    }
    private void OnDestroy(){
        if(checkRoutine != null){
            StopCoroutine(checkRoutine);
        }
    }
    private void OnDrawGizmos(){
        Gizmos.DrawRay(transform.position + Vector3.up *upOffset,transform.forward * frontCheckLength);
    }
    public void SetInitPoint(Vector3 startPoint){
        this.startPoint = new Vector3(startPoint.x,transform.position.y,startPoint.z);
    }
    public void SetActorNumber(int actorNumber){
        isReady = true;
        this.aiActorNumber = actorNumber;
        Debug.LogError("Bot Actor Number " + aiActorNumber + "Is ready " + isReady);
        nosEffect.gameObject.SetActive(false);
        SetTotalCheckPointPerLapCounts(MatchHandler.Current.GetTotalCheckPointCount());
        currentNosTime = maxNosTime;
        checkPointCrossCount = 0;
        SetInitPoint(transform.position);
        currentTargetPoint = checkPointSinglesList[checkPointCrossCount].transform;
    }
    public void SetCheckPointList(List<CheckPointSingle> checkPointSinglesList){
        this.checkPointSinglesList = checkPointSinglesList;
    }
    private IEnumerator RefreshDataRoutine(){
        WaitForSeconds waitTime = new WaitForSeconds(1.2f);
        while(true){
            if(photonView.IsMine){
                if(MatchHandler.Current?.GetGameState() == GameState.Playing){
                    photonView.RPC(nameof(SetNickNameRpc),RpcTarget.AllBuffered,GetUserName());
                    int tempFlag = raceCompleted ? 1 : 0;// 1 is true and 0 is false.........
                    MatchHandler.Current?.ChangeStat_S(aiActorNumber,currentLap,checkPointCrossCount,(byte) tempFlag,(byte)1/* 1 == Bot , 0 == Real Player */,lapTimer);// 1 for true.....
                    Debug.Log( "Bot Actor Number " + aiActorNumber + " Data Refreshed");
                    if(MatchHandler.Current.GetPlayerRank(aiActorNumber) == 1){
                        photonView.RPC(nameof(ShowHideFirstRankVisual),RpcTarget.AllBuffered,true);
                    }else{
                        photonView.RPC(nameof(ShowHideFirstRankVisual),RpcTarget.AllBuffered,false);
                    }
                    photonView.RPC(nameof(ShowPlayerNameToNonLocalPlayer),RpcTarget.All);
                }
            }
            yield return waitTime;
        }
    }
    
    private void Update(){
        if(photonView.IsMine){
            switch(MatchHandler.Current.GetGameState()){
                case GameState.WaitingForPlayer:
                    input = Vector2.zero;
                    forward = 0f;
                    turnAmount = 0f;
                    nos = false;
                    currentBrakeAmount = 1;// brake.
                    SetInputAndMoveDirection(nos);
                    SetDrivingValues();
                break;
                case GameState.Starting:
                    smokeEffect.gameObject.SetActive(true);
                    nos = false;
                    input = Vector2.zero;
                    currentBrakeAmount = 1;// full Brake.
                    SetDrivingValues();
                break;
                case GameState.Playing:
                    smokeEffect.gameObject.SetActive(false);
                    if(!raceCompleted){
                        lapTimer += Time.deltaTime;
                        if(currentTargetPoint == null){
                            forward = 0f;
                            turnAmount = 0f;
                        }else{
                            if(arcadeVehicleController.GetVelocity() <= blockSpeedThreshold) {
                                if(Physics.Raycast(transform.position + Vector3.up * upOffset,transform.forward,out RaycastHit hit,frontCheckLength,checkMask) && !isBlocked){
                                    logString = string.Concat("Hitting " + hit.transform.name);
                                    logString = string.Concat("Is Blocked"," Speed: " ,arcadeVehicleController.GetVelocity());
                                    isBlocked = true;
                                }
                            }
                            if(isBlocked){
                                if(checkBlockTimer <= 0){
                                    isBlocked = false;
                                    checkBlockTimer = maxBlockCheckTime;
                                }else{
                                    Vector3 targetCheckPointPos = new Vector3(currentTargetPoint.parent.position.x,transform.position.y,currentTargetPoint.parent.position.z);
                                    float angleToDir = Vector3.SignedAngle(transform.forward,targetCheckPointPos,Vector3.up);
                                    if(angleToDir > turnThreshold){//& Turning.....
                                        turnAmount = -1f;
                                    } else if(angleToDir < -turnThreshold){
                                        turnAmount = 1f;
                                    }else {
                                        turnAmount = 0f;
                                    }
                                    forward = -1f;
                                    checkBlockTimer -= Time.deltaTime;
                                }
                            }else{
                                logString = string.Concat("Free Moving"," Speed: " ,arcadeVehicleController.GetVelocity());
                                OnUnBlockedMovement();
                            }
                        }
                    }else{
                        float reachedTargetDist = 4f;
                        Vector3 targetPoint = (new Vector3(startPoint.x,transform.position.y,startPoint.z) - transform.position).normalized;
                        float distanceToTarget = Vector3.Distance(transform.position,targetPoint);
                        float dot = Vector3.Dot(transform.forward,targetPoint);
                        if(distanceToTarget > reachedTargetDist){
                            if(dot > 0f){
                                forward = 1f;
                            }else{
                                float reverseDistance = 1f;
                                if(distanceToTarget > reverseDistance){
                                    currentBrakeAmount = 1f;//? Slow Down Forward.........
                                }else{
                                    currentBrakeAmount = 0f;
                                }
                                forward = -1f;//! Reverse....
                            }
                            float angleToDir = Vector3.SignedAngle(transform.forward,targetPoint,Vector3.up);
                            if(angleToDir > turnThreshold){//& Turning.....
                                turnAmount = 1f;
                            } else if(angleToDir < -turnThreshold){
                                turnAmount = -1f;
                            }else {
                                turnAmount = 0f;
                            }
                        }else{
                            // Reached Target CheckPoint.........
                            if(arcadeVehicleController.GetVelocity() > 1f){
                                currentBrakeAmount = 1f;//! full Brake.....
                            }else{
                                currentBrakeAmount = 0f;//? zero Brake......
                            }
                            forward = 0f;
                            turnAmount = 0f;
                        }
                    }
                    SetInputAndMoveDirection(nos);
                    SetDrivingValues();
                break;
                case GameState.Ending:
                    smokeEffect.gameObject.SetActive(true);
                    nos = false;
                    input = Vector2.zero;
                    currentBrakeAmount = 1;
                    SetDrivingValues();
                break;
            }
        }
    }
    private void OnUnBlockedMovement(){
        float reachedTargetDist = .1f;
        Vector3 targetCheckPointPos = new Vector3(currentTargetPoint.parent.position.x,transform.position.y,currentTargetPoint.parent.position.z);
        checkBlockTimer = maxBlockCheckTime;
        Debug.DrawLine(transform.position,targetCheckPointPos,Color.cyan,.4f);
        float distanceToTarget = Vector3.Distance(transform.position,targetCheckPointPos);
        Vector3 dirToMove = (targetCheckPointPos - transform.position).normalized;
        if(distanceToTarget >= reachedTargetDist){
            float dot = Vector3.Dot(transform.forward,dirToMove);
            currentBrakeAmount = 0f;
            if(dot > 0f) { //* Check Forward and Reverse........
                forward = 1f;//^ Move Forward
                CheckNos();
            } else {
                float reverseDistance = 5f;
                if(distanceToTarget > reverseDistance){
                    // forward = -1f;//! Reverse....
                    forward = 1f;//? Slow Down Forward.........
                }else{
                    forward = -1f;
                }
            }
            float angleToDir = Vector3.SignedAngle(transform.forward,dirToMove,Vector3.up);
            if(angleToDir > turnThreshold){//& Turning.....
                turnAmount = 1f;
            } else if(angleToDir < -turnThreshold){
                turnAmount = -1f;
            }else {
                turnAmount = 0f;
            }
            
        }else{
            // Reached Target CheckPoint.........
            if(arcadeVehicleController.GetVelocity() >= 50f){
                currentBrakeAmount = 1f;//! full Brake.....
            }else{
                currentBrakeAmount  = 0f;//? zero Brake......
            }
            forward = 0f;
            turnAmount = 0f;
        }
    }
    [PunRPC]
    private void ShowHideFirstRankVisual(bool show){
        firstRanksVisual.SetActive(show);
    }
    private void LateUpdate(){
        if(photonView.IsMine){
            float smootingSpeed = 5f;
            if(input.x != turnAmount){
                input.x = Mathf.Lerp(input.x,turnAmount,smootingSpeed * Time.deltaTime);
            }
            if(input.y != forward){
                input.y = Mathf.Lerp(input.y,forward,smootingSpeed * Time.deltaTime);
            }
            if(brake != currentBrakeAmount){
                brake = Mathf.Lerp(brake,currentBrakeAmount, smootingSpeed * Time.deltaTime);
            }
        }
    }
    private void CheckNos(){
        if(currentNosTime >= 0f){
            randomNosTimeing += Time.deltaTime;
            if(randomNosTimeing >= 3f){
                randomNosTimeing = 3f;
                int randNosIndex = Random.Range(1,5);
                if(randNosIndex >= 2){
                    nos = true;
                }else{
                    nos = false;
                }
            }
        }else{
            nos = false;
        }
    }
    public void IncreaseNosTime(float incrementAmount){
        if(photonView.IsMine){
            photonView.RPC(nameof(IncrementNitroTimeRpc),RpcTarget.AllBuffered,incrementAmount);
        }
    }
    
    private void SetDrivingValues(){
        photonView.RPC(nameof(OnOffNos),RpcTarget.AllBuffered,nos);
        if(nos){
            currentNosTime -= Time.deltaTime;
        }
        arcadeVehicleController.SetInput(input,brake);
    }
    [PunRPC]
    private void OnOffNos(bool onOff){
        nosEffect.gameObject.SetActive(onOff);
    }
    public void SetInputAndMoveDirection(bool nosPressed) {
        this.nos = nosPressed && currentNosTime > 0f;
        if(this.nos){
            if(forward <= 0f){
                forward = 1f;
            }
        }
    }
    public bool IsReady() {
        return isReady;
    }
    public string GetUserName(){
        return MatchHandler.Current.GetPlayerName(aiActorNumber);
    }

    public void TrySync(){
        if(photonView.IsMine){
            photonView.RPC(nameof(SyncRpc),RpcTarget.AllBuffered);
        }
    }
    public void OnCorrectCheckPointCrossed(){
        if(!photonView.IsMine) return;
        checkPointCrossCount = (checkPointCrossCount + 1) % totalCheckPointPerLap;
        int temp = raceCompleted ? 1 : 0;// 1 is true and 0 is false.........
        currentTargetPoint = checkPointSinglesList[checkPointCrossCount].transform;
        // randomPoint = Random.Range(-4f,4f);
        // MatchHandler.Current.ChangeStat_S(aiActorNumber,currentLap,checkPointCrossCount,(byte) temp,(byte)1,lapTimer);// 1 for true.....
    }
    public void IncreaseLapsCount(){
        if(!photonView.IsMine) return;
        currentLap ++;
        Debug.Log("lap increased in " + GetUserName());
        if(currentLap >= totalLaps){
            currentLap = totalLaps;
            isReady = false;
            raceCompleted = true;
        }else{
            raceCompleted = false;
        }
        int tempFlag = raceCompleted ? 1 : 0;// 1 is true and 0 is false.........
        // MatchHandler.Current.ChangeStat_S(aiActorNumber,currentLap,checkPointCrossCount,(byte) tempFlag,(byte)1,lapTimer);// 1 for true.....
    }
    public void SetTotalCheckPointPerLapCounts(int totalChecksPointPerLap){
        this.totalCheckPointPerLap = totalChecksPointPerLap;
        Debug.Log("Total CheckPoints Per Laps is  " + this.totalCheckPointPerLap);
    }
    public int GetCurrentLapsCount(){
        return currentLap;
    }


    public int GetCarTypeIndex(){
        return (int)carBodyFullWorld.GetCarTypeSO().carSaveData.carTypeShop;
    }

#region  RPC Functions..............

    [PunRPC]
    private void IncrementNitroTimeRpc(float incrementAmount){
        currentNosTime += incrementAmount;
        if(currentNosTime >= maxNosTime){
            currentNosTime = maxNosTime;
        }
    }

    [PunRPC]
    private void SyncRpc(){
        playerIndicatorImage.color = Color.red;
        profileData = new GameProifileData(GetUserName(),(int)carBodyFullWorld.GetCarTypeSO().carSaveData.carTypeShop);
    }
    public void SetTotalLapsRpc(int maxLaps){
        raceCompleted = false;
        currentLap = 0;
        totalLaps = maxLaps;
    }
    [PunRPC]
    private void SetNickNameRpc(string userName){
        playerNickName.SetText(userName.ToUpper());
    }
    [PunRPC]
    private void ShowPlayerNameToNonLocalPlayer(){
        playerNameShowingOnServer.gameObject.SetActive(true);
        playerNickName.gameObject.SetActive(true);
        playerNickName.SetText(GetUserName());
        playerNameShowingOnServer.SetText(GetUserName());
    }

#endregion


}