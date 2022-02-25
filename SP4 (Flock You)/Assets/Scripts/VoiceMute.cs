using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;

public class VoiceMute : MonoBehaviour
{
    // Photon voice mute
    protected bool isMuted;
    Recorder recorder;

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
        recorder.TransmitEnabled = false; // mute when start game
    }

    // Update is called once per frame
    void Update()
    {
        // Mute a player when he selects mute button
        if (Input.GetKeyUp("m"))
        {
            Debug.Log("m key was released");
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
