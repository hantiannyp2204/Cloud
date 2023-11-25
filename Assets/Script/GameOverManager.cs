using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameOverManager : MonoBehaviour
{
    bool isLeaderboard;
    [SerializeField]
    private TMP_Text scoreTxt, rankTxt, xpEarntTxt;
    [SerializeField]
    private GameObject leaderboard, scoreboard;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private PlayerValue playerValue;
    [SerializeField]
    LeaderboardManager leaderboardManager;

    [SerializeField]
    PlayerStats playerStats;
    [SerializeField]
    UploadLevel levelReadingData;

    int currentRank;
    int xpEarnt;
    int currentXP;
    int currentMaxXP;
    // Start is called before the first frame update
    void Start()
    {

        getCurrentPlayerRank();
        Invoke("setText", 0.5f);
        isLeaderboard = true;
        checkToggle(false);
        currentXP = playerStats.GetLevelAndXP().xp;
        currentMaxXP = playerStats.GetLevelAndXP().maxXP;
    }
    void setText()
    {
        int finalScore = gameController.getScore();
        //set score
        scoreTxt.text = "Your Score: " + finalScore;
        //set xp
        xpEarnt = finalScore / 10;

        xpEarntTxt.text = xpEarnt + " xp earned";
        //check if he level up
        int newXP = playerStats.GetLevelAndXP().xp + xpEarnt;
        int newLevel = playerStats.GetLevelAndXP().level;
        bool canLevelUp = true;
        while(canLevelUp == true)
        {
            if (newXP >= playerStats.GetLevelAndXP().maxXP)
            {
                newXP -= playerStats.GetLevelAndXP().maxXP;
                newLevel++;
                canLevelUp = true;
            }
            else
            {
                canLevelUp = false;
            }
        }
        
        //add new xp into player
        playerStats.SetLevelAndXP(newLevel,newXP,10*newLevel);
        //publish change to database
        levelReadingData.SendJSONAutomatically();

        //update highscore if more
        if (gameController.getScore() > playerValue.currentHighscore)
        {
            scoreTxt.text += "\nNEW HIGHSCORE!!!";
            leaderboardManager.updateLeaderboardScore(finalScore);
           
            Invoke("UpdatePlayerRank", 1);
        }
        else
        {
            //set rank
            UpdatePlayerRankTxt();
        }

    }
    void UpdatePlayerRank()
    {
        getCurrentPlayerRank();
        Invoke("UpdatePlayerRankTxt", 0.5f);
    }
    void UpdatePlayerRankTxt()
    {
        rankTxt.text = "currently ranked at positon " + currentRank.ToString();
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
            StatisticName = "highscore",
            MaxResultsCount = 100
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
                currentRank = item.Position + 1;
                Debug.Log(item.Position);
            }
        }
    }
    void checkToggle(bool toggle)
    {
        leaderboardManager.GetLeaderboard();
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
