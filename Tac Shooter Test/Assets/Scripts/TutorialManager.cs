using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public Enemy[] enemies;
    private GameObject greenPanel;

    private bool playedEnemyOneDeathVoiceLine;
    private bool playedEnemyTwoAndThreeDeathVoiceLine;

    public AudioClip[] voiceLines;
    public bool isInCutscene;
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

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            isInCutscene = source.isPlaying;
            if (enemies[3] == null && !playedEnemyOneDeathVoiceLine)
            {
                PlayVoiceLine(2);
                playedEnemyOneDeathVoiceLine = true;
            }
            if (enemies[0] == null && enemies[1] == null && !playedEnemyTwoAndThreeDeathVoiceLine)
            {
                PlayVoiceLine(5);
                playedEnemyTwoAndThreeDeathVoiceLine = true;
            }
            if (!source.isPlaying && greenPanel.activeSelf)
            {
                greenPanel.SetActive(false);
            }
        }
        else
            isInCutscene = false;
    }

    public void PlayVoiceLine(int index)
    {
        source.PlayOneShot(voiceLines[index]);
        greenPanel.SetActive(true);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 3)
        {
            enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);
            greenPanel = GameObject.FindGameObjectWithTag("Panel");
        }
    }
}
