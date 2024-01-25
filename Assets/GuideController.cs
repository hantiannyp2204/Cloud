using PlayFab.GroupsModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using System;

public class GuideController : MonoBehaviour
{
    //for inputs
    [SerializeField] TMP_InputField GuideName;
    [SerializeField] TMP_Text txtDisplay;
    //player info
    string PlayerTitleID;

    //ghost memeber info
    PlayFab.GroupsModels.EntityKey GhostEntity = EntityKeyMaker("12BCA4C50B6DAE79", "title_player_account");

    public readonly HashSet<KeyValuePair<string, string>> EntityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public readonly Dictionary<string, string> GroupNameById = new Dictionary<string, string>();



    public static PlayFab.GroupsModels.EntityKey EntityKeyMaker(string entityId, string type)
    {
        return new PlayFab.GroupsModels.EntityKey { Id = entityId, Type = type };
    }
    private void OnSharedError(PlayFab.PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    public void GoToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ListAllGuilds()
    {
        var request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "ListMembershipFromCloudScript",
            FunctionParameter = new
            {
                entityId = "12BCA4C50B6DAE79"
            }

        };
        PlayFabClientAPI.ExecuteCloudScript(request, result => {

            txtDisplay.text = "";
            // Deserialize the JSON string into a GroupData object
            GroupData groupData = JsonUtility.FromJson<GroupData>(result.FunctionResult.ToString());

            if (groupData != null && groupData.Groups != null)
            {
                // Iterate through the Groups and log the GroupNames
                foreach (var group in groupData.Groups)
                {
                    Debug.Log("GroupName: " + group.GroupName);
                    txtDisplay.text += group.GroupName+"\n";
                }
            }
            else
            {
                Debug.Log("Invalid or missing data in the JSON response.");
            }

        }, result => Debug.Log("Some error in code dahh"));
    }
    [System.Serializable]
    public class GroupData
    {
        public List<GroupInfo> Groups;
    }

    [System.Serializable]
    public class GroupInfo
    {
        public string GroupName;
        // Add other properties as needed
    }
    public void ListMyGroups()
    {
        var request = new ListMembershipRequest { Entity = EntityKeyMaker(MyPlayFab.Instance.myPlayFabTitleID, "title_player_account") };
        PlayFabGroupsAPI.ListMembership(request, OnListGroups, OnSharedError);
    }
    private void OnListGroups(ListMembershipResponse response)
    {
        txtDisplay.text = "";
        var prevRequest = (ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            txtDisplay.text += pair.GroupName + "\n";
            EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
        }
    }

    public void CreateGroup()
    {
        // A player-controlled entity creates a new group
        var request = new CreateGroupRequest { GroupName = GuideName.text, Entity = EntityKeyMaker(MyPlayFab.Instance.myPlayFabTitleID, "title_player_account") };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
    }
    private void OnCreateGroup(CreateGroupResponse response)
    {
        Debug.Log("Group Created: " + response.GroupName + " - " + response.Group.Id);

        var prevRequest = (CreateGroupRequest)response.Request;
        EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, response.Group.Id));
        GroupNameById[response.Group.Id] = response.GroupName;


        //add ghost memeber

        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(response.Group.Id, "group"), Entity = GhostEntity };
        PlayFabGroupsAPI.ApplyToGroup(request, result => {
            var prevRequest = (ApplyToGroupRequest)result.Request;

            // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
            var request = new AcceptGroupApplicationRequest { Group = prevRequest.Group, Entity = GhostEntity };
            PlayFabGroupsAPI.AcceptGroupApplication(request, result => {
                var prevRequest = (AcceptGroupApplicationRequest)result.Request;
                Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);

                //Make a ghost role
                var AddGhostRole = new CreateGroupRoleRequest { Group = EntityKeyMaker(response.Group.Id, "group"), RoleId = "ghost", RoleName = "Ghost" };
                PlayFabGroupsAPI.CreateRole(AddGhostRole, result => {
                    //change ghost member's role to ghost (it onyl accept a list
                    var MembersToChange = new List<PlayFab.GroupsModels.EntityKey>();
                    MembersToChange.Add(GhostEntity);
                    var changeRole = new ChangeMemberRoleRequest { Group = EntityKeyMaker(response.Group.Id, "group"), Members = MembersToChange, OriginRoleId = "members", DestinationRoleId = "ghost" };
                    PlayFabGroupsAPI.ChangeMemberRole(changeRole, result => Debug.Log("Changed member to Ghost"), result => Debug.Log(result));
                }, result => Debug.Log(result));
            }, OnSharedError);
        },OnSharedError);

      

    }
    public void DeleteGroup(string groupId)
    {
        // A title, or player-controlled entity with authority to do so, decides to destroy an existing group
        var request = new DeleteGroupRequest { Group = EntityKeyMaker(groupId,"group") };
        PlayFabGroupsAPI.DeleteGroup(request, OnDeleteGroup, OnSharedError);
    }
    private void OnDeleteGroup(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (DeleteGroupRequest)response.Request;
        Debug.Log("Group Deleted: " + prevRequest.Group.Id);

        var temp = new HashSet<KeyValuePair<string, string>>();
        foreach (var each in EntityGroupPairs)
            if (each.Value != prevRequest.Group.Id)
                temp.Add(each);
        EntityGroupPairs.IntersectWith(temp);
        GroupNameById.Remove(prevRequest.Group.Id);
    }

    public void InviteToGroup(string groupId, PlayFab.GroupsModels.EntityKey entityKey)
    {
        // A player-controlled entity invites another player-controlled entity to an existing group
        var request = new InviteToGroupRequest { Group = EntityKeyMaker(groupId, "group"), Entity = entityKey };
        PlayFabGroupsAPI.InviteToGroup(request, OnInvite, OnSharedError);
    }
    public void OnInvite(InviteToGroupResponse response)
    {
        var prevRequest = (InviteToGroupRequest)response.Request;

        // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
        var request = new AcceptGroupInvitationRequest { Group = EntityKeyMaker(prevRequest.Group.Id,"group"), Entity = prevRequest.Entity };
        PlayFabGroupsAPI.AcceptGroupInvitation(request, OnAcceptInvite, OnSharedError);
    }
    public void OnAcceptInvite(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (AcceptGroupInvitationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
        EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, prevRequest.Group.Id));
    }
    public void ApplyToGroup(string groupId, PlayFab.GroupsModels.EntityKey entityKey)
    {
        // A player-controlled entity applies to join an existing group (of which they are not already a member)
        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(groupId, "group"), Entity = entityKey };
        PlayFabGroupsAPI.ApplyToGroup(request, OnApply, OnSharedError);
    }

    public void OnApply(ApplyToGroupResponse response)
    {
        var prevRequest = (ApplyToGroupRequest)response.Request;

        // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
        var request = new AcceptGroupApplicationRequest {Group = prevRequest.Group, Entity = prevRequest.Entity };
        PlayFabGroupsAPI.AcceptGroupApplication(request, OnAcceptApplication, OnSharedError);
    }
    public void OnAcceptApplication(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
    }
    public void KickMember(string groupId, PlayFab.GroupsModels.EntityKey entityKey)
    {
        var request = new RemoveMembersRequest { Group = EntityKeyMaker(groupId, "group"), Members = new List<PlayFab.GroupsModels.EntityKey> { entityKey } };
        PlayFabGroupsAPI.RemoveMembers(request, OnKickMembers, OnSharedError);

       
    }
    private void OnKickMembers(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (RemoveMembersRequest)response.Request;

        Debug.Log("Entity kicked from Group: " + prevRequest.Members[0].Id + " to " + prevRequest.Group.Id);
        EntityGroupPairs.Remove(new KeyValuePair<string, string>(prevRequest.Members[0].Id, prevRequest.Group.Id));
    }
}
