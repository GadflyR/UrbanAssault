using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons;
    public AudioClip buttonClickSound; // 按钮点击音效
    private string selectedLevel;

    void Start()
    {
        // 手动指定每个按钮对应的场景名称
        string[] levelNames = { "Tutorial", "Level 1", "Level 2", "Level 3" };

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i; // 本地副本以供闭包使用
            levelButtons[i].onClick.AddListener(() => {
                PlayButtonClickSound();
                OnLevelSelected(levelNames[index]);
            });
        }
    }

    private void PlayButtonClickSound()
    {
        if (ClickManager.instance != null && buttonClickSound != null)
        {
            ClickManager.instance.PlayClickSound(buttonClickSound);
        }
    }

    private void OnLevelSelected(string levelName)
    {
        selectedLevel = levelName;

        // 设置所有按钮为正常状态
        foreach (Button button in levelButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }

        // 找到选中的按钮并设置为按下状态
        foreach (Button button in levelButtons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == levelName)
            {
                button.GetComponent<Image>().color = Color.green;
                break;
            }
        }

        // 保存选中的等级
        PlayerPrefs.SetString("SelectedLevel", selectedLevel);

        // 加载武器选择界面
        SceneManager.LoadScene("WeaponLoadout"); // 替换为您的武器选择界面名称
    }

    private void StartGame()
    {
        if (!string.IsNullOrEmpty(selectedLevel))
        {
            SceneManager.LoadScene(selectedLevel);
        }
        else
        {
            Debug.LogError("未选择任何等级！");
        }
    }
}
