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
    public Text playerCashText;
    public GameObject BuyButton;
    public GameObject EquipButton;

    private float purchaseDisplayTimer = 0;
    private bool startDisplayCountdown;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "buyitem.php";
    }

    private void Update()
    {
        if(startDisplayCountdown)
        {
            purchaseDisplayTimer += Time.deltaTime;
            if(purchaseDisplayTimer >= 1.5f)
            {
                purchaseStatus.text = "";
            }
        }
    }

    public void DoBuyItem()
    {
        int Difference = GlobalSettings.Cash - (ShopPanel.GetComponent<ShopItemManager>().GetItemCost());
        if (Difference < 0)
            purchaseStatus.text = "Not Enough Cash!";
        else
            StartCoroutine(SenditemInfo());
    }

    public void UpdateCashDisplay()
    {
        playerCashText.text = "Cash: $" + GlobalSettings.Cash;
    }

    IEnumerator SenditemInfo()
    {
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
                //update player cash locally
                GlobalSettings.Cash -= ShopPanel.GetComponent<ShopItemManager>().GetItemCost();
                ShopPanel.GetComponent<ShopItemManager>().SetItembought();
                UpdateCashDisplay();
                purchaseStatus.text = webreq.downloadHandler.text;
                startDisplayCountdown = true;
                BuyButton.SetActive(false);
                EquipButton.SetActive(true);
                break;
            default:
                purchaseStatus.text = "Server Error";
                Debug.LogError("Error: Cannot purchase Item! Server Error");
                break;
        }
    }
}

