using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PFUserMgt : MonoBehaviour
{
    [SerializeField]
    TMP_InputField userEmail, userPassword, userInfo, currentScore, displayName;
    [SerializeField]
    TMP_Text registerText, usernamePlaceholder, loginButtonText;
    [SerializeField]
    TextMeshProUGUI Message;

    [Header("animtion")]
    public Animator animator;

    bool isRegistering = false;
   
    public void OnLoginButtonPressed()
    {
        if(isRegistering == false)
        {

            OnButtonLogin();
        }
        else
        {

            OnButtonRegister();
        }
    }
    private void Start()
    {
        switchToLogin();
        UpdateMessage("");
       
    }
    void UpdateMessage(string newMessage)
    {
        Debug.Log(newMessage);
        Message.text = newMessage;
    }
    void OnError(PlayFabError error)
    {
        UpdateMessage("Error " + error.GenerateErrorReport());
    }
    void OnErrorUsernameLogin(PlayFabError error)
    {
        OnButtonLoginEmail();
    }
    void OnErrorEmailAndUsernameLogin(PlayFabError error)
    {
        UpdateMessage("Error Username/Email or Password Invalid");
    }
    public void toggleLoginRegister()
    {
        //registering
        if(isRegistering == false)
        {
            animator.Play("LoginAnimation");
            registerText.text = "Login";
            switchToRegister();
            isRegistering = true;
        }
        //looging in
        else
        {
            animator.Play("RegisterAnimation");
            registerText.text = "Register";
            switchToLogin();
            isRegistering = false;
        }
    }
    void switchToRegister()
    {
        loginButtonText.text = "Register";
        usernamePlaceholder.text = "Username";
        userEmail.interactable = true;
    }
    void switchToLogin()
    {
        loginButtonText.text = "Login";
        usernamePlaceholder.text = "Username/Email";
        userEmail.interactable = false;
    }
    public void OnButtonPasswordReset()
    {
        var passReq = new SendAccountRecoveryEmailRequest
        {
            Email = userInfo.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(passReq, OnPasswordRecoverSuccess, OnError);
    }
    void OnPasswordRecoverSuccess(SendAccountRecoveryEmailResult r)
    {
        UpdateMessage("Password recovery sent to " + userEmail.text);
    }
    public void OnButtonRegister()
    {
        var regReq = new RegisterPlayFabUserRequest {
            Email = userEmail.text,
            Password = userPassword.text, Username = userInfo.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(regReq, OnRegSuccessful, OnError);
    }
    void OnRegSuccessful(RegisterPlayFabUserResult r)
    {
        UpdateMessage("Registration success!");

        //update current player's name
        var req = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
    }
    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMessage("Display name updated to " + r.DisplayName);
    }
    void OnLoginSuccessful(LoginResult r)
    {
        UpdateMessage("Login successful! Welcome " + r.InfoResultPayload.PlayerProfile.DisplayName);
        Message.color = Color.green;
        SceneManager.LoadScene("MainMenu");
    }
    public void OnButtonLoginEmail()
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = userInfo.text,
            Password = userPassword.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccessful, OnErrorEmailAndUsernameLogin);
    }
    public void OnButtonLogin()
    {
        var loginRequest = new LoginWithPlayFabRequest
        { 
            Username = userInfo.text,
            Password = userPassword.text,
            //to get player profile, including display name
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        //try email login if dont work
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccessful, OnErrorUsernameLogin);
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