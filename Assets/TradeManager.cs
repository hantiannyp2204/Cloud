using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using TMPro;
using UnityEngine.Events;

public class TradeManager : MonoBehaviour
{
    [SerializeField] Transform itemDisplayContent;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject confirmGiftUI;
    List<string> GiftIDList;
    List<string> GiftNameList;
    // Update is called once per frame
    void getInventoryItems()
    {
        //Reset the gift list
        GiftIDList = new List<string>();
        GiftNameList = new List<string>();
        //claer all existing prefabs first
        for (int i = itemDisplayContent.childCount - 1; i >= 0; i--)
        {
            Transform child = itemDisplayContent.GetChild(i);
            Destroy(child.gameObject);
        }
        // Specify the request details
        var InvReq = new GetUserInventoryRequest { };

        // Send the request to get the user's inventory
        PlayFabClientAPI.GetUserInventory(InvReq, result =>
        { 
            foreach(var item in result.Inventory)
            {
                GameObject InvObj =  Instantiate(itemPrefab, itemDisplayContent);
                InvObj.transform.Find("Item name").GetComponent<TMP_Text>().text = item.DisplayName;
                InvObj.transform.Find("Item cost").GetComponent<TMP_Text>().text = "Value: " + item.UnitPrice.ToString()+"GD";
                InvObj.transform.GetComponent<GiftingSystem>().ItemID = item.ItemInstanceId;
                InvObj.transform.GetComponent<GiftingSystem>().ItemName = item.DisplayName;
            }
        }
        ,result => Debug.Log(result));
    }

    public void AddGiftToList(string GiftId,string GiftName)
    {
        GiftIDList.Add(GiftId);
        GiftNameList.Add(GiftName);

    }
    public void sendGift()
    {
        GameEvent.instance.sendGift.Invoke(GiftIDList);

    }
    public void RemoveGiftFromList(string GiftID)
    {
        int index = 0;

        foreach(var giftID in GiftIDList)
        {
            if(giftID == GiftID)
            {
                //remove from giftID list
                GiftIDList.RemoveAt(index);
                //remove from GiftName list
                GiftNameList.RemoveAt(index);
                break;
            }
            index++;
        }

    }
    public void ShowConfirmGiftUI()
    {
        confirmGiftUI.SetActive(true);
        confirmGiftUI.GetComponent<ConfirmTradeUI>().showGiftsToSend(GiftNameList);
    }
    public void HideConfirmGiftUI()
    {
        confirmGiftUI.SetActive(false);
    }
    private void OnEnable()
    {
        HideConfirmGiftUI();
        getInventoryItems();
     
        //events
        GameEvent.instance.addGiftToList.AddListener(AddGiftToList);
        GameEvent.instance.removeGiftFromList.AddListener(RemoveGiftFromList);
    }
    private void OnDisable()
    {
        GameEvent.instance.addGiftToList.RemoveListener(AddGiftToList);
        GameEvent.instance.removeGiftFromList.RemoveListener(RemoveGiftFromList);
    }
}
