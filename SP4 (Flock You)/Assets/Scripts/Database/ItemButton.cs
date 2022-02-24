using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public void SetThisItem()
    {
        //get shop panel GO
        GameObject ShopPanel = transform.parent.parent.parent.gameObject;
        //to change the display according to which object selected
        ShopPanel.GetComponent<LocalShopManager>().ChangeStats(transform);
        //to set the item stats based on which object selected
        ShopPanel.GetComponent<ShopItemManager>().SetItemValues(transform.GetSiblingIndex());
        ShopPanel.GetComponent<ShopItemManager>().SetCurrentItem(transform);
    }
}
