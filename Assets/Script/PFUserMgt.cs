using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
public class PFUserMgt : MonoBehaviour
{


    [SerializeField]
    TMP_InputField userEmail, userPassword, userName, currentScore, displayName;
    [SerializeField]
    TextMeshProUGUI Message;

    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
        Message.text = newMessage;
    }
    void OnError(PlayFabError error)
    {
        UpdateMessage("Error " + error.GenerateErrorReport());
    }

    public void OnButtonRegister()
    {
        var regReq = new RegisterPlayFabUserRequest {
            Email = userEmail.text,
            Password = userPassword.text, Username = userName.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(regReq, OnRegSuccessful, OnError);
    }
    void OnRegSuccessful(RegisterPlayFabUserResult r)
    {
        UpdateMessage("Registration success!");

        var req = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
    }
    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMessage("Display name updated to " + r.DisplayName);
    }
    void OnLoginSuccessful(LoginResult r)
    {
        UpdateMessage("Login successful!" + r.PlayFabId + r.InfoResultPayload.PlayerProfile.DisplayName);

        //goes into the main menu
        SceneManager.LoadScene("MainMenu");
    }
    public void OnButtonLoginEmail()
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = userEmail.text,
            Password = userPassword.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccessful, OnError);
    }
    public void OnButtonLoginUserName()
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = userName.text,
            Password = userPassword.text,
            //to get player profile, including display name
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccessful, OnError);
    }

    public void OnButtonGetLeaderboard()
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = "highscore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        string LeaderboardStr = "Leaderboard \n";
        foreach (var item in r.Leaderboard)
        {
            string oneraw = item.Position + '/' + item.PlayFabId + '/' + item.DisplayName + '/' + item.StatValue + "\n";
            LeaderboardStr += oneraw;//combine all display into one string
            UpdateMessage(LeaderboardStr);
        }
    }
    public void OnButtonSendLeaderboard()
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="highscore",
                    Value = int.Parse(currentScore.text)
                }
            }
        };
        UpdateMessage("Submitting score:" + currentScore.text);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnleaderboardUpdate, OnError);
    }
    void OnleaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        UpdateMessage("Successful leaderboard sent:"+r.ToString());
    }
}