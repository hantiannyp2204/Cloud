using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ToggleScoreboard : MonoBehaviour
{
    bool isLeaderboard;
    [SerializeField]
    private TMP_Text scoreTxt, rankTxt;
    [SerializeField]
    private GameObject leaderboard, scoreboard;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private PlayerValue playerValue;
    [SerializeField]
    LeaderboardManager leaderboardManager;
    int currentRank;
    // Start is called before the first frame update
    void Start()
    {

        getCurrentPlayerRank();
        //set score
        scoreTxt.text = "Your Score: " + gameController.getScore();
   
        //update highscore if more
        if(gameController.getScore() > playerValue.currentHighscore)
        {
            scoreTxt.text += "\nNEW HIGHSCORE!!!";
            leaderboardManager.updateLeaderboardScore(gameController.getScore());
        }
        //set rank
        rankTxt.text = "currently ranked at positon " + (currentRank+1).ToString();
        isLeaderboard = true;
        checkToggle(false);
    }
    void OnError(PlayFabError error)
    {
        UpdateMessage("Error " + error.GenerateErrorReport());
    }
    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
    }
    void getCurrentPlayerRank()
    {
        var lbreq = new GetLeaderboardRequest
        {
        };

        PlayFabClientAPI.GetLeaderboard(lbreq, OnRankGetCurrentPlayer, OnError);
    }
    void OnRankGetCurrentPlayer(GetLeaderboardResult r)
    {
        //set currentHightScore value;
        foreach (var item in r.Leaderboard)
        {
            if (item.DisplayName == playerValue.currentUsername)
            {
                currentRank = item.Position;
            }
        }
    }
    void checkToggle(bool toggle)
    {
        if(toggle == true)
        {
            leaderboard.SetActive(true);
            scoreboard.SetActive(false);
        }
        else
        {
            scoreboard.SetActive(true);
            leaderboard.SetActive(false);
        }
    }
    public void togleLeaderboard()
    {
        if (isLeaderboard == true)
        {
            checkToggle(true);
            isLeaderboard = false;
         
        }
        else
        {
            checkToggle(false);
            isLeaderboard = true;
        
        }
    }
}
