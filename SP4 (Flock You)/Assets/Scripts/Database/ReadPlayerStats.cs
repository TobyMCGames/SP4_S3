using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ReadPlayerStats : MonoBehaviour
{
    public string URL;
    public OneStat playerStats;

    public Text PlayerLevelTxt;
    public Text PlayerXPTxt;
    public Text PlayerCashtxt;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "readShopItems.php";
    }

    public void LoadStats()
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
        playerStats = JsonUtility.FromJson<OneStat>(RawJSON); //convert raw json to objects

        //set global settings
        GlobalSettings.Cash = playerStats.cash;
        GlobalSettings.XP = playerStats.xp;
        GlobalSettings.Level = playerStats.level;

        //set display variables
        PlayerCashtxt.text = "Cash: $" + playerStats.cash;
        PlayerXPTxt.text = "Exp:  " + playerStats.xp;
        PlayerLevelTxt.text = "Lvl " + playerStats.level;

        //load shop
        transform.GetComponent<ReadShopItems>().LoadShop();
    }
}

[Serializable]
public class OneStat
{
    public int cash;
    public int xp;
    public int level;
}