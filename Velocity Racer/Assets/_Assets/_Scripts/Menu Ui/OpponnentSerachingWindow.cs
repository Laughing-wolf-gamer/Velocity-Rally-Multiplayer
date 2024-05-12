using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class OpponnentSerachingWindow : MonoBehaviour {
    private const string AnimatorKey = "Start Find";
    private const string AnimationRandomizeMovementKey = "MagMovementNum";
    private const int MaxMovementCount = 4;

    [SerializeField] private TextMeshProUGUI timeInQueue;
    [SerializeField] private Animator finderAnimator;
    private int randomFindingPattern;
    private bool startFind;
    private float currentSerachTime;
    private float maxSerachingTime;
    public Action onSerachComplete;
    private bool foundout;
    public void StartFinding(){
        gameObject.SetActive(true);
        currentSerachTime = 0f;
        maxSerachingTime = Random.Range(3f,10f);
        startFind = true;
        foundout = false;
        randomFindingPattern = Random.Range(0,MaxMovementCount);
        finderAnimator.SetInteger(AnimationRandomizeMovementKey,randomFindingPattern);
        finderAnimator.SetBool(AnimatorKey,startFind);

    }
    public void StopFinding(){
        startFind = false;
        currentSerachTime = 0f;
        maxSerachingTime = Random.Range(4f,6f);
        finderAnimator.SetBool(AnimatorKey,startFind);
        gameObject.SetActive(false);
    }


    private void Update(){
        if(startFind){
            currentSerachTime += Time.deltaTime;
            timeInQueue.SetText(string.Concat("TIME IN QUEUE : ",currentSerachTime.ToString("f2"),"s"));
            if(currentSerachTime >= maxSerachingTime){
                if(!foundout){
                    onSerachComplete?.Invoke();
                    foundout = true;
                }
            }
        }
    }
}