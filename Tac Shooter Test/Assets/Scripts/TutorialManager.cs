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
        {
            Destroy(gameObject);
        }
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
            if (!isInCutscene && greenPanel.activeSelf)
            {
                greenPanel.SetActive(false);
            }
        }
        else
        {
            isInCutscene = false;
            if (greenPanel != null)
                greenPanel.SetActive(false);
        }
    }

    public void PlayVoiceLine(int index)
    {
        isInCutscene = true;
        LockAllEnemies(); // 锁定所有敌人位置和旋转
        source.PlayOneShot(voiceLines[index]);
        greenPanel.SetActive(true);
        StartCoroutine(EndCutsceneAfterVoiceLine(voiceLines[index].length)); // 在播放完声音后结束cutscene
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 3)
        {
            playedEnemyOneDeathVoiceLine = false;
            playedEnemyTwoAndThreeDeathVoiceLine = false;
            enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);
            greenPanel = GameObject.FindGameObjectWithTag("Panel");
        }
    }

    private void LockAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.LockPositionAndRotation();
            }
        }
    }

    private void UnlockAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.UnlockPositionAndRotation();
            }
        }
    }

    private IEnumerator EndCutsceneAfterVoiceLine(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndCutscene();
    }

    public void EndCutscene()
    {
        isInCutscene = false;
        UnlockAllEnemies(); // 解锁所有敌人位置和旋转
        if (greenPanel != null)
        {
            greenPanel.SetActive(false);
        }
    }
}
