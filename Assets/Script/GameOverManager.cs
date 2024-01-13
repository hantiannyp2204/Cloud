using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        int goldEarned;
        bool canLevelUp = true;
        while(canLevelUp == true)
        {
            if (newXP >= playerStats.GetLevelAndXP().maxXP)
            {
                newXP -= playerStats.GetLevelAndXP().maxXP;
                newLevel++;
                //give gold when leveling up
                goldEarned = (int)Mathf.Pow(10, newLevel);
                AddCurrency(goldEarned);
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
        if (gameController.getScore() > playerValue.currentHighscore && playerStats.playerName != "Guest")
        {
            scoreTxt.text += "\nNEW HIGHSCORE!!!";
            leaderboardManager.updateLeaderboardScore(finalScore);
            Invoke("UpdatePlayerRank", 1);

        }
        //dont update scre
        else if(playerStats.playerName == "Guest")
        {
            rankTxt.text = "Rank not avaliable for being guest, score not uploaded";
        }
        else
        {
            //set rank
            UpdatePlayerRankTxt();
        }

    }
    private void AddCurrency(int amount)
    {
        // Specify the request details
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GD",
            Amount = amount
        };

        // Send the request to add currency to the player
        PlayFabClientAPI.AddUserVirtualCurrency(request, OnCurrencyAdded, OnError);
    }


    private void OnCurrencyAdded(ModifyUserVirtualCurrencyResult result)
{
    Debug.Log("Currency added successfully. New balance: " + result.Balance);
}
void UpdatePlayerRank()
    {
        getCurrentPlayerRank();
        Invoke("UpdatePlayerRankTxt", 0.5f);
    }
    void UpdatePlayerRankTxt()
    {
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
                currentRank = item.Position;
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
