using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemManager : MonoBehaviour
{
    public ItemList shopItemList;
    private string ItemName;
    private int ItemCost;
    private int ItemLevelReq;
    private Transform currentItem;

    [Header("Shop Panel Objects")]
    public GameObject ShopList;
    public Button buyButton;
    public Button equipButton;
    public Text ItemNameText;
    public Text ItemCostText;
    public Text LockedStatus;

    public void DisplayFirstItem()
    {
        Transform currentItem = ShopList.transform.GetChild(0);
        SetCurrentItem(currentItem);
    }

    //changes display based on which item selected
    public void ChangeDisplay()
    {
        //get item's name
        ItemNameText.text = currentItem.GetChild(0).GetComponentInChildren<Text>().text;
        //get item's cost
        ItemCostText.text = currentItem.GetChild(3).GetComponentInChildren<Text>().text;
        //get item's lock status
        if (currentItem.GetChild(2).gameObject.activeSelf == true)
        {
            LockedStatus.text = "Locked";
            buyButton.enabled = false;
        }
        else
        {
            LockedStatus.text = "";
            buyButton.enabled = true;
        }
    }


    public void SetItemValues(int buttonID)
    {
        //set item variables
        ItemName = shopItemList.items[buttonID].sName;
        ItemCost = shopItemList.items[buttonID].iCost;
        ItemLevelReq = shopItemList.items[buttonID].iLevelReq;

        GlobalSettings.itemname = ItemName;
    }
    
    public void SetItembought()
    {
        Text itemStatus = currentItem.GetChild(3).GetComponentInChildren<Text>();
        if(itemStatus.text != "Owned" || itemStatus.text != "Equipped")
        {
            itemStatus.color = Color.green;
            itemStatus.text = "Owned";
        }
    }

    public void SetEquippedStatus()
    {
        equipButton.GetComponent<EquipUnequipButton>().SetEquipStatus(currentItem);
    }

    public void SetCurrentItem(Transform thisItem)
    {
        currentItem = thisItem;
        ChangeDisplay();
        SetItemValues(currentItem.GetSiblingIndex());
        SetEquippedStatus();
    }

    public Transform GetCurrentItem()
    {
        return currentItem;
    }

    public string GetItemName()
    {
        return ItemName;
    }

    public int GetItemCost()
    {
        return ItemCost;
    }

    public int GetItemLevelReq()
    {
        return ItemLevelReq;
    }
}
