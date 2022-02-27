using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceDeafen : MonoBehaviour
{
    // Photon voice deafen
    protected bool isDeafened;
    AudioSource audioSource;
    public AudioSource[] soundFX;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Mute a player when he selects mute button
        if (Input.GetKeyUp(","))
        {
            Debug.Log(", key was released");
            if (isDeafened == true)
            {
                isDeafened = false;
                audioSource.mute = false;
                soundFX[1].Play();
            }
            else
            {
                isDeafened = true;
                audioSource.mute = true;
                soundFX[0].Play();
                // Add code to mute the player too if time permits
            }
        }
    }
}
