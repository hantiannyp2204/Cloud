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
    TMP_InputField newLevel;
    [SerializeField]
    TMP_InputField newXP;
    int maxXP;

    private void Start()
    {
        Invoke("SetUI", 1);
    }
    public void SetUI()
    {
        PlayerStats _playerStats = playerStats;
        playerDisplayName.text = _playerStats.playerName;
        Debug.Log(playerDisplayName.text);
        XpSlider.value = int.Parse(_playerStats.GetLevelAndXP().xp.ToString());
        currentXP.text = _playerStats.GetLevelAndXP().xp.ToString();
        currentLevel.text = _playerStats.GetLevelAndXP().level.ToString();
        maxXP = 10 * _playerStats.GetLevelAndXP().level;
        XpSlider.maxValue = maxXP;
    }
    public Stats SendNewLevelAndXP()
    {
        return new Stats(int.Parse(newLevel.text), int.Parse(newXP.text), maxXP);
    }

}
