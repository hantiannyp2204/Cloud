using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]
    PlayerStats playerStats;

    [SerializeField]
    TMP_Text playerDisplayName;
    [SerializeField]
    TMP_Text currentLevel;
    [SerializeField]
    TMP_Text currentXP;
    [SerializeField]
    UnityEngine.UI.Slider XpSlider;
    [SerializeField]
    TMP_Text playcount;
    int maxXP;
    int totalPlayAmount;
    private void Start()
    {

        getPlayAmount();
    }
    public void increasePlayTime()
    {
        if (PlayerPrefs.GetInt("shipExist") == 1)
        {
            setPlayAmount(totalPlayAmount + 1);
        }
            
    }
    public int getPlayAmount()
    {
        int playAmount = 0;
        var request = new GetUserDataRequest
        {
            PlayFabId = PlayFabSettings.staticPlayer.PlayFabId
        };

        // Send the request to get user data
        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data.TryGetValue("TimesPlayed", out var dataValue))
            {
                // Player data value found
                Debug.Log($"Value: {dataValue.Value}");
                playAmount = int.Parse(dataValue.Value);
                totalPlayAmount = playAmount;
                playcount.text = "Attempt number:\n" + dataValue.Value;
            }

        }, result => Debug.Log("ERROR"));
        return playAmount;
    }
    public void setPlayAmount(int amount)
    {

        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"TimesPlayed",amount.ToString()}
            }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("DATA sent successful"), result => Debug.Log("Data send error"));
    }
    private void Update()
    {
        if(playerStats.RunSetUI() == true)
        {
            SetUI();
        }
    }
    public void SetUI()
    {
        PlayerStats _playerStats = playerStats;
        playerDisplayName.text = _playerStats.playerName;
        XpSlider.value = int.Parse(_playerStats.GetLevelAndXP().xp.ToString());
        currentXP.text = _playerStats.GetLevelAndXP().xp.ToString();
        currentLevel.text = _playerStats.GetLevelAndXP().level.ToString();
        maxXP = 10 * _playerStats.GetLevelAndXP().level;
        XpSlider.maxValue = maxXP;
    }


}
