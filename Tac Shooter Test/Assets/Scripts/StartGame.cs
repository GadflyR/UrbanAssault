using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AudioClip buttonClickSound;

    public GameObject[] UIs;

    public void StartNewGame()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene("LevelSelection");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowCredits()
    {
        SwitchUIs(1);
    }

    public void Back()
    {
        SwitchUIs(0);
    }

    public void SwitchUIs(int index)
    {
        foreach (GameObject UI in UIs)
            UI.SetActive(false);
        UIs[index].SetActive(true);
    }

    private void PlayButtonClickSound()
    {
        if (ClickManager.instance != null && buttonClickSound != null)
        {
            ClickManager.instance.PlayClickSound(buttonClickSound);
        }
    }
}
