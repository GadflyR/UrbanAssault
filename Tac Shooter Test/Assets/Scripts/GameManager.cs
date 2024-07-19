using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private Enemy[] enemies;

    private float timer;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.enemyCounter.text = $"Hostiles Remaining: {enemies.Length}";
            if (enemies.Length <= 0)
            {
                UpdateBestTime();
                UIManager.instance.SwitchUIs(2);
            }
            else
                timer += Time.deltaTime;

            UIManager.instance.timerText.text = "Mission Time: " + timer.ToString("#.00") + "s";
        }
        if (!Array.TrueForAll(enemies, enemy => enemy != null))
            UpdateEnemyList();
    }

    private void OnLevelWasLoaded(int level)
    {
        timer = 0;
        UpdateEnemyList();
        UIManager.instance.restartButton.onClick.AddListener(RestartGame);
        UIManager.instance.backButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        UIManager.instance.backButton.onClick.AddListener(MusicManager.instance.StartPlaying);
    }

    public void UpdateBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat($"Level {SceneManager.GetActiveScene().buildIndex - 3} Best Time");
        if (timer < bestTime || bestTime <= 0)
            PlayerPrefs.SetFloat($"Level {SceneManager.GetActiveScene().buildIndex - 3} Best Time", timer);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting scene: " + SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnterMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void UpdateEnemyList()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }
}
