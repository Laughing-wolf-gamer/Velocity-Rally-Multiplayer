[System.Serializable]
public class GamePlayerInfo {
    public GameProifileData gameProifile;
    public int actorId;
    public bool isBot;
    public int currentLap;
    public bool reachedFinishedLine;
    public int currentCheckPointCount;
    public float gameFinishedTime;
    public GamePlayerInfo (GameProifileData gp, int actor, int currentLaps,int currentCheckPointCount,bool reachedFinished,bool isBot,float gameFinishedTime) {
        this.gameProifile = gp;
        this.actorId = actor;
        this.currentLap = currentLaps;
        this.currentCheckPointCount = currentCheckPointCount;
        this.reachedFinishedLine = reachedFinished;
        this.isBot = isBot;
        this.gameFinishedTime = gameFinishedTime;
    }
}