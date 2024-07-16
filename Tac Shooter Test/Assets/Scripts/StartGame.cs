using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartNewGame()
    {
        // 替换 "GameScene" 为你的实际游戏场景名称
        SceneManager.LoadScene("SampleScene");
    }
}
