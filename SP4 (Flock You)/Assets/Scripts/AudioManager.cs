using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip BG_Music;
    private LoopingAudioSource LoopBGAudio;

    [Header("Audio Settings")]
    [Range(0.0f, 1.0f)]
    public float Volume;
    private float prevVolume;
    public bool SetAsGlobalVolume;

    // Start is called before the first frame update
    void Start()
    {
        if (SetAsGlobalVolume)
            GlobalSettings.Volume = Volume;

        if (BG_Music != null)
        {
            LoopBGAudio = new LoopingAudioSource(this, BG_Music, null);
            LoopBGAudio.Play(Volume);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (prevVolume == Volume)
            return;

        if (SetAsGlobalVolume)
            GlobalSettings.Volume = Volume;

        if (LoopBGAudio != null)
        {
            LoopBGAudio.AudioSource.volume = Volume;
        }


        prevVolume = Volume;
    }
}

public class LoopingAudioSource
{
    public AudioSource AudioSource { get; private set; }
    public float TargetVolume { get; private set; }

    public LoopingAudioSource(MonoBehaviour script, AudioClip clip, AudioMixerGroup mixer)
    {
        AudioSource = script.gameObject.AddComponent<AudioSource>();

        if (mixer != null)
        {
            AudioSource.outputAudioMixerGroup = mixer;
        }

        AudioSource.loop = true;
        AudioSource.clip = clip;
        AudioSource.playOnAwake = false;
        AudioSource.volume = 0.0f;
        AudioSource.Stop();
        TargetVolume = 1.0f;
    }

    public void Play(float targetVolume)
    {
        if (!AudioSource.isPlaying)
        {
            AudioSource.volume = 0.0f;
            AudioSource.Play();
        }
        TargetVolume = targetVolume;
    }

    public void Stop()
    {
        TargetVolume = 0.0f;
    }

    public void Update()
    {
        if (AudioSource.isPlaying && (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime)) == 0.0f)
        {
            AudioSource.Stop();
        }
    }
}