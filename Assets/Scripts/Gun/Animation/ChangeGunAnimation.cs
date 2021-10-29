using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;

public class ChangeGunAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerVarriables playerVarriables;
    [SerializeField]
    private PlayerStatus playerStatus;
    //총기 변경 조작 홀더
    [SerializeField]
    private Transform changeGunAnimationHolder;
    [SerializeField]
    private AudioClip changeGunSound;

    public void playAnimation(int toWeaponType)
    {
        playerStatus.isChangingGun = true;
        StartCoroutine(changeGunAnimation(toWeaponType));
    }

    //총기 집어 넣기
    private IEnumerator getInGun(int toWeaponType)
    {
        Transform target;
        WeaponLoadout _weaponLoadout = Public.getWeaponLoadout(playerVarriables.PV, playerVarriables.weaponLoadout);

        playerVarriables.smallSound.PlayOneShot(changeGunSound);

        changeGunAnimationHolder.position = playerVarriables.gunController.gunHolder.transform.position;
        changeGunAnimationHolder.rotation = playerVarriables.gunController.gunHolder.transform.rotation;

        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            target = playerVarriables.bag.primaryGunHolder;
            Public.setParent(_weaponLoadout.primaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);
        }
        else
        {
            target = playerVarriables.bag.secondaryGunHolder;
            Public.setParent(_weaponLoadout.secondaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);
        }

        //총기 총기 거치대로
        while (true)
        {
            changeGunAnimationHolder.position =
                Vector3.Slerp(
                    changeGunAnimationHolder.position,
                    target.position,
                    PlayerVarriables.changeGunSpeed * Time.deltaTime);
            changeGunAnimationHolder.rotation =
                Quaternion.Slerp(
                    changeGunAnimationHolder.rotation,
                    target.rotation,
                    PlayerVarriables.changeGunSpeed * Time.deltaTime);

            if (Vector3.Distance(changeGunAnimationHolder.position, target.position) < ErrorRange.changeGun)
            {
                changeGunAnimationHolder.position = target.position;
                break;
            }
            yield return null;
        }

        //총기 거치대에 고정
        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, target.transform, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Public.setParent(_weaponLoadout.secondaryWeapon.transform, target.transform, Vector3.zero, Quaternion.identity);
        }

        _weaponLoadout.currentWeaponType = toWeaponType;
        playerVarriables.gunController.gunScript = _weaponLoadout.getGunScript(_weaponLoadout.currentWeaponType);

        StartCoroutine(getOutGun());
    }

    //총기 꺼내기
    private IEnumerator getOutGun()
    {
        WeaponLoadout _weaponLoadout = Public.getWeaponLoadout(playerVarriables.PV, playerVarriables.weaponLoadout);

        playerVarriables.rightArmHeader.position = playerVarriables.rightArmController.target.position;
        playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;

        playerVarriables.rightArmController.setTarget(playerVarriables.rightArmHeader);
        playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);

        //총기 잡기
        while (true)
        {
            playerVarriables.rightArmHeader.position = Vector3.Lerp(
                playerVarriables.rightArmHeader.position,
                playerVarriables.gunController.gunScript.triggerPosition.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);
            playerVarriables.leftArmHeader.position = Vector3.Slerp(
                playerVarriables.leftArmHeader.position,
                playerVarriables.gunController.gunScript.leftHandPosition.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);

            if (Vector3.Distance(playerVarriables.rightArmHeader.position, playerVarriables.gunController.gunScript.triggerPosition.position) < ErrorRange.changeArm)
            {
                playerVarriables.rightArmController.setTarget(playerVarriables.gunController.gunScript.triggerPosition);
                playerVarriables.leftArmController.setTarget(playerVarriables.gunController.gunScript.leftHandPosition);
                break;
            }
            yield return null;
        }

        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            changeGunAnimationHolder.position = playerVarriables.bag.primaryGunHolder.position;
            changeGunAnimationHolder.rotation = playerVarriables.bag.primaryGunHolder.rotation;

            Public.setParent(_weaponLoadout.primaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);
        }
        else
        {
            changeGunAnimationHolder.position = playerVarriables.bag.secondaryGunHolder.position;
            changeGunAnimationHolder.rotation = playerVarriables.bag.secondaryGunHolder.rotation;

            Public.setParent(_weaponLoadout.secondaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);
        }

        playerVarriables.gunController.gunHolder.transform.localPosition = playerVarriables.gunController.gunScript.gunDefaultPosition;

        playerVarriables.smallSound.PlayOneShot(changeGunSound);

        //총기 이동
        while (true)
        {
            changeGunAnimationHolder.position =
                Vector3.Slerp(
                    changeGunAnimationHolder.position,
                    playerVarriables.gunController.gunHolder.transform.position,
                    PlayerVarriables.changeGunSpeed * Time.deltaTime);
            changeGunAnimationHolder.rotation =
                Quaternion.Slerp(
                    changeGunAnimationHolder.rotation,
                    playerVarriables.gunController.gunHolder.transform.rotation,
                    PlayerVarriables.changeGunSpeed * Time.deltaTime);

            if (Vector3.Distance(changeGunAnimationHolder.position, playerVarriables.gunController.gunHolder.transform.position) < ErrorRange.changeGun)
            {
                changeGunAnimationHolder.position = playerVarriables.gunController.gunHolder.transform.position;
                break;
            }
            yield return null;
        }

        //현재 총기를 GunHolder에 할당
        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, playerVarriables.gunController.gunHolder, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Public.setParent(_weaponLoadout.secondaryWeapon.transform, playerVarriables.gunController.gunHolder, Vector3.zero, Quaternion.identity);
        }

        playerStatus.isChangingGun = false;

        //총기 초기화
        playerVarriables.gunController.initGun();
    }

    //총기 변경 애니메이션
    private IEnumerator changeGunAnimation(int toWeaponType)
    {
        StartCoroutine(getInGun(toWeaponType));
        yield return null;
    }
}
