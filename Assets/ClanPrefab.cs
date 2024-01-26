using PlayFab.ProfilesModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class ClanPrefab : MonoBehaviour
{
    public string ClanName; 
    public string ClanDescription = "This clan has no description";
    public string ClanGroupID;
    
    //10. make a funciton that will run the function
   public void RevealClanInfo()
    {
        //Fill in the Invoke's parameter with the needed parameter, in this case the name and description of the clan
        GameEvent.instance.showClanDetails.Invoke(ClanName, ClanDescription, ClanGroupID);
    }

}
