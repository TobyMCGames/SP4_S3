using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public void SetThisItem()
    {
        //get shop panel GO
        GameObject ShopPanel = transform.parent.parent.parent.gameObject;
        ShopPanel.GetComponent<ShopItemManager>().SetCurrentItem(transform);
    }
}
