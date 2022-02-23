using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ReadShopItems : MonoBehaviour
{
    public string URL;
    public GameObject ShopList; //must add using UnityEngine.UI and using UnityEngine.Networking on top
    public GameObject ItemIconPrefab;
    public ItemList itemList;
    public GameObject ShopPanel;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "readShopItems.php";
    }

    public void LoadShop()
    { //a function to allow reload
        StartCoroutine(GetRequest(URL)); //load data
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    Deserialize(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    void Deserialize(String RawJSON)
    {
        itemList = JsonUtility.FromJson<ItemList>(RawJSON); //convert raw json to objects
                                                            //OPTIONAL: to show its really broken down into separate objects

        for (int a = 0; a < itemList.items.Count; a++)
        {
            GameObject item;
            if (ShopList.transform.childCount == a)
            {
                item = Instantiate(ItemIconPrefab, ShopList.transform);
            }
            else
                item = ShopList.transform.GetChild(a).gameObject;

            //set item name
            item.transform.GetChild(0).GetComponentInChildren<Text>().text = itemList.items[a].sName;
            //set item cost
            item.transform.GetChild(3).GetComponentInChildren<Text>().text = "$" + itemList.items[a].iCost;
            //get lockPanel
            Transform LockPanel = item.transform.GetChild(2);
            LockPanel.GetChild(0).GetComponent<Text>().text = "Unlocked at lvl: " + itemList.items[a].iLevelReq;
            //set item to false if its not on first page
            //if(itemList.items[a].sType != GlobalSettings.ShopFirstPage)
            //{
            //    item.SetActive(false);
            //}
            if (GlobalSettings.Level >= itemList.items[a].iLevelReq) //if higher than lock level, then undisplay unlocked
            {
                LockPanel.gameObject.SetActive(false);
            }
        }

        ShopPanel.transform.GetComponent<LocalShopManager>().DisplayFirstItem();
    }
}

[Serializable]
public class OneItem
{
    public string sName;
    public string sType;
    public int iCost;
    public int iLevelReq;
}
[Serializable]
public class ItemList
{
    public List<OneItem> items = new List<OneItem>();
}