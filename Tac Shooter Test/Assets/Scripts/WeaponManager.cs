using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public Gun selectedPrimaryWeapon;
    public Gun selectedSecondaryWeapon;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 保持对象在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject); // 如果有多个 WeaponManager 实例，销毁多余的实例
        }
    }
}
