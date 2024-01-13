using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UIElements;
using UnityEngine.SocialPlatforms.Impl;

public class PFUserMgt : MonoBehaviour
{
    [SerializeField]
    TMP_InputField userEmail, userPassword, userInfo, currentScore;
    [SerializeField]
    TMP_Text registerText, usernamePlaceholder, loginButtonText;
    [SerializeField]
    TextMeshProUGUI Message;

    [Header("animtion")]
    public Animator animator;

    bool isRegistering = false;

    string deviceID = null;
    string currentUserName;
    deviceType currentDeviceType = deviceType.None;
    private enum deviceType
    {
        None,
        Andriod,
        IOS,
        PC
    }
    public void OnLoginButtonPressed()
    {
        if (isRegistering == false)
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
        if (isRegistering == false)
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
            DisplayName = userInfo.text,
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(req, result=>Debug.Log("Username updated to" + req.DisplayName), OnError);
    }
    public void OnDeviceLoggin()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            deviceID = secure.CallStatic<string>("getString", contentResolver, "android_id");
            currentDeviceType = deviceType.Andriod;
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            #if UNITY_IOS
            deviceID = UnityEngine.iOS.Device.vendorIdentifier;
            #endif
            currentDeviceType = deviceType.IOS;
        }
        else
        {
            deviceID = SystemInfo.deviceUniqueIdentifier;
            currentDeviceType = deviceType.PC;
        }

        LoginWithCurrentDevice();
    }
    void LoginWithCurrentDevice()
    {
        switch (currentDeviceType)
        {
            case deviceType.Andriod:
                var andriodLogin = new LoginWithAndroidDeviceIDRequest
                {
                    AndroidDeviceId = deviceID.ToString(),
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true
                };

                PlayFabClientAPI.LoginWithAndroidDeviceID(andriodLogin, OnLoginSuccessful, OnError);
                break;
            case deviceType.IOS:
                var IOSLogin = new LoginWithIOSDeviceIDRequest
                {
                    DeviceId = deviceID.ToString(),
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true
                };
                PlayFabClientAPI.LoginWithIOSDeviceID(IOSLogin, OnLoginSuccessful, OnError);
                break;
            case deviceType.PC:
                var PCLogin = new LoginWithCustomIDRequest
                {
                    CustomId = deviceID.ToString(),
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true
                };
                PlayFabClientAPI.LoginWithCustomID(PCLogin, OnLoginSuccessful, OnError);
                break;
        }
        Debug.Log(currentDeviceType.ToString());


    }

    public void OnButtonLoginAsGuest()
    {
        var loginReq = new LoginWithCustomIDRequest
        {
            CustomId = "Guest",
            CreateAccount= true
        };
        currentUserName = "Guest";
        PlayFabClientAPI.LoginWithCustomID(loginReq, OnLoginSuccessful, OnError);
    }
    void OnLoginSuccessful(LoginResult r)
    {
        if(currentUserName == "Guest")
        {
            var req = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = "Guest",
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(req, result => Debug.Log("Done"), OnError);
        }
        else if(currentDeviceType != deviceType.None)
        {
            var req = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = (currentDeviceType + deviceID.Substring(0,3) + " Player").ToString(),
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(req, result => Debug.Log(""), OnError);
        }
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