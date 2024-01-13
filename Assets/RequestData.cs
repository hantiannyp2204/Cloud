using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestData : MonoBehaviour
{
    public string RequesteeID;

    string myPlayFabID;
    private void Start()
    {
        GetUserAccountInfo();
    }
    void GetUserAccountInfo()
    {
        var request = new GetPlayerProfileRequest
        {
        };

        PlayFabClientAPI.GetPlayerProfile(request, result => { Debug.Log(myPlayFabID); myPlayFabID = result.PlayerProfile.PlayerId; Debug.Log(myPlayFabID); }, Errorresult => { Debug.Log(Errorresult); });

    }
    public void AcceptFriend()
    {
        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "acceptFriendRequest",
            FunctionParameter = new
            {
                senderID = myPlayFabID,
                reciverID = RequesteeID
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, result => Debug.Log(myPlayFabID + " accepted friend request to " + RequesteeID), result => Debug.Log("Some error in code dahh"));
        Destroy(gameObject);
    }
    public void DeclineFriend()
    {
        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "declineFrinedRequest",
            FunctionParameter = new
            {
                senderID = myPlayFabID,
                reciverID = RequesteeID
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, result => Debug.Log(myPlayFabID + " accepted friend request to " + RequesteeID), result => Debug.Log("Some error in code dahh"));
        Destroy(gameObject);
    }
}
