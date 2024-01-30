using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupMemeber : MonoBehaviour
{
    [SerializeField]
    TMP_Text memberName;

    [SerializeField]
    GameObject kickButton;

    public string playerTitleID;
    private void Awake()
    {
        kickButton.SetActive(false);
    }

    public void SetGroupMemberName(string playerName)
    {
        memberName.text = playerName;
    }
    public void I_AM_ADMIN()
    {
        kickButton.SetActive(true);
    }
    public void kickPlayer()
    {
        GameEvent.instance.KickPlayer.Invoke(new PlayFab.GroupsModels.EntityKey{Id = playerTitleID,Type = "title_player_account" });
        Destroy(gameObject);
    }
}
