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
using PlayFab.DataModels;
using PlayFab.PfEditor.EditorModels;
using static GuideController;
using UnityEngine.UIElements;
using PlayFab.ProfilesModels;
using System.Security.Principal;

public class GuideController : MonoBehaviour
{
    //for inputs
    [SerializeField] TMP_InputField guideName;
    [SerializeField] TMP_InputField guideDescription;
    [SerializeField] GameObject display;
    [SerializeField] ClanInfo clanInfo;
    //For prefabs
    [SerializeField] GameObject clanPrefab;

    //player info
    string PlayerTitleID;

    //ghost memeber info
    PlayFab.GroupsModels.EntityKey ghostEntity = EntityKeyMaker("12BCA4C50B6DAE79", "title_player_account");

    public readonly HashSet<KeyValuePair<string, string>> entityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public readonly Dictionary<string, string> groupNameById = new Dictionary<string, string>();



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

            // Deserialize the JSON string into a GroupData object
            GroupData groupData = JsonUtility.FromJson<GroupData>(result.FunctionResult.ToString());
            //claer all existing prefabs first
            for (int i = display.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = display.transform.GetChild(i);
                Destroy(child.gameObject);
            }
            if (groupData != null && groupData.Groups != null)
            {
                // Iterate through the Groups and log the GroupNames
                foreach (var group in groupData.Groups)
                {
                    Debug.Log($"GroupName: {group.GroupName}, GroupId: {group.Group.Id}");
                    //spawn the prefab
                    GameObject currentClan = Instantiate(clanPrefab, display.transform);

                    //set the name
                    TMP_Text clanName = currentClan.transform.Find("Clan name").GetComponent<TMP_Text>();
                    clanName.text = group.GroupName;
                    //set the name in the ClanPrefab.cs
                    currentClan.transform.GetComponent<ClanPrefab>().ClanName = clanName.text;

                    //set the Description
                    var getRequest = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = group.Group.Id, Type = "group" } };
                    PlayFabDataAPI.GetObjects(getRequest,
                         result =>
                         {

                             foreach (var objectData in result.Objects)
                             {
                                 if (objectData.Key == "Description")
                                 {
                                     TMP_Text clanDescription = currentClan.transform.Find("Clan description").GetComponent<TMP_Text>();
                                     clanDescription.text = objectData.Value.DataObject.ToString();
                                     Debug.Log(objectData.Value.DataObject.ToString());
                                     currentClan.transform.GetComponent<ClanPrefab>().ClanDescription = clanDescription.text;
                                 }

                             }
                         },
                        result => Debug.Log(result));
                    //set the group ID
                    currentClan.transform.GetComponent<ClanPrefab>().ClanGroupID = group.Group.Id;
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
        public GroupDetails Group;
        // Add other properties as needed
    }

    [System.Serializable]
    public class GroupDetails
    {
        public string Id;
        public string Type;
        public string TypeString;
        // Add other properties as needed
    }
    public void ListMyGroups()
    {
        var request = new ListMembershipRequest { Entity = EntityKeyMaker(MyPlayFab.Instance.myPlayFabTitleID, "title_player_account") };
        PlayFabGroupsAPI.ListMembership(request, OnListGroups, OnSharedError);
    }
    private void OnListGroups(ListMembershipResponse response)
    {
        //claer all existing prefabs first
        for (int i = display.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = display.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        var prevRequest = (ListMembershipRequest)response.Request;
        // Iterate through the Groups and log the GroupNames
        foreach (var group in response.Groups)
        {
            Debug.Log($"GroupName: {group.GroupName}, GroupId: {group.Group.Id}");
            //spawn the prefab
            GameObject currentClan = Instantiate(clanPrefab, display.transform);

            //set the name
            TMP_Text clanName = currentClan.transform.Find("Clan name").GetComponent<TMP_Text>();
            clanName.text = group.GroupName;
            //set the name in the ClanPrefab.cs
            currentClan.transform.GetComponent<ClanPrefab>().ClanName = clanName.text;

            //set the Description
            var getRequest = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = group.Group.Id, Type = "group" } };
            PlayFabDataAPI.GetObjects(getRequest,
                 result =>
                 {

                     foreach (var objectData in result.Objects)
                     {
                         if(objectData.Key == "Description")
                         {
                             TMP_Text clanDescription = currentClan.transform.Find("Clan description").GetComponent<TMP_Text>();
                             clanDescription.text = objectData.Value.DataObject.ToString();
                            Debug.Log(objectData.Value.DataObject.ToString());
                            currentClan.transform.GetComponent<ClanPrefab>().ClanDescription = clanDescription.text;
                         }
   
                     }
                 },
                result => Debug.Log(result));
            //set the group ID
            currentClan.transform.GetComponent<ClanPrefab>().ClanGroupID = group.Group.Id;
        }
    }
    public void CreateDescription(string description,string GroupID)
    {
        if(description == "")
        {
            description = "This guild has no description";
        }
        var dataList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = "Description",
                //input the description
                DataObject = description
            }
        };
        PlayFabDataAPI.SetObjects(new SetObjectsRequest()
        {
            Entity = new PlayFab.DataModels.EntityKey { Id = GroupID, Type = "group" }, // Saved from GetEntityToken, or a specified key created from a titlePlayerId, CharacterId, etc
            Objects = dataList,
        }, (setResult) => {
            Debug.Log(setResult.ProfileVersion);
        }, result=>Debug.Log(result));
    }
    public void CreateGroup()
    {
        // A player-controlled entity creates a new group
        var request = new CreateGroupRequest { GroupName = guideName.text, Entity = EntityKeyMaker(MyPlayFab.Instance.myPlayFabTitleID, "title_player_account") };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
    }
    private void OnCreateGroup(CreateGroupResponse response)
    {
        Debug.Log("Group Created: " + response.GroupName + " - " + response.Group.Id);

        var prevRequest = (CreateGroupRequest)response.Request;
        entityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, response.Group.Id));
        groupNameById[response.Group.Id] = response.GroupName;

        //add a description
        CreateDescription(guideDescription.text,response.Group.Id);

        //add ghost memeber

        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(response.Group.Id, "group"), Entity = ghostEntity };
        PlayFabGroupsAPI.ApplyToGroup(request, result => {
            var prevRequest = (ApplyToGroupRequest)result.Request;

            // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
            var request = new AcceptGroupApplicationRequest { Group = prevRequest.Group, Entity = ghostEntity };
            PlayFabGroupsAPI.AcceptGroupApplication(request, result => {
                var prevRequest = (AcceptGroupApplicationRequest)result.Request;
                Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);

                //Make a ghost role
                var AddGhostRole = new CreateGroupRoleRequest { Group = EntityKeyMaker(response.Group.Id, "group"), RoleId = "ghost", RoleName = "Ghost" };
                PlayFabGroupsAPI.CreateRole(AddGhostRole, result => {
                    //change ghost member's role to ghost (it onyl accept a list
                    var MembersToChange = new List<PlayFab.GroupsModels.EntityKey>();
                    MembersToChange.Add(ghostEntity);
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
        foreach (var each in entityGroupPairs)
            if (each.Value != prevRequest.Group.Id)
                temp.Add(each);
        entityGroupPairs.IntersectWith(temp);
        groupNameById.Remove(prevRequest.Group.Id);
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
        entityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, prevRequest.Group.Id));
    }
    public void ApplyToGroup()
    {
        // A player-controlled entity applies to join an existing group (of which they are not already a member)
        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(clanInfo.GroupID, "group"), Entity = EntityKeyMaker(MyPlayFab.Instance.myPlayFabTitleID,"title_player_account") };

        //reset the current guild screen
        GameEvent.instance.showClanDetails.Invoke(clanInfo.GroupName, clanInfo.GroupDescription, clanInfo.GroupID);
        PlayFabGroupsAPI.ApplyToGroup(request, result => Debug.Log(result), result => Debug.Log(result));
    }
    public void AcceptUser(string _playerID)
    {

        var request = new AcceptGroupApplicationRequest { Group = EntityKeyMaker(clanInfo.GroupID, "group"), Entity = EntityKeyMaker(_playerID, "title_player_account") };
        PlayFabGroupsAPI.AcceptGroupApplication(request, result => Debug.Log(result), result => Debug.Log(result));

    }
    public void DeclineUser(string _playerID)
    {
        //accpet then kick
        var request = new AcceptGroupApplicationRequest { Group = EntityKeyMaker(clanInfo.GroupID, "group"), Entity = EntityKeyMaker(_playerID, "title_player_account") };
        PlayFabGroupsAPI.AcceptGroupApplication(request, result => { KickMember(EntityKeyMaker(_playerID, "title_player_account")); }, result => Debug.Log(result));
    }
    public void OnLeaveButtonPressed()
    {
        KickMember(EntityKeyMaker(MyPlayFab.Instance.myPlayFabTitleID, "title_player_account"));
    }    
    public void KickMember( PlayFab.GroupsModels.EntityKey entityKey)
    {
        var request = new RemoveMembersRequest { Group = EntityKeyMaker(clanInfo.GroupID, "group"), Members = new List<PlayFab.GroupsModels.EntityKey> { entityKey } };
        PlayFabGroupsAPI.RemoveMembers(request, OnKickMembers, OnSharedError);

       
    }
    private void OnKickMembers(PlayFab.GroupsModels.EmptyResponse response)
    {
        var prevRequest = (RemoveMembersRequest)response.Request;

        Debug.Log("Entity kicked from Group: " + prevRequest.Members[0].Id + " to " + prevRequest.Group.Id);
        entityGroupPairs.Remove(new KeyValuePair<string, string>(prevRequest.Members[0].Id, prevRequest.Group.Id));

        //reset the current guild screen
        GameEvent.instance.showClanDetails.Invoke(clanInfo.GroupName, clanInfo.GroupDescription, prevRequest.Group.Id);
    }
    private void OnEnable()
    {
        GameEvent.instance.AcceptClanJoining.AddListener(AcceptUser);
        GameEvent.instance.DeclineClanJoining.AddListener(DeclineUser);
        GameEvent.instance.KickPlayer.AddListener(KickMember);
    }
    private void OnDisable()
    {
        GameEvent.instance.AcceptClanJoining.RemoveListener(AcceptUser);
        GameEvent.instance.DeclineClanJoining.RemoveListener(DeclineUser);
        GameEvent.instance.KickPlayer.RemoveListener(KickMember);
    }
}
