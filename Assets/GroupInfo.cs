using PlayFab.GroupsModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Dynamic;
using PlayFab.ClientModels;

public class GroupInfo : MonoBehaviour
{
    public string GrpID;
    // Start is called before the first frame update

    public void AcceptGroupRequest()
    {
        var request = new AcceptGroupInvitationRequest { Group = new PlayFab.GroupsModels.EntityKey { Id = GrpID, Type = "group" }, Entity = new PlayFab.GroupsModels.EntityKey { Id = MyPlayFab.Instance.myPlayFabTitleID, Type = "title_player_account" } };
        PlayFabGroupsAPI.AcceptGroupInvitation(request, result => {
  
        }, result => Debug.Log(result));
    }
    public void DenyGroupRequest()
    {
        //accpet invide and then kick
        var request = new AcceptGroupInvitationRequest { Group = new PlayFab.GroupsModels.EntityKey { Id = GrpID, Type = "group" }, Entity = new PlayFab.GroupsModels.EntityKey { Id = MyPlayFab.Instance.myPlayFabTitleID, Type = "title_player_account" } };
        PlayFabGroupsAPI.AcceptGroupInvitation(request, result => {
            //kick the player
            var request = new RemoveMembersRequest { Group = new PlayFab.GroupsModels.EntityKey { Id = GrpID, Type = "group" }, Members = new List<PlayFab.GroupsModels.EntityKey> { new PlayFab.GroupsModels.EntityKey { Id = MyPlayFab.Instance.myPlayFabTitleID, Type = "title_player_account" } } };
            PlayFabGroupsAPI.RemoveMembers(request, result=>{ Destroy(gameObject); }, result => { Debug.Log(result); });

        }, result => Debug.Log(result));
    }
}
