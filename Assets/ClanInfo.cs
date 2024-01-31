using PlayFab.GroupsModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;

public class ClanInfo : MonoBehaviour
{
    [SerializeField] GameObject clanInfo;
    [SerializeField] TMP_Text clanName;
    [SerializeField] TMP_Text clanDescription;
    [SerializeField] Transform requestDisplayContent;
    [SerializeField] Transform groupMemberDisplayContent;

    [SerializeField] GameObject requestGroupPrefab;
    [SerializeField] GameObject groupMemberPrefab;

    [SerializeField] GameObject pendingDisplay;

    //To show pending if player is admin and is in the group
    [SerializeField] GameObject PendingInvitesButton;
    [SerializeField] GameObject settingsBtn;
    [SerializeField] GameObject inviteUI;
    [SerializeField] GameObject settingsUI;

    //Joining the group status
    [SerializeField] GameObject JoinButton;
    [SerializeField] GameObject LeaveButton;
    [SerializeField] GameObject PendingJoinButton;

    //check any grp inv
    [SerializeField] GameObject GroupInvUI;

    public string GroupName;
    public string GroupID;
    public string GroupDescription;
    private void Awake()
    {
        hideClanDetails();
        pendingDisplay.SetActive(false);
        CloseInviteUI();
        CloseSettingsUI();
        HideInvUI();
    }

 
    public void ClosePendingRequests()
    {
        pendingDisplay.SetActive(false);
    }
    public void OpenInviteUI()
    {
        inviteUI.SetActive(true);
    }
    public void CloseInviteUI()
    {
        inviteUI.SetActive(false);
    }
    public void OpenSettingsUI()
    {
        settingsUI.SetActive(true);
    }
    public void CloseSettingsUI()
    {
        settingsUI.SetActive(false);
    }
    public void ShowInvUI()
    {
        GroupInvUI.SetActive(true);
    }
    public void HideInvUI()
    {
        GroupInvUI.SetActive(false);
    }
    public void ShowPendingRequests()
    {

        pendingDisplay.SetActive(true);
        //delete all exisitng prefab if exist
        for (int i = requestDisplayContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = requestDisplayContent.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        var request = new ListGroupApplicationsRequest
        {
            Group = new PlayFab.GroupsModels.EntityKey { Id = GroupID, Type = "group" }
        };
        PlayFabGroupsAPI.ListGroupApplications(request,
            result =>   
            {
                foreach (var application in result.Applications)
                {

                    //Obtained TitleID
                    Debug.Log($"Pending Request: GroupID - {application.Group.Id}, PlayerID - {application.Entity.Key.Id}");
                    
                    //get the player's Master ID using TitleID
                    var getEntityProfileRequest = new GetEntityProfileRequest { Entity = new PlayFab.ProfilesModels.EntityKey { Id = application.Entity.Key.Id, Type = "title_player_account" } };
                    PlayFabProfilesAPI.GetProfile(getEntityProfileRequest, result => {
                        //Obtained MasterID(PlayFabID)
                        Debug.Log($"Player Master ID (PlayFabIDID): {result.Profile.Lineage.MasterPlayerAccountId}");
                        //get the player's DisplayName using MasterID(PlayFabID)
                        var getPlayerProfileRequest = new GetPlayerProfileRequest { PlayFabId = result.Profile.Lineage.MasterPlayerAccountId };
                        PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, result => {
                            //Obtained Display Name
                            Debug.Log(result.PlayerProfile.DisplayName);
                            GameObject playerRequest = Instantiate(requestGroupPrefab, requestDisplayContent);
                            playerRequest.GetComponent<GroupRequest>().playerID = application.Entity.Key.Id;
                            playerRequest.GetComponent<GroupRequest>().playerName = result.PlayerProfile.DisplayName;
                        }, result => Debug.Log(result));
                    }, result => Debug.Log(result));

                }
            },
            error => Debug.LogError($"Error listing group applications: {error.ErrorMessage}")
        );

    }
    public void OnShowClanDetails()
    {
        //refresh the friends list
        showClanDetails(GroupName, GroupDescription, GroupID);
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


    bool I_AM_ADMIN = false;
    //6. Make a somewhat "duplicated" fucntion that mirrors the fucntions that was created in the GameEvent.cs
    //It should have parameters that mimics what was needed
    //public UnityEvent <string,string> showClanDetails was made, thus we need "String String" in parameter
    void showClanDetails(string _clanName, string _clanDesription, string _groupID)
    {
        //init the pending button to be hiden and invite button to be invite button
        PendingInvitesButton.gameObject.SetActive(false);
        settingsBtn.gameObject.SetActive(false);
        JoinButton.gameObject.SetActive(false);
        LeaveButton.gameObject.SetActive(false);
        PendingJoinButton.gameObject.SetActive(false);

        //delete all exisitng prefab if exist
        for (int i = groupMemberDisplayContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = groupMemberDisplayContent.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        clanInfo.SetActive(true);
        clanName.text = _clanName;
        clanDescription.text = _clanDesription;

        //change the public group ID
        GroupName = _clanName;
        GroupID = _groupID;
        GroupDescription = _clanDesription;



        //check if player is in the group, else dont show the invite icon
        var requestMemberExistance = new IsMemberRequest
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

        PlayFabGroupsAPI.IsMember(requestMemberExistance,
            result =>
            {
                var listGrpMembersRequst = new ListGroupMembersRequest
                {
                    Group = new PlayFab.GroupsModels.EntityKey
                    {
                        Id = GroupID,
                        Type = "group" // Adjust the type based on your setup
                    }
                };
                //if its a member
                //switch join clan button to leave clan
                if (result.IsMember)
                {
                    //enable the leave button

                    LeaveButton.gameObject.SetActive(true);

                    //to check if its admin
                   
                    PlayFabGroupsAPI.ListGroupMembers(listGrpMembersRequst,
                      result =>
                      {
 
                          //see all members with Admin role

                          //Might as well show all the players in the group too
                          foreach (var members in result.Members[0].Members)
                          {
                              //player logged in is an admin of the group
                              if (members.Key.Id == MyPlayFab.Instance.myPlayFabTitleID)
                              {
                                  //allow current player to kick
                                  I_AM_ADMIN = true;

                                  //show the pending request button
                                  PendingInvitesButton.gameObject.SetActive(true);
                                  settingsBtn.gameObject.SetActive(true);
                              }
                              showPlayers(members);
                          }
                          //see all members with Member role
                          foreach (var members in result.Members[1].Members)
                          {
                              showPlayers(members);
                              Debug.Log(members.Key.Id);

                          }
                      },
                      error => Debug.LogError($"Error listing group membership: {error.GenerateErrorReport()}")
                        );


                }
                //keep the join clan button
                else
                {
                    //check if already pending request 
                    var listGrpApplicationReq = new ListGroupApplicationsRequest
                    {
                        Group = new PlayFab.GroupsModels.EntityKey
                        {   
                            Id = GroupID,
                            Type = "group"
                        }
                    };

                    PlayFabGroupsAPI.ListGroupApplications(listGrpApplicationReq,
                        result =>
                        {
                            // Check if the player has a pending application
                            bool hasPendingApplication = CheckForPendingApplication(result.Applications, MyPlayFab.Instance.myPlayFabTitleID);

                            if (hasPendingApplication)
                            {
                                //enable the request sent button
                                PendingJoinButton.gameObject.SetActive(true);
                            }
                            else
                            {
                                //enable the join button
                                JoinButton.gameObject.SetActive(true);
                            }
                        },
                        error => Debug.LogError($"Error listing group applications: {error.GenerateErrorReport()}")
                    );
                    PlayFabGroupsAPI.ListGroupMembers(listGrpMembersRequst,
                      result =>
                      {

                          //see all members with Admin role

                          //Might as well show all the players in the group too
                          foreach (var members in result.Members[0].Members)
                          {
                              showPlayers(members);

                          }
                          //see all members with Member role
                          foreach (var members in result.Members[1].Members)
                          {
                              showPlayers(members);
                          }
                      },
                      error => Debug.LogError($"Error listing group membership: {error.GenerateErrorReport()}")
                        );

                }
            },
            error => Debug.LogError($"Error listing group members: {error.GenerateErrorReport()}")
        );
    }

    void showPlayers(EntityWithLineage members)
    {
        //get the player's Master ID using TitleID
        var getPlayerProfileRequest = new GetEntityProfileRequest { Entity = new PlayFab.ProfilesModels.EntityKey { Id = members.Key.Id, Type = "title_player_account" } };
        PlayFabProfilesAPI.GetProfile(getPlayerProfileRequest, result => {
            //Use MasterID to get the DisplayName
            Debug.Log(result.Profile.Lineage.MasterPlayerAccountId);
            var GetProfileRequest = new GetPlayerProfileRequest { PlayFabId = result.Profile.Lineage.MasterPlayerAccountId };
            PlayFabClientAPI.GetPlayerProfile(GetProfileRequest, result => {
                //spawning the prefab
                GameObject groupMember = Instantiate(groupMemberPrefab, groupMemberDisplayContent);
                //setting the prefab name using displayName
                groupMember.GetComponent<GroupMemeber>().SetGroupMemberName(result.PlayerProfile.DisplayName);
                //setting the player Titile ID
                groupMember.GetComponent<GroupMemeber>().playerTitleID = members.Key.Id;
                //show kick button if I am admin and the player is not myself
                if (I_AM_ADMIN && members.Key.Id != MyPlayFab.Instance.myPlayFabTitleID)
                {
                    groupMember.GetComponent<GroupMemeber>().I_AM_ADMIN();
                }
            }, result => Debug.Log(result));

        }, result => Debug.Log(result));
    }
    bool CheckForPendingApplication(List<GroupApplication> applications, string playerId)
    {
        return applications.Exists(app => app.Entity.Key.Id == playerId);
    }
    public void hideClanDetails()
    {
        clanInfo.SetActive(false);
    }
}
