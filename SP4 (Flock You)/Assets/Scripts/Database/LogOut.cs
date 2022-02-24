using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;

public class LogOut : MonoBehaviour
{
    private string URL;
    public Text displayTxt; 
    public GameObject ShopList;

    [Header("Panels")]
    public GameObject LoginPanel;
    public Text loadingStatus;
    public Text LoginStatus;
    public GameObject SelectionPanel;

    void Awake()
    {
        URL = GlobalSettings.BaseServer + "logout.php";
    }

    public void DoLogOut()
    { //to be invoked by button click
       StartCoroutine(SendLogOutInfo());
    }

    IEnumerator SendLogOutInfo()
    {
        displayTxt.text = "Logging Out...";
        WWWForm form = new WWWForm();
        form.AddField("sUsername", GlobalSettings.Username);

        UnityWebRequest webreq = UnityWebRequest.Post(URL, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                displayTxt.text = webreq.downloadHandler.text;
                for (int a = 0; a < ShopList.transform.childCount; a++)
                {
                    //unload all items from shop list
                    Destroy(ShopList.transform.GetChild(a).gameObject);
                }
                //switch back to login panel
                PhotonNetwork.Disconnect();
                SwitchToLogin();
                break;
            default:
                displayTxt.text = "Error: Failed to log out! (Server Error)";
                break;
        }
    }

    private void SwitchToLogin()
    {
        SelectionPanel.SetActive(false);
        LoginPanel.SetActive(true);
        loadingStatus.gameObject.SetActive(false);
        LoginStatus.text = "";
    }
}
