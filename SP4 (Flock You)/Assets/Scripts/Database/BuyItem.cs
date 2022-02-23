using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class BuyItem : MonoBehaviour
{
    private string URL;
    public Text purchaseStatus;
    public GameObject ShopPanel;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "buyitem.php";
    }

    public void DoBuyItem()
    {
        int Difference = GlobalSettings.Cash - (ShopPanel.GetComponent<ShopItemManager>().GetItemCost());
        if (Difference < 0)
            purchaseStatus.text = "Not Enough Cash!";
        else
            StartCoroutine(SendLoginInfo());
    }

    IEnumerator SendLoginInfo()
    {
        //update player cash locally
        GlobalSettings.Cash -= ShopPanel.GetComponent<ShopItemManager>().GetItemCost();
        purchaseStatus.text = "Processing...";

        //update player inv and cash on datatbase
        WWWForm form = new WWWForm();
        form.AddField("sUsername", GlobalSettings.Username);
        form.AddField("itemname", ShopPanel.GetComponent<ShopItemManager>().GetItemName());
        form.AddField("itemcost", ShopPanel.GetComponent<ShopItemManager>().GetItemCost());

        UnityWebRequest webreq = UnityWebRequest.Post(URL, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                purchaseStatus.text = webreq.downloadHandler.text;
                break;
            default:
                purchaseStatus.text = "Server Error";
                Debug.LogError("Error: Cannot purchase Item! Server Error");
                break;
        }
    }
}

