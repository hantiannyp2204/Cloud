using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingTrades : MonoBehaviour
{

    void GetTrades()
    {
        var GetTradesReq = new GetPlayerTradesRequest { };
        PlayFabClientAPI.GetPlayerTrades(GetTradesReq, result => {
            //loop through all trades in playfab
            Debug.Log(result.OpenedTrades.Count);
            foreach (var gift in result.OpenedTrades)
            {
                Debug.Log("Got gift");
                string giftedItemsString = "";
            
                //loop through all items in the trades
                foreach (var giftItems in gift.OfferedCatalogItemIds)
                {
                    giftedItemsString += giftItems + " ";
                }
                //trades for you
                if (gift.OfferingPlayerId[0].ToString() == MyPlayFab.Instance.myPlayFabMasterID)
                {
                    //gets all the items
                    Debug.Log("Gift from " + gift.OfferingPlayerId + " | Items: " + giftedItemsString);
                }
            }
          
        }, result => Debug.Log(result));
    }
    private void OnEnable()
    {
        GetTrades();
    }
}
