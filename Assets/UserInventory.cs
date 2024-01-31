using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.EconomyModels;
using PlayFab.ClientModels;
using System;
using TMPro;

public class UserInventory : MonoBehaviour
{
    [SerializeField]
    GameObject lockedPng;
    [SerializeField]
    TMP_Text currentGold;
    [SerializeField]
    TMP_Text nameTag;
    [SerializeField]
    TMP_Text priceTag;
    int currentGoldIndex;

    int currentShipPrice;
    // Start is called before the first frame update
    void Start()
    {
        findShip();
        getGold();
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("shipExist") == 1)
        {
            lockedPng.SetActive(false);
        }
        else
        {
            lockedPng.SetActive(true);
        }
    }
    void getGold()
    {
        var request = new GetUserInventoryRequest();

        PlayFabClientAPI.GetUserInventory(request, result => {
            foreach (var currency in result.VirtualCurrency)
            {
                if (currency.Key == "GD")
                {
                    currentGoldIndex = currency.Value;
                }
                else
                {
                    currentGoldIndex = 0;
                }
            }
            currentGold.text = "Gold: " + currentGoldIndex;
        }, result => Debug.Log("Error"));
    }    

    public void findShip() {
        getShipInInv(PlayerPrefs.GetString("choosenShip"));
    }
    void getShipInInv(string shipID)
    {

        // Specify the request details
        var request = new GetUserInventoryRequest { };

        
        // Send the request to get the user's inventory
        PlayFabClientAPI.GetUserInventory(request, result => {
            // Player's inventory has been successfully retrieved
            if (shipID != "default")
            {
                PlayerPrefs.SetInt("shipExist", 0);
                //To show current plane's name and price
                var request = new GetCatalogItemsRequest();

                // Send the request to get information about items in the catalog
                PlayFabClientAPI.GetCatalogItems(request, result =>
                {
                    // Find the specific item by ID in the catalog
                    PlayFab.ClientModels.CatalogItem catalogItem = result.Catalog.Find(item => item.ItemId == shipID);

                    if (catalogItem != null)
                    {
                        //show current plane's name
                        nameTag.text = $"{catalogItem.DisplayName}";
                        Debug.Log($"Item ID: {catalogItem.ItemId}");
                        Debug.Log($"Display Name: {catalogItem.DisplayName}");

                        // Find the price
                        if (catalogItem.VirtualCurrencyPrices != null)
                        {
                            foreach (var currencyPrice in catalogItem.VirtualCurrencyPrices)
                            {
                                //make sure the currency used is gold(in-game)
                                if (currencyPrice.Key == "GD")
                                {
                                    //show current plane's price
                                    priceTag.text = $"{currencyPrice.Key}: {currencyPrice.Value}";
                                    Debug.Log($"Price in {currencyPrice.Key}: {currencyPrice.Value}");
                                    currentShipPrice = int.Parse(currencyPrice.Value.ToString());
                                }
                            }
                        }
                    }
                },
                result => Debug.Log("Error"));

                //now loop through inventory to check
                foreach (var item in result.Inventory)
                {
                    //check if have ship
                    if (item.ItemId == shipID)
                    {
                        PlayerPrefs.SetInt("shipExist", 1);
                    }
                }

            }
            else
            {
                //if deafult ship is selected
                PlayerPrefs.SetInt("shipExist", 1);
            }

        }, OnError);
    }
    
    public void PurchasePlane()
    {
        PurchaseItemRequest req = new PurchaseItemRequest { CatalogVersion= "main", ItemId = PlayerPrefs.GetString("choosenShip"), VirtualCurrency = "GD", Price = currentShipPrice};


        PlayFabClientAPI.PurchaseItem(req, result => { Debug.Log("Purchased " + req.ItemId); findShip(); getGold(); }, OnError);
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error " + error.GenerateErrorReport());
    }
    public void AddCurrency(int amount)
    {
        // Specify the request details
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GD",
            Amount = amount
        };

        // Send the request to add currency to the player
        PlayFabClientAPI.AddUserVirtualCurrency(request, result=>Debug.Log(amount + "added, " + result.Balance +" is the new blaance"), OnError);
    }
}
