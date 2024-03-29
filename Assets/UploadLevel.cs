using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class UploadLevel : MonoBehaviour
{

    [SerializeField]
    PlayerStats playerStats;

    private void Awake()
    {
        playerStats.GetLevelAndXPJson();
    }
    public void SendJSONAutomatically()
    {
        string stringListAsJson = JsonUtility.ToJson(playerStats.GetLevelAndXP());
        Debug.Log(stringListAsJson);
        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Stats",stringListAsJson}
            }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("DATA sent successful"), result => Debug.Log("Data send error"));
    }
    
}
