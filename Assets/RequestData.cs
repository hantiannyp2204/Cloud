using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestData : MonoBehaviour
{
    public string RequesteeID;

    public string friendName;

    public void AcceptFriend()
    {
        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "acceptFriendRequest",
            FunctionParameter = new
            {
                senderID = MyPlayFab.Instance.myPlayFabMasterID,
                reciverID = RequesteeID
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, result => Debug.Log(MyPlayFab.Instance.myPlayFabMasterID + " accepted friend request to " + RequesteeID), result => Debug.Log("Some error in code dahh"));
        Destroy(gameObject);
    }
    public void DeclineFriend()
    {
        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "declineFrinedRequest",
            FunctionParameter = new
            {
                senderID = MyPlayFab.Instance.myPlayFabMasterID,
                reciverID = RequesteeID
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, result => Debug.Log(MyPlayFab.Instance.myPlayFabMasterID + " accepted friend request to " + RequesteeID), result => Debug.Log("Some error in code dahh"));
        Destroy(gameObject);
    }


    public void SendGiftRequest()
    {
        GameEvent.instance.showGiftDetails.Invoke(friendName);
    }
}
