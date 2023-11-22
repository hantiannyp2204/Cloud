using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class ReadLevel : MonoBehaviour
{
    [SerializeField]
    PlayerStats playerStats;
    public void SendJSON()
    {
        string stringListAsJson = JsonUtility.ToJson(playerStats.ReturnClass());
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
    public void ReadJSON()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataRecieved, OnError);
    }
    void OnJSONDataRecieved(GetUserDataResult r)
    {


        if(r.Data != null &&r.Data.ContainsKey("Stats"))
        {
            Debug.Log(r.Data["Stats"].Value);
            Stats fromJson = JsonUtility.FromJson<Stats>(r.Data["Stats"].Value);
            Debug.Log(fromJson);
            playerStats.SetUI(fromJson);
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
