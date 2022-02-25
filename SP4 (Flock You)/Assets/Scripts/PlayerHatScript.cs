using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHatScript : MonoBehaviour
{
    private string currentHat;
    private void Awake()
    {
        currentHat = GlobalSettings.CurrentSkin;
    }

    private void Start()
    {
        if(currentHat != "" && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Hats/" + currentHat, Vector3.zero, Quaternion.identity); 
        }
    }
}
