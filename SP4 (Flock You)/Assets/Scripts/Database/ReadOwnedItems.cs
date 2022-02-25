using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Photon.Pun;

public class ReadOwnedItems : MonoBehaviour
{
    private string URL;
    public GameObject ShopList;
    public GameObject ShopPanel;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "readowneditems.php";
    }

    public void LoadOwneditems()
    { //a function to allow reload
        StartCoroutine(GetRequest(URL)); //load data
    }

    IEnumerator GetRequest(string uri)
    {
        WWWForm form = new WWWForm();
        form.AddField("sUsername", GlobalSettings.Username);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form))
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

                    //photon connect to server
                    if (!PhotonNetwork.IsConnected)
                    {
                        PhotonNetwork.ConnectUsingSettings();
                    }
                    GlobalSettings.curMode = GlobalSettings.GAMEMODES.MULTIPLAYER; // Player playing online
                    break;
            }
        }
        
    }


    void Deserialize(String RawJSON)
    {
        ShopPanel.GetComponent<ShopItemManager>().ownedItemList = JsonUtility.FromJson<OwnedItemList>(RawJSON);
        OwnedItemList oi = ShopPanel.GetComponent<ShopItemManager>().ownedItemList;

        for (int i = 0; i < oi.ownedItemList.Count; i++)
        {
            OneOwnedItem ownedItem = oi.ownedItemList[i];
            for (int j = 0; j < ShopList.transform.childCount; j++)
            {
                //get the item from the shop list
                GameObject shopItem = ShopList.transform.GetChild(j).gameObject;
                //check if item name is inside the database
                if (shopItem.transform.GetChild(0).GetComponentInChildren<Text>().text == ownedItem.sItemName)
                {
                    //change the cost panel to either equipped or owned
                    GameObject CostPanel = shopItem.transform.GetChild(3).gameObject;
                    CostPanel.transform.GetChild(0).GetComponent<Text>().color = Color.green;
                    //if item eqipped:
                    if (ownedItem.bEquipped)
                    {
                        CostPanel.transform.GetChild(0).GetComponent<Text>().text = "Equipped";
                        if(ownedItem.sItemType == "skin")
                        {
                            GlobalSettings.CurrentSkin = ownedItem.sItemName;
                        }
                    }
                    else
                        CostPanel.transform.GetChild(0).GetComponent<Text>().text = "Owned";
                    break;
                }
            }
        }
    }
}

[Serializable]
public class OneOwnedItem
{
    public string sItemName;
    public bool bEquipped;
    public string sItemType;
}
[Serializable]
public class OwnedItemList
{
    public List<OneOwnedItem> ownedItemList = new List<OneOwnedItem>();
}

