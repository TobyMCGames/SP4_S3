using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public string URL = GlobalSettings.BaseServer + "login.php";
    public Text displayTxt; //must add using UnityEngine.UI
    public InputField if_username, if_password; //to link to the inpufields
    public void SendUserNamePassword()
    { //to be invoked by button click
        if (if_username.text == "" || if_password.text == "")
            displayTxt.text = "Username or Password is Empty";
        else
        {
            StartCoroutine(DoSendUserNamePassword());
        }

    }
    IEnumerator DoSendUserNamePassword()
    {
        WWWForm form = new WWWForm();
        form.AddField("sUsername", if_username.text);
        form.AddField("sPassword", if_password.text);

        UnityWebRequest webreq = UnityWebRequest.Post(URL, form);
        yield return webreq.SendWebRequest();
        switch (webreq.result)
        {
            case UnityWebRequest.Result.Success:
                displayTxt.text = webreq.downloadHandler.text;
                //set the username to saved to following scenes
                //MainManager.Instance.Username = if_username.text;
                GlobalSettings.Username = if_username.text;
                if (webreq.downloadHandler.text == "Login success")
                    SceneManager.LoadScene("sc_MainMenu");
                break;
            default:
                displayTxt.text = "server error";
                break;
        }
    }
}
