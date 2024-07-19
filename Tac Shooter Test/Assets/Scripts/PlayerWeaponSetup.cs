using UnityEngine;

public class PlayerWeaponSetup : MonoBehaviour
{
    void Start()
    {
        // 获取 WeaponManager 实例
        WeaponManager weaponManager = WeaponManager.instance;

        // 检查是否有选中的主武器和副武器
        if (weaponManager.selectedPrimaryWeapon != null)
        {
            // 实例化主武器并将其附加到主武器槽
            Gun primaryWeapon = Instantiate(weaponManager.selectedPrimaryWeapon, transform);
            // primaryWeapon.transform.SetParent(primaryWeaponSlot);
        }

        if (weaponManager.selectedSecondaryWeapon != null)
        {
            // 实例化副武器并将其附加到副武器槽
            Gun secondaryWeapon = Instantiate(weaponManager.selectedSecondaryWeapon, transform);
            // secondaryWeapon.transform.SetParent(secondaryWeaponSlot);
        }
    }
}
