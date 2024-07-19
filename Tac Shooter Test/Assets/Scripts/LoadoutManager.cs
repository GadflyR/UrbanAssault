using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadoutManager : MonoBehaviour
{
    public Gun[] guns;
    public Image weaponDisplay;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponDescriptionText;

    public GameObject weaponDataPanel;
    public TextMeshProUGUI bulletSpeedText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI firerateText;
    public TextMeshProUGUI magAmmoText;
    public TextMeshProUGUI reloadTimeText;
    public TextMeshProUGUI spreadText;
    public TextMeshProUGUI shotsFiredText;
    public TextMeshProUGUI noiseText;

    public Button primarySlotButton;
    public Button secondarySlotButton;
    public GameObject weaponSelectionPanel;
    public Button[] weaponButtons;
    public Button startGameButton;
    public AudioClip buttonClickSound; // 按钮点击音效

    private int selectedSlot; // 0 for Primary, 1 for Secondary

    private Gun selectedPrimaryWeapon;
    private Gun selectedSecondaryWeapon;

    private string selectedLevel;

    void Start()
    {
        // 获取传递的选中等级
        selectedLevel = PlayerPrefs.GetString("SelectedLevel");

        Debug.Log("Initializing LoadoutManager...");
        Debug.Log($"Selected Level: {selectedLevel}");
        Debug.Log($"Guns array length: {guns.Length}");

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            int index = i;
            weaponButtons[i].onClick.AddListener(() => {
                PlayButtonClickSound();
                SelectWeapon(index);
            });
            Debug.Log($"Button {i} bound to weapon index {index}");

            EventTrigger trigger = weaponButtons[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { OnPointerEnter(index); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { OnPointerExit(); });
            trigger.triggers.Add(entryExit);
        }

        primarySlotButton.onClick.AddListener(() => {
            PlayButtonClickSound();
            ShowWeaponSelection(0);
        });
        secondarySlotButton.onClick.AddListener(() => {
            PlayButtonClickSound();
            ShowWeaponSelection(1);
        });
        startGameButton.onClick.AddListener(() => {
            PlayButtonClickSound();
            StartGame();
            MusicManager.instance.StopPlaying();
            UIManager.instance.SwitchUIs(1);
        });

        weaponSelectionPanel.SetActive(false);
        weaponDisplay.gameObject.SetActive(false);
        weaponDataPanel.SetActive(false);
        startGameButton.interactable = false;
    }

    private void PlayButtonClickSound()
    {
        if (ClickManager.instance != null && buttonClickSound != null)
        {
            ClickManager.instance.PlayClickSound(buttonClickSound);
        }
    }

    public void ShowWeaponSelection(int slot)
    {
        selectedSlot = slot;

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            bool shouldShow = false;
            if (slot == 0 && guns[i].weaponType == Gun.WeaponType.Primary)
            {
                shouldShow = true;
            }
            else if (slot == 1 && guns[i].weaponType == Gun.WeaponType.Secondary)
            {
                shouldShow = true;
            }

            weaponButtons[i].gameObject.SetActive(shouldShow);
        }

        weaponSelectionPanel.SetActive(true);
    }

    public void SelectWeapon(int index)
    {
        if (index < 0 || index >= guns.Length)
        {
            Debug.LogError($"Index {index} is out of bounds for guns array of length {guns.Length}");
            return;
        }

        weaponSelectionPanel.SetActive(false);

        Gun selectedGun = guns[index];

        if (selectedSlot == 0)
        {
            selectedPrimaryWeapon = selectedGun;
            WeaponManager.instance.selectedPrimaryWeapon = selectedGun;
            primarySlotButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedGun.gunName;
        }
        else if (selectedSlot == 1)
        {
            selectedSecondaryWeapon = selectedGun;
            WeaponManager.instance.selectedSecondaryWeapon = selectedGun;
            secondarySlotButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedGun.gunName;
        }

        weaponDisplay.sprite = selectedGun.weaponSprite;
        weaponDisplay.color = Color.white;

        weaponNameText.text = guns[index].gunName;
        weaponDescriptionText.text = guns[index].GetDescription();

        CheckWeaponsSelected();
    }

    private void CheckWeaponsSelected()
    {
        if (selectedPrimaryWeapon != null && selectedSecondaryWeapon != null)
        {
            startGameButton.interactable = true;
        }
    }

    public void StartGame()
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

    public void OnPointerEnter(int index)
    {
        if (index < 0 || index >= guns.Length)
        {
            Debug.LogError($"Index {index} is out of bounds for guns array of length {guns.Length}");
            return;
        }

        Gun gun = guns[index];

        weaponNameText.text = gun.gunName;
        weaponDescriptionText.text = gun.description;
        weaponDisplay.sprite = gun.weaponSprite;
        weaponDisplay.color = Color.white;

        weaponDisplay.gameObject.SetActive(true);

        bulletSpeedText.text = $"Bullet Speed: {gun.bulletSpeed}";
        damageText.text = $"Damage: {gun.damage}";
        firerateText.text = $"Firerate: {gun.firerate}s";
        magAmmoText.text = $"Mag Ammo: {gun.magAmmo}";
        reloadTimeText.text = $"Reload Time: {gun.reloadTime}s";
        spreadText.text = $"Spread: {gun.spread}";
        shotsFiredText.text = $"Shots Fired: {gun.shotsFired}";
        noiseText.text = $"Noise: {gun.noise}";

        weaponDataPanel.SetActive(true);
    }

    public void OnPointerExit()
    {
        weaponNameText.text = "";
        weaponDescriptionText.text = "";
        weaponDisplay.sprite = null;
        weaponDisplay.color = new Color(1, 1, 1, 0);

        weaponDisplay.gameObject.SetActive(false);

        bulletSpeedText.text = "";
        damageText.text = "";
        firerateText.text = "";
        magAmmoText.text = "";
        reloadTimeText.text = "";
        spreadText.text = "";
        shotsFiredText.text = "";
        noiseText.text = "";

        weaponDataPanel.SetActive(false);
    }
}
