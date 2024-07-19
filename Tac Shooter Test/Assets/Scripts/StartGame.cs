using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AudioClip buttonClickSound;
    public void StartNewGame()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene("LevelSelection");
    }
    private void PlayButtonClickSound()
    {
        if (ClickManager.instance != null && buttonClickSound != null)
        {
            ClickManager.instance.PlayClickSound(buttonClickSound);
        }
    }
}
