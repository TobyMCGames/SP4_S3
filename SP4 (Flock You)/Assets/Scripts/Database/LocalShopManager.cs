using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalShopManager : MonoBehaviour
{
    public GameObject ShopList;
    public Text ItemName;
    public Text ItemCost;
    public Text LockedStatus;


    public void DisplayFirstItem()
    {
        Transform firstItem = ShopList.transform.GetChild(0);
        //get item's name
        ItemName.text = firstItem.GetChild(0).GetComponentInChildren<Text>().text;
        //get item's cost
        ItemCost.text = "Cost: " + firstItem.GetChild(3).GetComponentInChildren<Text>().text;
        //get item's lock status
        if (firstItem.GetChild(2).gameObject.activeSelf == true)
            LockedStatus.text = "Locked";
        else
            LockedStatus.text = "";
    }

    public void ChangeStats(Transform thisItem)
    {
        //get item's name
        ItemName.text = thisItem.GetChild(0).GetComponentInChildren<Text>().text;
        //get item's cost
        ItemCost.text = "Cost: " + thisItem.GetChild(3).GetComponentInChildren<Text>().text;
        //get item's lock status
        if (thisItem.GetChild(2).gameObject.activeSelf == true)
            LockedStatus.text = "Locked";
        else
            LockedStatus.text = "";
    }
}
