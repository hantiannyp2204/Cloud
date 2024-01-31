using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmTradeUI : MonoBehaviour
{
    [SerializeField] TMP_Text ItemList;
    [SerializeField] TMP_Text SendTarget;



    // Update is called once per frame
    public void showGiftsToSend(List<string> giftNamelist)
    {
        foreach(var gift in giftNamelist)
        {
            ItemList.text += gift + "\n";
        }
    }
    private void OnEnable()
    {
        ItemList.text = "";
    }
}
