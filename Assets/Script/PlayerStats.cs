using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using System.Security.Cryptography;
using PlayFab.ClientModels;
using UnityEditor.PackageManager;
using UnityEngine.UIElements;

[System.Serializable]
public class Stats
{
    public int level;
    public int xp;
    public int maxXP;
    public Stats(int _level, int _xp, int _maxXP)
    {
        level = _level;
        xp = _xp;
        maxXP = _maxXP;
    }
}
public class PlayerStats : MonoBehaviour
{
    public string playerName=null;
    int currentLevel;
    int currentXP;
    int currenMaxXP;
    private void Awake()
    {
        GetUserAccountInfo();
    }
    public void GetUserAccountInfo()
    {
        var request = new GetPlayerProfileRequest
        {
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnGetUserAccountInfoSuccess, OnError);
        
    }
    public Stats CreateFreshLevelXPProfile()
    {
        currenMaxXP = 10;
        return new Stats(1, 0, currenMaxXP);
    }
    void OnGetUserAccountInfoSuccess(GetPlayerProfileResult result)
    {
        playerName = result.PlayerProfile.DisplayName;
    }
    public void SetLevelAndXP(int _currenLevel, int _currentXP, int _currentMaxXP)
    {
        currentLevel = _currenLevel;
        currentXP = _currentXP;
        currenMaxXP = _currentMaxXP;
    }
    public Stats PublishLvlXPMaxXPToDatabase()
    {
        return new Stats(currentLevel, currentXP, currenMaxXP);
    }
    public Stats GetLevelAndXP()
    {
        return new Stats(currentLevel, currentXP, currenMaxXP);
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
