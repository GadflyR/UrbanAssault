using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI ammoText;

    public List<GameObject> UIs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
