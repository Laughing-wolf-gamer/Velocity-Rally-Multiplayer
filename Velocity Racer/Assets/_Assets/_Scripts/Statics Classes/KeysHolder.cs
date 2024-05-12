using UnityEngine;

public static class KeysHolder {
    public const string leadBoardPublicKey = "548aaf9fd6cdbb1909c2a0de959b07deeb46978298a102e1051df2454b2c89c7";// LeaderBoard Key
    public const int MAX_PLAYER_NAME_CHAR_LENGHT = 12;
    public const int MIN_PLAYER_NAME_CHAR_LENGHT = 3;
    public const int TRACK_SCENE_INDEX = 2;
    public const int MENU_SCENE_INDEX = 1;
    public const int MAX_PLAYERS = 6;
    public const int MAX_LAPS = 7;
    public const string TRACK_KEY = "TRACKS";
    public const string LAP_KEY = "LAP";
    public const string RACE_START_TIME_KEY = "RaceStartingTime";
    public const string RANKING_MATCH = "IsRankingMatch";
    public const int PLAYER_TIME_TO_LIVE = 10;

    // Match Handler.....
    public const byte PHOTON_CODE_MAX = 200;
    public const int MAX_PACKAGE_ARRAY_LENGTH = 8;
    public const float STARTING_TIME_MAX = 5f;
    public const float FINALFINISHINGTIME = 10f;
    public const float START_SOUND_DELAY = .8f;
    public const float BOT_SPAWN_TIME = .85f;


    //TimeManager
    public const float MILI_SECONDS_TO_WAIT = 86400000;// 86400000 = 24Hrs
    public const string TIMER_SAVE_KEY = "LastTimeClicked";

    // Saving and Loading Keys.
    public const string GAMESETTINGS_SAVE_KEY = "Profile_Data_key";
    public const string GAMEDATA_SAVE_KEY = "Game_DataSaveKey";
    public const string LEVEL_DATA_SAVE_KEY = "PlayerLevelSaveKey";
    public const string CLOUD_SAVE_DATA_KEY = "cloudSaveDataKey";


    // Methords....
    public static string NormalizedTime(float timeToSort,bool includeHours = true){
        float seconds = Mathf.FloorToInt(timeToSort % 60);
        float minutes = Mathf.FloorToInt(timeToSort / 60);
        float hours = Mathf.FloorToInt(timeToSort / 3600);
        string timeing = string.Format("{0:00H}:{1:00M}:{2:00s}",hours,minutes,seconds);
        if(!includeHours){
            timeing = string.Format("{0:00M}:{1:00s}",minutes,seconds);
        }
        return timeing;
    }
    public static string GetAmountNormalized(int amount){
        if(amount >= 100000){
            int cashAmount = Mathf.RoundToInt((float)amount / 100000);
            return string.Concat(cashAmount.ToString("f1"),"M");
        }
        if(amount >= 1000){
            int cashAmount = Mathf.RoundToInt((float)amount / 1000);
            return string.Concat(cashAmount.ToString("f1"),"K");
        }
        return amount.ToString();
    }
    public static string StringShorting(string inpuName,int maxCharacterLength = 6){
        string uName = inpuName;
        char[] setName = new char[uName.Length];
        if(uName.Length > maxCharacterLength){
            for (int i = 0; i < setName.Length; i++) {
                if(i < maxCharacterLength){
                    setName[i] = uName[i];
                }else{
                    setName[i] ='.';
                }
            }
            uName = new string(setName);
        }
        return uName;
    }
    public static string RankingSuffix(int r){
        string rankSufix;
        if (r == 1){
            rankSufix = "st";
        }else if(r == 2){
            rankSufix = "nd";
        }else if(r == 3){
            rankSufix = "rd";
        }else{
            rankSufix = "th";
        }
        return rankSufix;
    }
}