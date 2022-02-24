using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;

public class Login : MonoBehaviour
{
    private string URL;
    public Text displayTxt; //must add using UnityEngine.UI
    public InputField if_username, if_password; //to link to the inpufields
    public GameObject LoadingStatus;

    private void Awake()
    {
        URL = GlobalSettings.BaseServer + "login.php";
    }

    private void Start()
    {
        LoadingStatus.SetActive(false);
    }

    [Header("From Selection Panel")]
    public Text PlayerNameDisplayTxt;

    public void DoLogin()
    { //to be invoked by button click
        if (if_username.text == "" || if_password.text == "")
            displayTxt.text = "Username or Password is Empty";
        else
        {
            StartCoroutine(SendLoginInfo());
        }

    }
    IEnumerator SendLoginInfo()
    {
        displayTxt.text = "Processing...";
        WWWForm form = new WWWForm();
        form.AddField("sUsername", if_username.text);
        form.AddField("sPassword", if_password.text);

        UnityWebRequest webreq = UnityWebRequest.Post(URL, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                displayTxt.text = webreq.downloadHandler.text;
                //GlobalSettings.Username = if_username.text;
                if (webreq.downloadHandler.text == "Login success")
                {
                    //Set player name
                    string playerName = if_username.text;
                    LoadingStatus.SetActive(true);
                    GlobalSettings.Username = playerName;
                    PhotonNetwork.LocalPlayer.NickName = playerName;
                    PlayerNameDisplayTxt.text = playerName;

                    //load player stats
                    transform.GetComponent<ReadPlayerStats>().LoadStats();
                }
                break;
            default:
                LoadingStatus.SetActive(false);
                displayTxt.text = "Server Error";
                break;
        }
    }
}
