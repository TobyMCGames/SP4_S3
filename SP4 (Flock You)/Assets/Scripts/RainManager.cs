using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RainManager : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem RainSystem;
    public ParticleSystem CloudSystem;

    [Header("Audio")]
    public AudioClip RainSound;
    public AudioClip WindSound;

    private LoopingAudioSource RainLoopAudio;
    private LoopingAudioSource WindLoopAudio;

    [Header("Rain Settings")]
    [Range(0.0f, 1.0f)]
    public float RainIntensity;
    private float PrevRainIntensity;

    public Transform camera;

    [Header("Default Rain Emmision")]
    float RainEmi;
    float CloudEmi;

    // Start is called before the first frame update
    void Start()
    {
        RainEmi = RainSystem.emission.rateOverTimeMultiplier;
        CloudEmi = CloudSystem.emission.rateOverTimeMultiplier;

        RainSystem.emissionRate = RainEmi * RainIntensity;
        CloudSystem.emissionRate = CloudEmi * RainIntensity;

        if (RainSound != null)
            RainLoopAudio = new LoopingAudioSource(this, RainSound, null);

        if (WindSound != null)
            WindLoopAudio = new LoopingAudioSource(this, WindSound, null);
    }

    // Update is called once per frame
    void Update()
    {
        // Update Audio
        RainLoopAudio.Update();
        WindLoopAudio.Update();

        // Follow Camera
        if (camera != null)
        {
            Vector3 NewPos = camera.position;
            NewPos.y += 25.0f;
            RainSystem.transform.position = NewPos;
        }

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
            if (FYGameManager.Instance.isPaused)
                return;

        // If Change in Rain Intensity modify values
        if (PrevRainIntensity == RainIntensity)
            return;

        RainSystem.emissionRate = RainEmi * RainIntensity;
        CloudSystem.emissionRate = CloudEmi * RainIntensity;

        if (RainLoopAudio != null)
            RainLoopAudio.Play(RainIntensity * 0.5f);

        if (WindLoopAudio != null)
            WindLoopAudio.Play(RainIntensity * 0.5f);


        PrevRainIntensity = RainIntensity;
    }

    public void TogglePause()
    {
        if (FYGameManager.Instance.isPaused)
        {
            RainLoopAudio.AudioSource.Stop();
            WindLoopAudio.AudioSource.Stop();
        }
        else
        {
            RainLoopAudio.AudioSource.Play();
            WindLoopAudio.AudioSource.Play();
        }
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