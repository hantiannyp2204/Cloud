using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GiftingSystem : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image ItemImage;
    [SerializeField] GameObject giftIcon;

    bool gifted = false;
    public string ItemID;
    public string ItemName;
    // Start is called before the first frame update
    void Start()
    {
        ResetGiftSend();
    }
    public void ResetGiftSend()
    {
        Color imageColor = ItemImage.color;
        imageColor.a =1;
        ItemImage.color = imageColor;

        giftIcon.SetActive(false);

        gifted = false;
    }
    public void toggleGifting()
    {
        gifted = !gifted;
        if (gifted)
        {
            Color imageColor = ItemImage.color;
            imageColor.a = 0.5f;
            ItemImage.color = imageColor;
            giftIcon.SetActive(true);
            GameEvent.instance.addGiftToList.Invoke(ItemID, ItemName);
        }
        else
        {
            Color imageColor = ItemImage.color;
            imageColor.a = 1;
            ItemImage.color = imageColor;

            giftIcon.SetActive(false);
            GameEvent.instance.removeGiftFromList.Invoke(ItemID);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
