using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClanInfo : MonoBehaviour
{
    [SerializeField] GameObject clanInfo;
    [SerializeField] TMP_Text clanName;
    [SerializeField] TMP_Text clanDescription;

    //5. Create OnEnabled and Disabled to prevent errors
    private void OnEnable()
    {
        //7. Add listener to our made function (This is basically refrencing this script to the GameEvent.cs)
        //If the parameters matches, you will have no errors

        //NOTE:If theres a null refeence in this line, it is becos the GameEvent class ran too slow, it should be pirorities before anything else run
        //First head to: Edit -> Project Settings -> Script Execution Order
        //Press on the + icon and find "GameEvent" or what ever you named the .cs
        //Drag it all the way up, to before the block "Default Time"
        GameEvent.instance.showClanDetails.AddListener(showClanDetails);
        //9. go to the GO that will trigger the funciton
        //we want showClanDetails() to run when the ClanPrefab is pressed, thus the GO is the ClanPrefab Prefab, make a .cs file if don't exist on it
    }
    private void OnDisable()
    {
        //8. Remove listener to our made function (This is basically unfrefrencing this script to the GameEvent.cs, this prevent errors, just do this)
        GameEvent.instance.showClanDetails.RemoveListener(showClanDetails);
    }

    //6. Make a somewhat "duplicated" fucntion that mirrors the fucntions that was created in the GameEvent.cs
    //It should have parameters that mimics what was needed
    //public UnityEvent <string,string> showClanDetails was made, thus we need "String String" in parameter
    void showClanDetails(string _clanName, string _clanDesription)
    {
        clanInfo.SetActive(true);
        clanName.text = _clanName;
        clanDescription.text = _clanDesription;
    }
}
