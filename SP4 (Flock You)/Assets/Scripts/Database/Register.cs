using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class Register : MonoBehaviour
{
    public string URL = GlobalSettings.BaseServer + "register.php";
    public Text displayTxt; //must add using UnityEngine.UI
    public InputField if_username, if_password, if_confirmPassword; //to link to the inpufields
    public void DoRegister()
    { //to be involved by button click
        Debug.Log("SendUserNamePassword");
        if (if_username.text == "" || if_password.text == "")
            displayTxt.text = "Username or Password is empty";
        else if (if_password.text != if_confirmPassword.text)
            displayTxt.text = "Passwords do not match";
        else
            StartCoroutine(DoSendUserNamePassword());
    }
    IEnumerator DoSendUserNamePassword()
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
                break;
            default:
                displayTxt.text = "Server Error";
                break;
        }
    }
}

