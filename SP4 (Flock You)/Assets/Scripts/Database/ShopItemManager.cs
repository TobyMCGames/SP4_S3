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

    public void SetItemValues(int buttonID)
    {
        //set item variables
        ItemName = shopItemList.items[buttonID].sName;
        ItemCost = shopItemList.items[buttonID].iCost;
        ItemLevelReq = shopItemList.items[buttonID].iLevelReq;
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

    public void SetCurrentItem(Transform thisItem)
    {
        currentItem = thisItem;
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
