using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.PackageManager;
using UnityEngine.SocialPlatforms.Impl;
using PlayFab.MultiplayerModels;

public class PlayerValue : MonoBehaviour
{
    //score values
    public string currentUsername;
    public int currentHighscore, currentRank;
    
    // Start is called before the first frame update
    void Start()
    {
        GetUserAccountInfo();
        getCurrentPlayerScore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetUserAccountInfo()
    {
        var request = new GetPlayerProfileRequest
        {
           
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnGetUserAccountInfoSuccess, OnError);
    }

    void OnGetUserAccountInfoSuccess(GetPlayerProfileResult result)
    {
        // set current username
        currentUsername = result.PlayerProfile.DisplayName;
    }
    void getCurrentPlayerScore()
    {
        var lbreq = new GetPlayerStatisticsRequest
        {
            
        };

        PlayFabClientAPI.GetPlayerStatistics(lbreq, OnScoreGetCurrentPlayer, OnError);
    }
    void OnScoreGetCurrentPlayer(GetPlayerStatisticsResult r)
    {
        //set currentHightScore value;
        foreach (var item in r.Statistics)
        {
            if(item.StatisticName == "highscore")
            {
                currentHighscore = item.Value;
            }
        }
    }
    void OnError(PlayFabError error)
    {
        UpdateMessage("Error " + error.GenerateErrorReport());
    }
    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
    }
}
