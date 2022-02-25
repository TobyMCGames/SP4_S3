using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ReadPlayerStats : MonoBehaviour
{
    private string URL;
    public PlayerStats playerStats;

    public Text PlayerLevelTxt;
    public Text PlayerXPTxt;
    public Text PlayerCashtxt;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "readplayerstats.php";
    }

    public void LoadStats()
    {
        StartCoroutine(GetRequest(URL));
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
                    break;
            }
        }
    }

    void Deserialize(String RawJSON)
    {
        playerStats = JsonUtility.FromJson<PlayerStats>(RawJSON); //convert raw json to objects

        for (int a = 0; a < playerStats.stats.Count; a++)
        {
            //set cash
            GlobalSettings.Cash = playerStats.stats[a].cash;
            PlayerCashtxt.text = "Cash: $" + playerStats.stats[a].cash;

            //get level
            GlobalSettings.Level = playerStats.stats[a].level;
            PlayerLevelTxt.text = "Lvl " + playerStats.stats[a].level;

            //set stats
            GlobalSettings.XP = playerStats.stats[a].xp;
            GlobalSettings.XPToLevelUp = GlobalSettings.BaseXPToLevelUp * GlobalSettings.Level;
            PlayerXPTxt.text = "Exp:  " + playerStats.stats[a].xp + "/" + GlobalSettings.XPToLevelUp;
        }

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
public class PlayerStats
{
    public List<OneStat> stats = new List<OneStat>();
}