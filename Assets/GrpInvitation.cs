using PlayFab.GroupsModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ProfilesModels;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using static GuideController;
using TMPro;

public class GrpInvitation : MonoBehaviour
{
    [SerializeField] Transform GrpInvDisplayContent;
    [SerializeField] GameObject invitedByGrpPrefab;

    private void ShowGroupInvitations()
    {
        //claer all existing prefabs first
        for (int i = GrpInvDisplayContent.childCount - 1; i >= 0; i--)
        {
            Transform child = GrpInvDisplayContent.GetChild(i);
            Destroy(child.gameObject);
        }
        //loop through all the groups avaliable in the game
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
            if (groupData != null && groupData.Groups != null)
            {
                // Iterate through all grps to check if it has any invites
                foreach (var group in groupData.Groups)
                {
                    Debug.Log($"GroupName: {group.GroupName}, GroupId: {group.Group.Id}");
                    var checkGroupInvReq = new ListGroupInvitationsRequest
                    {
                        Group = new PlayFab.GroupsModels.EntityKey { Id = group.Group.Id, Type = "" }
                    };

                    PlayFabGroupsAPI.ListGroupInvitations(checkGroupInvReq,
                        result =>
                        {
                            foreach (var invitation in result.Invitations)
                            {
                                //check if any of the invitation in ours (Invited players' title ID = my title ID
                                if (invitation.InvitedEntity.Key.Id == MyPlayFab.Instance.myPlayFabTitleID)
                                {
                                    //Make the prefab
                                    Debug.Log("Invited to group: " + invitation.Group.Id);
                                    GameObject GrpInvt = Instantiate(invitedByGrpPrefab, GrpInvDisplayContent);
                                    GrpInvt.transform.Find("GrpName").GetComponent<TMP_Text>().text = group.GroupName;
                                    GrpInvt.transform.GetComponent<GroupInfo>().GrpID = group.Group.Id;
                                }

                            }

                        },
                        error => Debug.LogError($"Error listing group invitations: {error.GenerateErrorReport()}")
                    );

                }
            }
            else
            {
                Debug.Log("Invalid or missing data in the JSON response.");
            }

        }, result => Debug.Log("Some error in code dahh"));

    }
    private void OnEnable()
    {
        ShowGroupInvitations(); 
    }
}
