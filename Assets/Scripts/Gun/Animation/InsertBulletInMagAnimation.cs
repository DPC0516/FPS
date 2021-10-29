using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;
using Photon.Pun;

public class InsertBulletInMagAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerVarriables playerVarriables;
    [SerializeField]
    private PlayerStatus playerStatus;
    //총기 변경 조작 홀더
    [SerializeField]
    private Transform changeGunAnimationHolder;
    //탄창에 총알 넣기시 탄창 위치
    [SerializeField]
    private Transform insertMagPosition;
    //총알 박스 지점
    [SerializeField]
    private Transform cartridgePosition;
    //총 변경 소리
    [SerializeField]
    private AudioClip changeGunSound;
    //총알 넣는 소리
    [SerializeField]
    private AudioClip insertBullet;

    IEnumerator insertBulletInMagAnimationI;

    public void playPrepareAnimation(int targetNum)
    {
        playerStatus.isChangingGun = true;
        StartCoroutine(getInGunInsert(targetNum));
    }

    public void playUnPrepareAnimation(int targetNum)
    {
        try
        {
            StopCoroutine(insertBulletInMagAnimationI);
        }
        catch { }

        playerStatus.isPreparedInsertBulletInMag = false;
        StartCoroutine(magInInsert(targetNum));
    }

    public void playInsertAnimation(int _cartridgeNum, int targetNum, string bulletType)
    {
        try
        {
            StopCoroutine(insertBulletInMagAnimationI);
        }
        catch { }

        insertBulletInMagAnimationI = insertBulletInMagAnimation(_cartridgeNum, targetNum, bulletType);
        playerStatus.isInsertingBulletInMag = true;
        StartCoroutine(insertBulletInMagAnimationI);
    }

    //총기 집어 넣기
    private IEnumerator getInGunInsert(int targetNum)
    {
        Transform target;
        WeaponLoadout _weaponLoadout = Public.getWeaponLoadout(playerVarriables.PV, playerVarriables.weaponLoadout);

        changeGunAnimationHolder.position = playerVarriables.gunController.gunHolder.transform.position;
        changeGunAnimationHolder.rotation = playerVarriables.gunController.gunHolder.transform.rotation;

        playerVarriables.smallSound.PlayOneShot(changeGunSound);

        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);

            target = playerVarriables.bag.primaryGunHolder;
        }
        else
        {
            Public.setParent(_weaponLoadout.secondaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);

            target = playerVarriables.bag.secondaryGunHolder;
        }
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
        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, playerVarriables.bag.primaryGunHolder, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Public.setParent(_weaponLoadout.secondaryWeapon.transform, playerVarriables.bag.secondaryGunHolder, Vector3.zero, Quaternion.identity);
        }
        playerStatus.isChangingGun = false;
        StartCoroutine(magOutInsert(targetNum));
    }

    //총기 꺼내기
    private IEnumerator getOutGunInsert()
    {
        WeaponLoadout _weaponLoadout = Public.getWeaponLoadout(playerVarriables.PV, playerVarriables.weaponLoadout);

        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);

            changeGunAnimationHolder.position = playerVarriables.bag.primaryGunHolder.position;
            changeGunAnimationHolder.rotation = playerVarriables.bag.primaryGunHolder.rotation;
        }
        else
        {
            changeGunAnimationHolder.position = playerVarriables.bag.secondaryGunHolder.position;
            changeGunAnimationHolder.rotation = playerVarriables.bag.secondaryGunHolder.rotation;

            Public.setParent(_weaponLoadout.secondaryWeapon.transform, changeGunAnimationHolder, Vector3.zero, Quaternion.identity);
        }

        playerVarriables.rightArmHeader.position = playerVarriables.rightArmController.target.position;
        playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;

        playerVarriables.rightArmController.setTarget(playerVarriables.rightArmHeader);
        playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);

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

        playerVarriables.smallSound.PlayOneShot(changeGunSound);
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
    }

    //탄창 꺼내기
    private IEnumerator magOutInsert(int targetNum)
    {
        playerVarriables.rightArmHeader.position = playerVarriables.rightArmController.target.position;
        playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;

        playerVarriables.rightArmController.setTarget(playerVarriables.rightArmHeader);
        playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);

        Transform targetMag = playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().mag.transform;
        Transform grabPosition = playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript().magGrabPosition;

        while (true)
        {
            playerVarriables.rightArmHeader.position =
                Vector3.Slerp(
                    playerVarriables.rightArmHeader.position,
                    grabPosition.position,
                    PlayerVarriables.armChangeTargetSpeed * Time.deltaTime);

            playerVarriables.leftArmHeader.position =
                Vector3.Slerp(
                    playerVarriables.leftArmHeader.position,
                    grabPosition.position,
                    PlayerVarriables.armChangeTargetSpeed * Time.deltaTime);

            if (Vector3.Distance(grabPosition.position, playerVarriables.rightArmHeader.position) < ErrorRange.changeArm)
            {
                playerVarriables.rightArmController.setTarget(grabPosition);
                playerVarriables.leftArmController.setTarget(grabPosition);
                break;
            }

            yield return null;
        }

        while (true)
        {
            targetMag.position =
                Vector3.Slerp(
                    targetMag.position,
                    insertMagPosition.position,
                    playerVarriables.gunController.gunScript.speed[Speed.magInOutSpeed] * Time.deltaTime);

            targetMag.rotation =
                Quaternion.Slerp(
                    targetMag.rotation,
                    insertMagPosition.rotation,
                    playerVarriables.gunController.gunScript.speed[Speed.magInOutSpeed] * Time.deltaTime);

            if (Vector3.Distance(targetMag.position, insertMagPosition.position) < ErrorRange.magReload)
            {
                Public.setParent(targetMag, insertMagPosition, Vector3.zero, Quaternion.identity);
                playerStatus.isPreparedInsertBulletInMag = true;
                break;
            }

            yield return null;
        }
    }

    //탄창 넣기
    private IEnumerator magInInsert(int targetNum)
    {
        Transform targetMag = playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().mag.transform;

        playerVarriables.rightArmController.setTarget(targetMag);
        playerVarriables.leftArmController.setTarget(targetMag);

        while (true)
        {
            targetMag.position =
                Vector3.Slerp(
                    targetMag.position,
                    playerVarriables.magPositions[targetNum].position,
                    playerVarriables.gunController.gunScript.speed[Speed.magInOutSpeed] * Time.deltaTime);

            targetMag.rotation =
                Quaternion.Slerp(
                    targetMag.rotation,
                    playerVarriables.magPositions[targetNum].rotation,
                    playerVarriables.gunController.gunScript.speed[Speed.magInOutSpeed] * Time.deltaTime);

            if (Vector3.Distance(targetMag.position, playerVarriables.magPositions[targetNum].position) < ErrorRange.magReload)
            {
                Public.setParent(targetMag, playerVarriables.magPositions[targetNum], Vector3.zero, Quaternion.identity);
                break;
            }

            yield return null;
        }

        playerStatus.isChangingGun = true;

        StartCoroutine(getOutGunInsert());
    }

    private IEnumerator insertBulletInMagAnimation(int _cartridgeNum, int targetNum, string bulletType)
    {
        Mag targetMag = playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript();
        Transform grabPosition = playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript().magGrabPosition;

        playerVarriables.rightArmHeader.position = playerVarriables.rightArmController.target.position;

        playerVarriables.rightArmController.setTarget(playerVarriables.rightArmHeader);

        while (true)
        {
            playerVarriables.rightArmHeader.position =
                Vector3.Slerp(
                    playerVarriables.rightArmHeader.position,
                    cartridgePosition.position,
                    targetMag.insertBulletSpeed * Time.deltaTime);

            if (Vector3.Distance(playerVarriables.rightArmHeader.position, cartridgePosition.position) < ErrorRange.insertBullet)
            {
                break;
            }
            yield return null;
        }

        GameObject bulletPrefab = Resources.Load(Path.bulletsPrefabPath + "Bullet_NR_" + bulletType) as GameObject;
        GameObject bulletInstantiate = Instantiate(bulletPrefab, playerVarriables.rightArmHeader);

        while (true)
        {
            playerVarriables.rightArmHeader.position =
                Vector3.Slerp(
                    playerVarriables.rightArmHeader.position,
                    targetMag.magEntrancePosition.position,
                    targetMag.insertBulletSpeed * Time.deltaTime);

            playerVarriables.rightArmHeader.localRotation = Quaternion.identity;

            if (Vector3.Distance(playerVarriables.rightArmHeader.position, targetMag.magEntrancePosition.position) < ErrorRange.changeArm)
            {
                break;
            }
            yield return null;
        }

        Destroy(bulletInstantiate);

        playerVarriables.smallSound.PlayOneShot(insertBullet);

        if (playerVarriables.PV.IsMine)
        {
            Bullet bullet = Public.weaponLoadout.bulletCartridges[_cartridgeNum].removeBullet();
            if (!playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript().insertBullet(bullet))
            {
                Public.weaponLoadout.bulletCartridges[_cartridgeNum].insertBullet();
            }
        }

        playerStatus.isInsertingBulletInMag = false;

        while (true)
        {
            playerVarriables.rightArmHeader.position =
                   Vector3.Slerp(
                       playerVarriables.rightArmHeader.position,
                       grabPosition.position,
                       PlayerVarriables.armChangeTargetSpeed * Time.deltaTime);

            if (Vector3.Distance(playerVarriables.rightArmHeader.position, grabPosition.position) < ErrorRange.changeArm)
            {
                playerVarriables.rightArmController.setTarget(grabPosition);
                break;
            }
            yield return null;
        }
    }
}
