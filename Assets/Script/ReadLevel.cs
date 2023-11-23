using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class ReadLevel : MonoBehaviour
{
    [SerializeField]
    PlayerStatsUI playerStatsUI;
    [SerializeField]
    PlayerStats playerStats;

    
}
[System.Serializable]
public class JSListWrapper<T>
{
    public List<T> list;
    public JSListWrapper(List<T> list) => this.list = list;
}
