using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UpdatePlayerStats : MonoBehaviour
{
    private string URL, URL2;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "updateplayerstats.php";
        URL2 = GlobalSettings.BaseServer + "levelup.php";
    }

    public void UpdateStats(int xp, int cash)
    { 
        StartCoroutine(SendStatsInfo(xp, cash));
        //check if enough to level up
        if ((GlobalSettings.XP + xp) > GlobalSettings.XPToLevelUp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        StartCoroutine(SendUpdateLevel());
    }

    IEnumerator SendStatsInfo(int xp, int cash)
    {
        Debug.Log("Sending Player Stats...");
        WWWForm form = new WWWForm();
        form.AddField("sUsername", GlobalSettings.Username);
        form.AddField("xp", xp);
        form.AddField("cash", cash);

        UnityWebRequest webreq = UnityWebRequest.Post(URL, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log(webreq.downloadHandler.text);
                break;
            default:
                Debug.LogError("Error: cannot update player stats! (Server Error)");
                break;
        }
    }


    IEnumerator SendUpdateLevel()
    {
        Debug.Log("Sending level up info...");
        WWWForm form = new WWWForm();
        form.AddField("sUsername", GlobalSettings.Username);

        UnityWebRequest webreq = UnityWebRequest.Post(URL2, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log(webreq.downloadHandler.text);
                break;
            default:
                Debug.LogError("Error: cannot update player level! (Server Error)");
                break;
        }
    }
}
