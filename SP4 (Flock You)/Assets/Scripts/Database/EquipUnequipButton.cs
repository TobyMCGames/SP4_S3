using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EquipUnequipButton : MonoBehaviour
{
    public GameObject buyButton;
    private bool isEquipped;
    public GameObject ShopPanel;

    private Transform selectedItem;
    private string selectedItemName;
    private bool runOnce; //this is so scuffed wtf
    private string URL;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "equipunequipitem.php";
    }

    public void SetEquipStatus(Transform itemSelected)
    {
        runOnce = true;
        selectedItem = itemSelected;
        selectedItemName = ShopPanel.GetComponent<ShopItemManager>().GetItemName();

        Text itemStatus = itemSelected.GetChild(3).GetComponentInChildren<Text>();
        if (itemStatus.text == "Owned")
        {
            gameObject.SetActive(true);
            buyButton.SetActive(false);
            isEquipped = false;
            GetComponentInChildren<Text>().text = "Equip";
        }
        else if (itemStatus.text == "Equipped")
        {
            gameObject.SetActive(true);
            buyButton.SetActive(false);
            isEquipped = true;
            GetComponentInChildren<Text>().text = "Unequip";
        }
        else
        {
            buyButton.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void EquipUnequipItem()
    {
        if (runOnce)
            UpdateEquipStatus(selectedItemName, !isEquipped);
        runOnce = false;
    }

    public void UpdateEquipStatus(string itemname, bool bEquipped)
    {
        StartCoroutine(SendInfo(itemname, bEquipped));
    }

    IEnumerator SendInfo(string itemname, bool bEquipped)
    {
        if (bEquipped)
            Debug.Log("Equipping Item...");
        else
            Debug.Log("Unequipping Item...");

        WWWForm form = new WWWForm();
        form.AddField("sUsername", GlobalSettings.Username);
        form.AddField("itemname", itemname);
        form.AddField("bEquipped", bEquipped.GetHashCode());

        UnityWebRequest webreq = UnityWebRequest.Post(URL, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log(webreq.downloadHandler.text);
                if(bEquipped)
                {
                    selectedItem.GetChild(3).GetComponentInChildren<Text>().text = "Equipped";
                    GetComponentInChildren<Text>().text = "Unequip";
                    ShopPanel.GetComponent<ShopItemManager>().SetCurrentItem(selectedItem);
                }
                else
                {

                    selectedItem.GetChild(3).GetComponentInChildren<Text>().text = "Owned";
                    GetComponentInChildren<Text>().text = "Equip";
                    ShopPanel.GetComponent<ShopItemManager>().SetCurrentItem(selectedItem);
                }

                break;
            default:
                Debug.LogError("Error: cannot equip item, Server Error");
                break;
        }
    }
}
