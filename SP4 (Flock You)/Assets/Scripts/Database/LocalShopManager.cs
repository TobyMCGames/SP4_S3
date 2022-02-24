using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalShopManager : MonoBehaviour
{

    // !!-----Script not in use anymore can delete later-----!!

    public GameObject ShopList;
    public Button buyButton;
    public Button equipButton;
    public Text ItemName;
    public Text ItemCost;
    public Text LockedStatus;


    //displays first item in the shop (calls everytime shop is opened)
    public void DisplayFirstItem()
    {
        Transform firstItem = ShopList.transform.GetChild(0);
        ChangeStats(firstItem);
    }

    //changes display based on which item selected
    public void ChangeStats(Transform thisItem)
    {
        //get item's name
        ItemName.text = thisItem.GetChild(0).GetComponentInChildren<Text>().text;
        //get item's cost
        ItemCost.text = thisItem.GetChild(3).GetComponentInChildren<Text>().text;
        //get item's lock status

        if (thisItem.GetChild(2).gameObject.activeSelf == true)
        {
            LockedStatus.text = "Locked";
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            LockedStatus.text = "";
        }
    }

}
