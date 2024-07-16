using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource source;
    public AudioClip kickDoorSFX;

    void Awake()
    {
        // 确保单例模式的唯一性
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 如果希望在场景切换时保留该对象
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 获取 AudioSource 组件
        source = GetComponent<AudioSource>();

        // 检查是否成功获取
        if (source == null)
        {
            Debug.LogError("AudioSource component not found on AudioManager!");
        }
    }

    public void PlaySFX(AudioClip sound, float volume)
    {
        // 检查 sound 和 source 是否有效
        if (sound != null && source != null)
        {
            source.PlayOneShot(sound, volume);
        }
        else
        {
            Debug.LogError("AudioClip or AudioSource is missing!");
        }
    }
    public void StopPlaying()
    {
        source.Stop();
    }
}
