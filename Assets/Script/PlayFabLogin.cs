using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public void Start()
    {
        
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}