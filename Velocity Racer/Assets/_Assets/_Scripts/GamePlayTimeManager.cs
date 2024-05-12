using UnityEngine;
public class GamePlayTimeManager : MonoBehaviour{
    [SerializeField] private GameDataSO gameData;
    [SerializeField,Tooltip("Testing Purpose Only")] private float timeElapsedSpeed = 20f;
    private float seconds,hours,minutes;
 
    private float totalMiliSeconds;
    private bool timerActive;
    private void Awake(){
        timerActive = false;
    }
    [ContextMenu("Start Time")]
    public void StartTimer(){
        totalMiliSeconds = gameData.GetGamePlayedTime();
        timerActive = true;
    }
    [ContextMenu("Stop Time")]
    public void StopTimer(){
        timerActive = false;
        gameData.IncreaseGamePlayed(totalMiliSeconds);
    }
    private void Update() {
        if(!timerActive) return;
        totalMiliSeconds += Time.deltaTime * timeElapsedSpeed;
        seconds = Mathf.FloorToInt(totalMiliSeconds % 60);
        minutes = Mathf.FloorToInt(totalMiliSeconds / 60);
        hours = Mathf.FloorToInt(totalMiliSeconds / 3600);
    }
}