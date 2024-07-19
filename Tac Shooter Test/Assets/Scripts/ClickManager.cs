using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public static ClickManager instance = null;

    private AudioSource audioSource;

    void Awake()
    {
        // 单例模式，确保全局唯一的ClickManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClickSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
