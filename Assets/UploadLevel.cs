using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

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
        string stringListAsJson = JsonUtility.ToJson(playerStats.PublishLvlXPMaxXPToDatabase());
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
