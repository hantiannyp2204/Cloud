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
    ReadLevel levelReadingData;

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
        //set score
        scoreTxt.text = "Your Score: " + gameController.getScore();
        //set xp
        xpEarnt = gameController.getScore() / 10;
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
            leaderboardManager.updateLeaderboardScore(gameController.getScore());
            getCurrentPlayerRank();
        }
        //set rank
        rankTxt.text = "currently ranked at positon " + (currentRank + 1).ToString();
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
            StatisticName = "highscore"
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
                Debug.Log(item.Position);
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
