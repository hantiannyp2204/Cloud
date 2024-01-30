using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;

public class MyPlayFab : MonoBehaviour
{
    public string myPlayFabMasterID;
    public string myPlayFabTitleID;
    public static MyPlayFab Instance { get;private set; }

    private void Awake()
    {
     
        if (Instance != null && Instance!= this)
        {
            Destroy(gameObject);
        }
        else
        {
            //our new Start fucniton (init)
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void SetAccountInfos()
    {
        GetPlayerProfileRequest();
        GetUserAccountInfo();

    }
    void GetPlayerProfileRequest()
    {
        var request = new GetPlayerProfileRequest
        {
        };

        PlayFabClientAPI.GetPlayerProfile(request, result => { myPlayFabMasterID = result.PlayerProfile.PlayerId; Debug.Log(myPlayFabMasterID); }, Errorresult => { Debug.Log(Errorresult); });
    }
    void GetUserAccountInfo()
    {
        var request = new GetAccountInfoRequest
        {
            
        };

        PlayFabClientAPI.GetAccountInfo(request, result => {  myPlayFabTitleID = result.AccountInfo.TitleInfo.TitlePlayerAccount.Id; }, 
            Errorresult => { Debug.Log(Errorresult); });
    }
}
