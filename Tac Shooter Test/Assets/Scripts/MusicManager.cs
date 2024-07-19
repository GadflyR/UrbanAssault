using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void StartPlaying()
    {
        source.Play();
    }

    public void StopPlaying()
    {
        source.Stop();
    }
}
