using TMPro;
using System;
using UnityEngine;
using Baracuda.Monitoring;
public class TimeManager : MonoBehaviour {
    public event EventHandler OnClick,OnTimeOver;
    public event EventHandler<OnTimeTickingArgs> OnTimeTicking;
    public class OnTimeTickingArgs : EventArgs{
        public int hours,minit,seconds;
    }
    [SerializeField] private bool canClick;
    [SerializeField] private TextMeshProUGUI[] timeTextArray;
    private long lastTimeClicked;
    private int hours,minit,seconds;
    private void Awake() {
        Monitor.StartMonitoring(this);
        // Or use this extension method:
        this.StartMonitoring();
        Debug.Log(KeysHolder.TIMER_SAVE_KEY + lastTimeClicked);
        
        if(SaveSystemManager.HasSaveFile(KeysHolder.TIMER_SAVE_KEY)){
            lastTimeClicked = /* long.Parse(PlayerPrefs.GetString(TIMER_SAVE_KEY)); */SaveSystemManager.Load<long>(KeysHolder.TIMER_SAVE_KEY);
        }else{
            lastTimeClicked = 0;
            // PlayerPrefs.SetString(TIMER_SAVE_KEY, lastTimeClicked.ToString());
            SaveSystemManager.Save<long>(lastTimeClicked,KeysHolder.TIMER_SAVE_KEY);
        }
        if (!Ready()){
            canClick = false;
        }
    }
    private void OnDestroy() {
        Monitor.StopMonitoring(this);
        // Or use this extension method:
        this.StopMonitoring();
    }
    

    private void Update(){
        if (Ready()){
            OnTimeOver?.Invoke(this,EventArgs.Empty);
            canClick = true;
            SetTimerText("Ready!");
            return;
        }
        long diff = ((long)DateTime.Now.Ticks - lastTimeClicked);
        long m = diff / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (float)(KeysHolder.MILI_SECONDS_TO_WAIT - m) / 1000.0f;

        // string r = "";
        
        //HOURS
        hours = (Mathf.RoundToInt(secondsLeft) / 3600);
        secondsLeft -= (Mathf.RoundToInt(secondsLeft) / 3600) * 3600;
        //MINUTES
        minit = (Mathf.RoundToInt(secondsLeft) / 60);
        //SECONDS
        seconds = Mathf.RoundToInt((secondsLeft % 60));
        OnTimeTicking?.Invoke(this,new OnTimeTickingArgs {hours = hours,minit = minit,seconds = seconds});
        SetTimerText( "Reward Available in "+ string.Format("{0:00H} : {1:00M} : {2:00S}",hours,minit,seconds));
    }
    private void SetTimerText(string time){
        foreach(TextMeshProUGUI texts in timeTextArray){
            texts.SetText(time);
        }

    }
    public float GetTimeLeftToUnlock(int dayIndex){
        return hours * (dayIndex + 1);
    }



    public void Click() {
        lastTimeClicked = DateTime.Now.Ticks;
        SaveSystemManager.Save<long>(lastTimeClicked,KeysHolder.TIMER_SAVE_KEY);
        canClick = false;
        OnClick?.Invoke(this,EventArgs.Empty);
    }
    public bool Ready(){
        long diff = (long)DateTime.Now.Ticks - lastTimeClicked;
        long m = diff / TimeSpan.TicksPerMillisecond;

        float secondsLeft = (float)(KeysHolder.MILI_SECONDS_TO_WAIT - m) / 1000.0f;

        if (secondsLeft < 0){
            //DO SOMETHING WHEN TIMER IS FINISHED
            return true;
        }

        return false;
    }

}

