using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.VersionControl;
using TMPro;
using Unity.VisualScripting;

public class leaderboardManager : MonoBehaviour
{
    public static leaderboardManager instance { get; private set; }
    [SerializeField]
    TextMeshProUGUI leaderboard;
    // Start is called before the first frame update
    void Start()
    {
        GetLeaderboard();
       
    }
    public void GetLeaderboard()
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = "highscore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        string LeaderboardStr = "";
        foreach (var item in r.Leaderboard)
        {
            string oneraw = "Rank " + ((int)item.Position + 1) +": "+ item.DisplayName + " | " + item.StatValue + "\n";
            LeaderboardStr += oneraw;//combine all display into one string
            
        }
        leaderboard.text = LeaderboardStr;
    }
    void OnError(PlayFabError error)
    {
        UpdateMessage("Error " + error.GenerateErrorReport());
    }
    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
        leaderboard.text = newMessage;
    }
    public void updateLeaderboardScore(int newScore)
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="highscore",
                    Value = newScore
                }
            }
        };
        UpdateMessage("Submitting score:" + newScore);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnleaderboardUpdate, OnError);
    }
    void OnleaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        UpdateMessage("Successful leaderboard sent:" + r.ToString());
    }
}
