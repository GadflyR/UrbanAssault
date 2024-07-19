using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI ammoText;
    public TMP_Text enemyCounter;
    public TMP_Text timerText;

    public Button restartButton;
    public Button backButton;

    public List<GameObject> UIs;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        SwitchUIs(0);
    }

    public void SwitchUIs(int index)
    {
        foreach (GameObject UI in UIs)
            UI.SetActive(false);
        UIs[index].SetActive(true);
    }
}
