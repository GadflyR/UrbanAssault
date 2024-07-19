using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public GameObject victoryScreen;

    private void Start()
    {
        // 初始化时隐藏胜利界面
        victoryScreen.SetActive(false);
    }

    private void Update()
    {
        // 查找场景中所有带有“Enemy”标签的对象
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 检查敌人的数量
        if (enemies.Length == 0)
        {
            // 显示胜利界面
            ShowVictoryScreen();
        }
    }

    void ShowVictoryScreen()
    {
        // 显示胜利界面
        victoryScreen.SetActive(true);
        // 这里你可以添加更多逻辑，比如暂停游戏等
    }
}
