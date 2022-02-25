using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceDeafen : MonoBehaviour
{
    // Photon voice deafen
    protected bool isDeafened;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Mute a player when he selects mute button
        //if (Input.GetKeyUp("m"))
        if (Input.GetKeyUp(","))
        {
            Debug.Log(", key was released");
            if (isDeafened == true)
            {
                isDeafened = false;
                //recorder.TransmitEnabled = true;
                audioSource.mute = false;
            }
            else
            {
                isDeafened = true;
                //recorder.TransmitEnabled = false;
                audioSource.mute = true;
            }
        }
    }


}
