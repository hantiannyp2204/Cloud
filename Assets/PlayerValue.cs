using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.PackageManager;
using UnityEngine.SocialPlatforms.Impl;
using PlayFab.MultiplayerModels;
using TMPro;

public class PlayerValue : MonoBehaviour
{
    //score values
    public string currentUsername;
    public int currentHighscore, currentRank;

    public TMP_InputField sumbitXP = null;
    
    // Start is called before the first frame update
    void Start()
    {
        GetUserAccountInfo();
        getCurrentPlayerScore();
    }

    public void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"XP",sumbitXP.text.ToString()},
        }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void GetUserData(string myPlayFabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabId,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("XP")) Debug.Log("No XP");
            else
            {

                Debug.Log("XP: " + result.Data["XP"].Value + "");

            }

        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
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
