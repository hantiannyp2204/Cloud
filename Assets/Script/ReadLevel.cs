using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class ReadLevel : MonoBehaviour
{
    [SerializeField]
    PlayerStatsUI playerStatsUI = null;
    [SerializeField]
    PlayerStats playerStats = null;
    private void Start()
    {
        GetLevelAndXP();
    }
    public void SendJSONWithButton()
    {
        string stringListAsJson = JsonUtility.ToJson(playerStatsUI.SendNewLevelAndXP());
        Debug.Log(stringListAsJson);
        var req = new UpdateUserDataRequest
        {
           Data=new Dictionary<string, string>
           {
               {"Stats",stringListAsJson}
           }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("DATA sent successful"), OnError);
    }
    public void SendJSONAutomatically()
    {
        string stringListAsJson = JsonUtility.ToJson(playerStats.PublishLvlXPMaxXPToDatabase());
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
    public void GetLevelAndXP()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataRecieved, OnError);
    }
    void OnJSONDataRecieved(GetUserDataResult r)
    {
        if(r.Data != null &&r.Data.ContainsKey("Stats"))
        {

            Debug.Log(r.Data["Stats"].Value);
            Stats fromJson = JsonUtility.FromJson<Stats>(r.Data["Stats"].Value);
            playerStatsUI.SetUI(fromJson);
            playerStats.SetLevelAndXP(fromJson.level,fromJson.xp,fromJson.maxXP);

        }
        //if its blank, make it
        else
        {
            string stringListAsJson = JsonUtility.ToJson(playerStats.CreateFreshLevelXPProfile());
            Debug.Log(stringListAsJson);
            var req = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
           {
               {"Stats",stringListAsJson}
           }
            };
            //gets it again

            PlayFabClientAPI.UpdateUserData(req, result => GetLevelAndXP(), OnError);
        }
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error " + error.GenerateErrorReport());
    }
}
[System.Serializable]
public class JSListWrapper<T>
{
    public List<T> list;
    public JSListWrapper(List<T> list) => this.list = list;
}
