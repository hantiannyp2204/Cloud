using PlayFab.GroupsModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;

public class ClanInfo : MonoBehaviour
{
    [SerializeField] GameObject clanInfo;
    [SerializeField] TMP_Text clanName;
    [SerializeField] TMP_Text clanDescription;
    [SerializeField] Transform requestDisplayContent;

    [SerializeField] GameObject requestGroupPrefab;
    [SerializeField] GameObject pendingDisplay;

    //To show pending if player is admin
    [SerializeField] GameObject pendingButton;
    public string GroupID;
    private void Awake()
    {
        hideClanDetails();
        pendingDisplay.SetActive(false);
    }

 
    public void ClosePendingRequests()
    {
        pendingDisplay.SetActive(false);
    }
    public void ShowPendingRequests()
    {
        pendingDisplay.SetActive(true);
        var request = new ListGroupApplicationsRequest
        {
            Group = new PlayFab.GroupsModels.EntityKey { Id = GroupID, Type = "group" }
        };

        PlayFabGroupsAPI.ListGroupApplications(request,
            result =>   
            {
                foreach (var application in result.Applications)
                {
                    GameObject playerRequest = Instantiate(requestGroupPrefab, requestDisplayContent);
                    playerRequest.GetComponent<GroupRequest>().playerID = application.Entity.Key.Id;

                    Debug.Log($"Pending Request: GroupID - {application.Group.Id}, PlayerID - {application.Entity.Key.Id}");
                }
            },
            error => Debug.LogError($"Error listing group applications: {error.ErrorMessage}")
        );
    }
    //5. Create OnEnabled and Disabled to prevent errors
    private void OnEnable()
    {
        //7. Add listener to our made function (This is basically refrencing this script to the GameEvent.cs)
        //If the parameters matches, you will have no errors

        //NOTE:If theres a null refeence in this line, it is becos the GameEvent class ran too slow, it should be pirorities before anything else run
        //First head to: Edit -> Project Settings -> Script Execution Order
        //Press on the + icon and find "GameEvent" or what ever you named the .cs
        //Drag it all the way up, to before the block "Default Time"

        GameEvent.instance.showClanDetails.AddListener(showClanDetails);

        //9. go to the GO that will trigger the funciton
        //we want showClanDetails() to run when the ClanPrefab is pressed, thus the GO is the ClanPrefab Prefab, make a .cs file if don't exist on it
    }
    private void OnDisable()
    {
        //8. Remove listener to our made function (This is basically unfrefrencing this script to the GameEvent.cs, this prevent errors, just do this)
        GameEvent.instance.showClanDetails.RemoveListener(showClanDetails);
    }

    //6. Make a somewhat "duplicated" fucntion that mirrors the fucntions that was created in the GameEvent.cs
    //It should have parameters that mimics what was needed
    //public UnityEvent <string,string> showClanDetails was made, thus we need "String String" in parameter
    void showClanDetails(string _clanName, string _clanDesription, string _groupID)
    {
        clanInfo.SetActive(true);
        clanName.text = _clanName;
        clanDescription.text = _clanDesription;

        //change the public group ID
        GroupID = _groupID;

        //check if player is in the group, else dont show the invite icon
        var request = new IsMemberRequest
        {
            Entity = new PlayFab.GroupsModels.EntityKey
            {
                Id = MyPlayFab.Instance.myPlayFabTitleID,
                Type = "title_player_account"
            },
            Group = new PlayFab.GroupsModels.EntityKey
            {
                Id = GroupID,
                Type = "group"
            }

        };

        PlayFabGroupsAPI.IsMember(request,
            result =>
            {
                Debug.Log(result.IsMember);
            },
            error => Debug.LogError($"Error listing group members: {error.GenerateErrorReport()}")
        );
    }
    public void hideClanDetails()
    {
        clanInfo.SetActive(false);
    }
}
