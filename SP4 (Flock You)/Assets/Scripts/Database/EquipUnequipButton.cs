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
    public GameObject ShopList;

    private Transform selectedItem;
    private Transform previousEquipItem;
    private string selectedItemName;
    ItemList shopItemList;
    private string URL;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "equipunequipitem.php";
        shopItemList = ShopPanel.GetComponent<ShopItemManager>().shopItemList;
    }

    public void SetEquipStatus(Transform itemSelected)
    {
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
        //fi its a skin, unequip all skins
        bool thisitemEquipped = false;
        for (int j = 0; j < ShopList.transform.childCount; j++)
        {
            for (int a = 0; a < shopItemList.items.Count; a++)
            {
                //check if item is a skin
                if (shopItemList.items[a].sType == "skin")
                {
                    //check if the item is equipped
                    Transform shopItem = ShopList.transform.GetChild(a);
                    if (shopItem.GetChild(3).GetComponentInChildren<Text>().text == "Equipped")
                    {
                        if(!thisitemEquipped)
                        {
                            Debug.Log("This item is equipped: " + shopItem.GetChild(0).GetComponentInChildren<Text>().text);
                            UpdateEquipStatus(shopItem.GetChild(0).GetComponentInChildren<Text>().text, false);
                            shopItem.GetChild(3).GetComponentInChildren<Text>().text = "Owned";
                            previousEquipItem = shopItem;
                            thisitemEquipped = true;
                        }
                    }

                }
            }
        }

        //equip or enequip the item
        UpdateEquipStatus(selectedItemName, !isEquipped);
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

                if (bEquipped)
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

                    if(previousEquipItem != null)
                        previousEquipItem.GetChild(3).GetComponentInChildren<Text>().text = "Owned";
                }

                break;
            default:
                Debug.LogError("Error: cannot equip item, Server Error");
                break;
        }
    }
}
