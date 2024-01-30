using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using System;
using System.Linq;

public class FriendManager : MonoBehaviour
{
    [SerializeField] GameObject FriendPrefab,PendingPrefab,RequestPrefab, displayListContent;
    [SerializeField] TextMeshProUGUI leaderboarddisplay;
    [SerializeField] TMP_InputField tgtFriend;
    List<FriendInfo> _friends = null;
    enum FriendIdType { PlayFabId, Username, Email, DisplayName };
    public enum ListType { Friends,Pending,Request };
    string myPlayFabID;
    private void Start()
    {
        myPlayFabID = MyPlayFab.Instance.myPlayFabMasterID;
    }
    public void GoToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
    //Display Friend Code
    void DisplayFriends(List<FriendInfo> friendsCache, int listType)
    {
        //clean all prefab if located
        // Iterate through each child and destroy it
        for (int i = displayListContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = displayListContent.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        friendsCache.ForEach(f => {
            switch (listType)
            {
                case 0:
                    if (f.Tags.Contains("friend"))
                    {
                        GameObject requestPrefab = Instantiate(FriendPrefab);
                        requestPrefab.transform.parent = displayListContent.transform;
                        //setting the Name placeholder to name of frined
                        requestPrefab.transform.Find("Name").GetComponent<TMP_Text>().text = f.TitleDisplayName;
                        requestPrefab.GetComponent<RequestData>().RequesteeID = f.FriendPlayFabId;
                    }
                    break;
                case 1:
                    if (f.Tags.Contains("requestee"))
                    {
                        GameObject requestPrefab = Instantiate(PendingPrefab);
                        requestPrefab.transform.parent = displayListContent.transform;
                        //setting the Name placeholder to name of frined
                        requestPrefab.transform.Find("Name").GetComponent<TMP_Text>().text = f.TitleDisplayName;
                        requestPrefab.GetComponent<RequestData>().RequesteeID = f.FriendPlayFabId;
                    }
                    break;
                case 2:
                    if (f.Tags.Contains("requester"))
                    {
                        GameObject requestPrefab = Instantiate(RequestPrefab);
                        requestPrefab.transform.parent = displayListContent.transform;
                        //setting the Name placeholder to name of frined
                        requestPrefab.transform.Find("Name").GetComponent<TMP_Text>().text = f.TitleDisplayName;
                        requestPrefab.GetComponent<RequestData>().RequesteeID = f.FriendPlayFabId;

                    }
                    break;
            }

        });
    }

    //Get Friend Code
    public void GetFriends(int listType)
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            // ExternalPlatformFriends = false,
            // XboxToken = null
        }, result => {
            _friends = result.Friends;
            DisplayFriends(_friends, listType); // triggers your UI
        }, DisplayPlayFabError);
    }

    //Add Friend Code
    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, DisplayPlayFabError);
    }
    public void OnAddFriend()
    {
        //to add friend based on display name
        //AddFriend(FriendIdType.DisplayName, tgtFriend.text);

        //getting friend's playfab ID
        string friendPlayID = null;
        var requestFriendID = new GetAccountInfoRequest{TitleDisplayName = tgtFriend.text };
        PlayFabClientAPI.GetAccountInfo(requestFriendID,result=> {
            friendPlayID = result.AccountInfo.PlayFabId;
            var GetTitleIDReq = new GetAccountInfoRequest {PlayFabId = result.AccountInfo.PlayFabId };
            PlayFabClientAPI.GetAccountInfo(GetTitleIDReq, result => Debug.Log(result.AccountInfo.TitleInfo.TitlePlayerAccount.Id), result => Debug.Log(result));
            
            Debug.Log("friend's id is: " + result.AccountInfo.PlayFabId);
            if (myPlayFabID != null && friendPlayID != null)
            {
                sendFriendRequest(myPlayFabID, friendPlayID);
            }
            else
            {
                Debug.Log("Can't get own ID dah");
            }
        },result=>Debug.Log("error somewhere dah"));

    }
    //runs the cloud script
    void sendFriendRequest(string myPlayFabID, string friendPlayID)
    {
        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "sendFriendRequest",
            FunctionParameter = new
            {
                senderID = myPlayFabID,
                reciverID = friendPlayID
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request,result=> Debug.Log(myPlayFabID + " sent friend request to "+ friendPlayID), result => Debug.Log("Some error in code dahh"));
    }
    //Remove Friend Code
    void RemoveFriend(string friendName)
    { //to investigat
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            // ExternalPlatformFriends = false,
            // XboxToken = null
        }, result => {
            _friends = result.Friends;
            RemoveFriendWithName(_friends, friendName); // triggers your UI
        }, DisplayPlayFabError);
    }
    void RemoveFriendWithName(List<FriendInfo> friendsCache, string friendName)
    {
        friendsCache.ForEach(f => {
            if (friendName == f.TitleDisplayName)
            {
                RemoveFriendFromFriendList(f.FriendPlayFabId);
            }
        });
    }
    //Friend Leaderboard Code
    public void OnGetFriendLB()
    {
        List<string> friendID = new();
        PlayFabClientAPI.GetFriendLeaderboard(
        new GetFriendLeaderboardRequest { StatisticName = "highscore", MaxResultsCount = 10 },
        r => {
            //get all friends with "Friend" tags
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
            {
                // ExternalPlatformFriends = false,
                // XboxToken = null
            }, 
            result =>
            {
                //This array list stores all friend's name with tags "Friend"
                
                _friends = result.Friends;
                _friends.ForEach(f => { 
                    if(f.Tags.Contains("friend"))
                    {
                        Debug.Log("plus one");
                        friendID.Add(f.FriendPlayFabId);
                    }

                    //only print out frineds with same name as frinedArray
                    Debug.Log(friendID.Count);
                    leaderboarddisplay.text = "Friends LB\n";
                    foreach (var leaderboardFriend in r.Leaderboard)
                    {
                        for (int x = 0; x < friendID.Count; x++)
                        {
                            if (leaderboardFriend.PlayFabId == friendID[x])
                            {
                                string onerow = leaderboardFriend.Position + " | " + leaderboardFriend.DisplayName + " | " + leaderboardFriend.StatValue + "\n";
                                Debug.Log("Tag: " + leaderboardFriend.Profile.Tags);
                                leaderboarddisplay.text += onerow;
                            }
                        }

                    }
                });

            }
            , result => Debug.Log("error in code dah")
            );

            
        }, DisplayPlayFabError);
    }


    void RemoveFriendFromFriendList(string pfid)
    {
        var req = new RemoveFriendRequest
        {
            FriendPlayFabId = pfid
        };
        PlayFabClientAPI.RemoveFriend(req
        , result => {
            Debug.Log("unfriend!");
        }, DisplayPlayFabError);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("Menu"); //change this to your main scene pls
    }

    void DisplayPlayFabError(PlayFabError error) { Debug.Log(error.GenerateErrorReport()); }
    void DisplayError(string error) { Debug.LogError(error); }

    

}
