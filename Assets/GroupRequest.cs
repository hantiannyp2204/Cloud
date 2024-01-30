using PlayFab.ProfilesModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.PfEditor.EditorModels;

public class GroupRequest : MonoBehaviour
{
    public string playerID;
    public string playerName;
    [SerializeField] TMP_Text Name;

    private void Start()
    {
        Name.text = playerName;
    }
    public void AcceptPlayerToClan()
    {
        GameEvent.instance.AcceptClanJoining.Invoke(playerID);
        Destroy(gameObject);
    }
    public void DeclinePlayerToClan()
    {
        GameEvent.instance.DeclineClanJoining.Invoke(playerID);
        Destroy(gameObject);
    }
    public void GetDisplayNameWithTitleID(string titleID)
    {
        Debug.Log(titleID);
        var request = new GetEntityProfileRequest
        {
            Entity = new PlayFab.ProfilesModels.EntityKey { Id = titleID, Type = "title_player_account" }
        };
        PlayFabProfilesAPI.GetProfile(request, result => {
            Debug.Log(result.Profile.Lineage.MasterPlayerAccountId);
        }, result => Debug.Log(result));
    }
    public void SetDisplayName(string newName)
    {
        Name.text = newName;
    }
}
