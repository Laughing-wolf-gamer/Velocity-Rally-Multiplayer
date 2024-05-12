using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Configs/Daily Reward", fileName = "Reward")]
public class DailyRewardSO : ScriptableObject {
    public enum RewardType{
        Coins,
        Cash,
    }
    [Serializable]
    public class Rewards{
        public RewardType rewardType;
        public int rewardAmount;
        [TextArea(10,10)]
        public string discription;
        
    }
    public Rewards[] rewardArrays;
    public int dayNumber;
    public void SetDayNumber(int dayNum){
        dayNumber = dayNum;
    }
    
}