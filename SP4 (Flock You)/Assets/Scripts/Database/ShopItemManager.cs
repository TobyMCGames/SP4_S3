using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemManager : MonoBehaviour
{
    public ItemList shopItemList;
    private string ItemName;
    private int ItemCost;
    private int ItemLevelReq;

    public void SetItemValues(int buttonID)
    {
        //set item variables
        ItemName = shopItemList.items[buttonID].sName;
        ItemCost = shopItemList.items[buttonID].iCost;
        ItemLevelReq = shopItemList.items[buttonID].iLevelReq;
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
