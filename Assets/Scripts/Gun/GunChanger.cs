using UnityEngine;
using Varriables;

//총기 변환 관리
public class GunChanger : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    private PlayerVarriables playerVarriables;

    void Update() 
    {
        if (!Input.GetKey(Key.Relaod) 
            && !Input.GetKey(Key.InsertBulletInMag) 
            && !playerStatus.isChangingGun 
            && !playerStatus.isPreparedInsertBulletInMag 
            && !playerStatus.isParachute)
        {
            if (Input.GetKeyDown(Key.PrimaryWeapon))
            {
                if (!playerStatus.isReloading)
                {
                    if (Public.weaponLoadout.currentWeaponType == WeaponType.secondary)
                    {
                        playerVarriables.gunController.changeGun(WeaponType.primary);
                    }
                }
            }
            if (Input.GetKeyDown(Key.SecondaryWeapon) && !Input.GetKey(Key.Relaod))
            {
                if (!playerStatus.isReloading)
                {
                    if (Public.weaponLoadout.currentWeaponType == WeaponType.primary)
                    {
                        playerVarriables.gunController.changeGun(WeaponType.secondary);
                    }
                }
            }
        }
    }
}
