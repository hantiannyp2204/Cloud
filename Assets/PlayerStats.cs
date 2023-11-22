using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using System.Security.Cryptography;
using PlayFab.ClientModels;
using UnityEditor.PackageManager;

[System.Serializable]
public class Stats
{
    public int level;
    public int xp;
    public Stats(int _level, int _xp)
    {
        level = _level;
        xp = _xp;
    }
}
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    TMP_InputField newLevel;
    [SerializeField]
    Slider XpSlider;
    [SerializeField]
    TMP_Text xpTxt;

    string playerName;
    int currentLevel, currentXP;
    public void GetUserAccountInfo()
    {
        var request = new GetPlayerProfileRequest
        {
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnGetUserAccountInfoSuccess, OnError);
        
    }

    void OnGetUserAccountInfoSuccess(GetPlayerProfileResult result)
    {
        playerName = result.PlayerProfile.DisplayName;
    }
    public Stats ReturnClass()
    {
        return new Stats(int.Parse(newLevel.text),int.Parse(xpTxt.text));
    }
    public void SetUI(Stats stat)
    {
        XpSlider.value = stat.level;
        xpTxt.text = stat.xp.ToString();
    }
    public void SliderChangeUpdate()
    {
        xpTxt.text = XpSlider.value.ToString();
    }
    void OnError(PlayFabError error)
    {
        UpdateMessage("Error " + error.GenerateErrorReport());
    }

    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
    }
}
