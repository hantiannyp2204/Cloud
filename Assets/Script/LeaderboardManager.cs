using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.VersionControl;
using TMPro;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text Title;
    public static LeaderboardManager instance { get; private set; }
    [SerializeField]
    TextMeshProUGUI leaderboard;

    string currentUserID;

    bool leaderBoardIsLocal = false;
    // Start is called before the first frame update
    void Start()
    {
        GetPlayerProfileRequest request = new GetPlayerProfileRequest { };
        PlayFabClientAPI.GetPlayerProfile(request, GetPlayerPlayfabID, result=>Debug.Log("Get user data error"));

        Title.text = "Leaderboard (Global)";
        GetLeaderboard();
       
    }
    public void toggleLocalLeaderboard()
    {
        //set leaderboard to local
        if(leaderBoardIsLocal == false)
        {
            Title.text = "Leaderboard (Local)";
            GetLeaderboardAroundPlayer();
            leaderBoardIsLocal= !leaderBoardIsLocal;
        }
        else
        {
            Title.text = "Leaderboard (Global)";
            GetLeaderboard();
            leaderBoardIsLocal= !leaderBoardIsLocal;
        }
    }
    void GetPlayerPlayfabID(GetPlayerProfileResult r)
    {
        currentUserID = r.PlayerProfile.PlayerId;
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
    private void GetLeaderboardAroundPlayer()
    {
        // Create and configure the request
        var request = new GetLeaderboardAroundCharacterRequest
        {
            StatisticName = "highscore",
            CharacterId= currentUserID,
            MaxResultsCount = 11 // 5 above + 1 (current player) + 5 below
        };

        // Send the request
        PlayFabClientAPI.GetLeaderboardAroundCharacter(request, OnLeaderboardCharacterReceived, OnError);
    }

    private void OnLeaderboardCharacterReceived(GetLeaderboardAroundCharacterResult result)
    {
        var leaderboardResult = result.Leaderboard;
        int playerIndex = -1;
        string LeaderboardStr = "";
        // Find the player's index in the leaderboard
        for (int i = 0; i < leaderboardResult.Count; i++)
        {
            if (leaderboardResult[i].CharacterId == currentUserID)
            {
                playerIndex = i;
                break;
            }
        }

        // Display the leaderboard entries around the player
        if (playerIndex != -1)
        {
            int startIndex = Mathf.Max(0, playerIndex - 5);
            int endIndex = Mathf.Min(leaderboardResult.Count, playerIndex + 6);

            for (int i = startIndex; i < endIndex; i++)
            {
                var entry = leaderboardResult[i];
                string oneraw = "Rank " + (entry.Position + 1) + ": " + entry.DisplayName + " | " + entry.StatValue + "\n";
                LeaderboardStr += oneraw;
            }
           leaderboard.text = LeaderboardStr;
        }
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
