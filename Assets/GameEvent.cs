using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GameEvent : MonoBehaviour
{
    //1. Make this class a singleton
    public static GameEvent instance = null;

    //2. inputs needed functions and their parameters
    public UnityEvent <string,string> showClanDetails;


    private void Awake()
    {
        //1. make this class a singleton
        if(instance == null)
        {
            instance = this;
        }


        //3. init all the functions or esle it will be null
        showClanDetails = new();
    }

    //4. head to the GO that will call the function (This case it is ClanPrefab.cs, it calls the function to spawn the Clan Info GO and edit it)
    //If the GO has no script, make a script for it

}
