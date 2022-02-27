using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip BG_Music;
    private LoopingAudioSource LoopBGAudio;
    public string[] AudioKey;
    public AudioClip[] AudioData;
    public Dictionary<string, AudioSource> AudioLib;

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
        else
            Volume = GlobalSettings.Volume;

        prevVolume = Volume;

        if (BG_Music != null)
        {
            LoopBGAudio = new LoopingAudioSource(this, BG_Music, null);
            LoopBGAudio.Play(Volume);
            LoopBGAudio.AudioSource.volume = Volume;
        }

        if (AudioData == null || AudioKey == null)
            return;

        if (AudioData.Length != AudioKey.Length)
        {
            Debug.Log("Audio Key do not match number of Audio Data");
            return;
        }

        AudioLib = new Dictionary<string, AudioSource>();
        for (int i = 0; i < AudioKey.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            source.clip = AudioData[i];
            source.playOnAwake = false;
            source.volume = GlobalSettings.Volume;
            source.Stop();
            AudioLib.Add(AudioKey[i], source);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (prevVolume == Volume)
            return;

        if (SetAsGlobalVolume)
            GlobalSettings.Volume = Volume;
        else
            Volume = GlobalSettings.Volume;

        if (LoopBGAudio != null)
        {
            LoopBGAudio.AudioSource.volume = Volume;
        }

        if (AudioLib != null)
        {
            for (int i = 0; i < AudioLib.Count; i++)
            {
                AudioLib[AudioKey[i]].volume = Volume;
            }
        }

        prevVolume = Volume;
    }

    public void PlayAudio(string Key)
    {
        if (AudioLib.ContainsKey(Key))
        {
            AudioLib[Key].Play();
        }
        else
            Debug.Log("This Audio Manager does not contain " + Key);
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