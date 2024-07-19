using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource source;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeAudioSource();
    }

    void OnEnable()
    {
        InitializeAudioSource();
    }

    void InitializeAudioSource()
    {
        source = GetComponent<AudioSource>();

        if (source == null)
            Debug.LogError("AudioSource component not found on AudioManager!");
    }

    public void PlaySFX(AudioClip sound, float volume)
    {
        if (sound != null && source != null)
            source.PlayOneShot(sound, volume);
        else
            Debug.LogError($"AudioClip or AudioSource is missing! AudioClip: {(sound == null ? "null" : sound.name)}, AudioSource: {(source == null ? "null" : "exists")}");
    }

    public void StopPlaying()
    {
        if (source != null)
            source.Stop();
        else
            Debug.LogError("AudioSource component is missing or destroyed!");
    }
}
