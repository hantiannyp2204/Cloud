using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScoreboard : MonoBehaviour
{
    bool isLeaderboard;

    [SerializeField]
    private GameObject leaderboard, scoreboard;
    // Start is called before the first frame update
    void Start()
    {
        isLeaderboard = true;
        checkToggle(false);
    }

    void checkToggle(bool toggle)
    {
        if(toggle == true)
        {
            leaderboard.SetActive(true);
            scoreboard.SetActive(false);
        }
        else
        {
            scoreboard.SetActive(true);
            leaderboard.SetActive(false);
        }
    }
    public void togleLeaderboard()
    {
        if (isLeaderboard == true)
        {
            checkToggle(true);
            isLeaderboard = false;
         
        }
        else
        {
            checkToggle(false);
            isLeaderboard = true;
        
        }
    }
}
