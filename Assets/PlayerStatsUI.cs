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

    public void SetUI(Stats stat)
    {
        playerDisplayName.text = playerStats.playerName;
        XpSlider.value = int.Parse(stat.xp.ToString());
        currentXP.text = stat.xp.ToString();
        currentLevel.text = stat.level.ToString();
        maxXP = 10 * stat.level;
        XpSlider.maxValue = maxXP;
    }
    public Stats SendNewLevelAndXP()
    {
        return new Stats(int.Parse(newLevel.text), int.Parse(newXP.text), maxXP);
    }

}
