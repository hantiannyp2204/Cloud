using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GameEvent : MonoBehaviour
{
    //1. Make this class a singleton
    public static GameEvent instance = null;

    //2. inputs needed functions and their parameters
    public UnityEvent <string,string,string> showClanDetails;
    public UnityEvent<string> AcceptClanJoining;
    public UnityEvent<string> DeclineClanJoining;
    public UnityEvent<PlayFab.GroupsModels.EntityKey> KickPlayer;
    private void Awake()
    {
        //1. make this class a singleton
        if(instance == null)
        {
            instance = this;
        }


        //3. init all the functions or esle it will be null
        showClanDetails = new();
        AcceptClanJoining = new();
        DeclineClanJoining = new();
        KickPlayer = new();
    }

    //4. head to the GO that contains the original fucntion (This case it is ClanInfo.cs, it has the fucntion that spawns the ClanInfo GO, which is what we want)
    //If the GO has no script, make a script for it

}
