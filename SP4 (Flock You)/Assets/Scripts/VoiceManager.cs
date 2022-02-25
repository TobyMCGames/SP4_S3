using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    // Photon voice mute
    protected bool isMuted;
    Recorder recorder;

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
    }

    // Update is called once per frame
    void Update()
    {
        // Mute a player when he selects mute button
        if (Input.GetKeyUp("m"))
        {
            Debug.Log("M key was released");
            if (isMuted == true)
            {
                isMuted = false;
                recorder.TransmitEnabled = true;
            }
            else
            {
                isMuted = true;
                recorder.TransmitEnabled = false;
            }
        }
    }
}
