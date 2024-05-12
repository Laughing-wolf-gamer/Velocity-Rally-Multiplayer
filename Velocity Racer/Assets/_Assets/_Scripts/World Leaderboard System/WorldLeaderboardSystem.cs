using System;
using Dan.Main;
using UnityEngine;
using System.Collections.Generic;
public class WorldLeaderboardSystem : MonoBehaviour {
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private RectTransform worldLeaderBoardContantHolder;
    [SerializeField] private WorldLeaderboardCard newWorldLeaderBoardCard;
    [SerializeField] private List<WorldLeaderboardCard> worldLeaderboardCardsList;
    [SerializeField] private ScrollSelect scrollSelect;
    private bool isLeaderBoardOnline;
    private bool isLeaderBoardOpen;
    private void Start(){
        isLeaderBoardOpen = false;
        LeaderboardCreator.Ping((isOnline) =>{
            this.isLeaderBoardOnline = isOnline;
            if(isOnline){
                RefreshWorldLeaderBoard();
            }
        });
    }
    public void RefreshLeaderBoardBtnCall(){
        isLeaderBoardOpen = true;
        if(isLeaderBoardOnline){
            RefreshWorldLeaderBoard();
        }
    }
    public void ClosingLeaderBoardBtnCall(){
        isLeaderBoardOpen = false;
    }
    private void RefreshWorldLeaderBoard(){
        SetLeaderBoardEntry();
        LeaderboardCreator.GetLeaderboard(KeysHolder.leadBoardPublicKey,false,RecheckLeaderBoardUI);
    }

    private void RecheckLeaderBoardUI(Dan.Models.Entry[] receivingData) {
        if(isLeaderBoardOpen){
            Debug.Log("Refreshing LeaderBoard");
            if(receivingData.Length > worldLeaderboardCardsList.Count){
                int deff = receivingData.Length - worldLeaderboardCardsList.Count;
                for (int d = 0; d < deff; d++) {
                    WorldLeaderboardCard card = Instantiate(newWorldLeaderBoardCard,worldLeaderBoardContantHolder);
                    card.HideCard();
                    if(!worldLeaderboardCardsList.Contains(card)){
                        worldLeaderboardCardsList.Add(card);
                    }
                }
            }

            // Total Numbers of Entry in LeaderBoard.............
            int totalEntry = Mathf.Min(worldLeaderboardCardsList.Count,receivingData.Length);
            for (int i = 0; i < worldLeaderboardCardsList.Count; i++) {
                if(i >= totalEntry){
                    worldLeaderboardCardsList[i].HideCard();
                }else{
                    worldLeaderboardCardsList[i].ShowCard();
                }
            }
            // Set the User Name for Each Players in LeaderBoard.....
            for (int i = 0; i < totalEntry; i++) {
                if(receivingData[i].Username == gameSettings.gameProfileData.username){
                    scrollSelect.SetSelected(worldLeaderboardCardsList[i].gameObject);
                }
                worldLeaderboardCardsList[i].SetCardData(i+1,receivingData[i].Username,receivingData[i].Score);
            }

            // is The Leaderboard is Onpen Then Scroll the Local Player UserNames.......
            for (int i = 0; i < totalEntry; i++) {
                if(receivingData[i].Extra == gameSettings.gameData.GetUniqueIdNumber()){
                    scrollSelect.SetSelected(worldLeaderboardCardsList[i].gameObject);
                    break;
                }
            }
        }
    }

    public void SetLeaderBoardEntry(){
        if(isLeaderBoardOpen){
            int scoreAveraging = Mathf.RoundToInt((gameData.GetTotalMatchWon() + gameData.GetTotalMedalsWon() + gameData.GetTotalRacesJoined() + gameData.GetTotalGoldTropyWon()) / 4);
            LeaderboardCreator.UploadNewEntry(KeysHolder.leadBoardPublicKey,gameSettings.gameProfileData.username,scoreAveraging,gameSettings.gameData.GetUniqueIdNumber(),(receivingData)=>{ RefreshWorldLeaderBoard(); });
        }
    }
    public void UpgradeEntryUserName() {
        if(isLeaderBoardOnline){
            LeaderboardCreator.UpdateEntryUsername(KeysHolder.leadBoardPublicKey,gameSettings.gameProfileData.username,(receivingData)=>{ RefreshWorldLeaderBoard(); });
        }
    }
    public void CheckNewNameIsAlreadyPresent(string currentUserName,Action<bool> AfterCheckComplete){
        bool isPresent = false;
        LeaderboardCreator.GetLeaderboard(KeysHolder.leadBoardPublicKey,(receivingData) =>{
            for (int i = 0; i < receivingData.Length; i++) {
                if(receivingData[i].Username == currentUserName){
                    isPresent = true;
                    AfterCheckComplete?.Invoke(isPresent);
                    break;
                }
            }
        });
        AfterCheckComplete?.Invoke(isPresent);
    }

}