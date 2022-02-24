using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EquipUnequipItem : MonoBehaviour
{
    private string URL;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "equipunequipitem.php";
    }

    public void UpdateEquipStatus(string itemname, bool bEquipped)
    {
        StartCoroutine(SendInfo(itemname, bEquipped));
    }

    IEnumerator SendInfo(string itemname, bool bEquipped)
    {
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
                break;
            default:
                Debug.LogError("Error: cannot equip item, Server Error");
                break;
        }
    }
}
