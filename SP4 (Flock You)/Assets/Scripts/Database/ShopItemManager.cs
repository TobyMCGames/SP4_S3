using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemManager : MonoBehaviour
{
    public ItemList shopItemList;
    public OwnedItemList ownedItemList;
    private string ItemName;
    private int ItemCost;
    private int ItemLevelReq;
    private string ItemType;
    private Transform currentItem;

    [Header("Shop Panel Objects")]
    public GameObject ShopList;
    public Button buyButton;
    public Button equipButton;
    public Text ItemNameText;
    public Text ItemCostText;
    public Text LockedStatus;

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

    public void ChangeSkinPage()
    {
        ChangePage("skin");
    }

    public void ChangePerksPage()
    {
        ChangePage("perk");
    }

    public void ChangePage(string itemtype)
    {
        bool firstitem = false;
        for (int j = 0; j < ShopList.transform.childCount; j++)
        {
            for (int a = 0; a < shopItemList.items.Count; a++)
            {
                if (shopItemList.items[a].sType != itemtype)
                {
                    //set all those not the same type to not show
                    ShopList.transform.GetChild(a).gameObject.SetActive(false);
                }
                else
                {
                    //set all those of same type to show
                    ShopList.transform.GetChild(a).gameObject.SetActive(true);
                    if(!firstitem)
                    {
                        SetCurrentItem(ShopList.transform.GetChild(a));
                        firstitem = true;
                    }
                }
            }
        }
    }


    public void SetItemValues(int buttonID)
    {
        //set item variables
        ItemName = shopItemList.items[buttonID].sName;
        ItemCost = shopItemList.items[buttonID].iCost;
        ItemLevelReq = shopItemList.items[buttonID].iLevelReq;
        ItemType = shopItemList.items[buttonID].sType;

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

    public string GetItemType()
    {
        return ItemType;
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
