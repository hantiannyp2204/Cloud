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
    public string playerName;
    int currentLevel;
    int currentXP;
    int currenMaxXP;
    private void Awake()
    {
        //gets user username
        var request = new GetPlayerProfileRequest
        {
        };

        PlayFabClientAPI.GetPlayerProfile(request, result => { playerName = result.PlayerProfile.DisplayName; },OnError) ;

        //gets data
        GetLevelAndXPJson();
    }
    public Stats CreateFreshLevelXPProfile()
    {
        currenMaxXP = 10;
        return new Stats(1, 0, currenMaxXP);
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


    public void SendJSONAutomatically()
    {
        string stringListAsJson = JsonUtility.ToJson(PublishLvlXPMaxXPToDatabase());
        Debug.Log(stringListAsJson);
        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
           {
               {"Stats",stringListAsJson}
           }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("DATA sent successful"), OnError);
    }
    public void GetLevelAndXPJson()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataRecieved, OnError);
    }

    void OnJSONDataRecieved(GetUserDataResult r)
    {
        if (r.Data != null && r.Data.ContainsKey("Stats"))
        {

            Debug.Log(r.Data["Stats"].Value);
            Stats fromJson = JsonUtility.FromJson<Stats>(r.Data["Stats"].Value);
            SetLevelAndXP(fromJson.level, fromJson.xp, fromJson.maxXP);

        }
        //if its blank, make it
        else
        {
            string stringListAsJson = JsonUtility.ToJson(CreateFreshLevelXPProfile());
            Debug.Log(stringListAsJson);
            var req = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
           {
               {"Stats",stringListAsJson}
           }
            };
            //gets it again

            PlayFabClientAPI.UpdateUserData(req, result => GetLevelAndXPJson(), OnError);
        }
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error " + error.GenerateErrorReport());
    }

    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
    }
}
