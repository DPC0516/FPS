using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;

public class ChangeFireModeAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerVarriables playerVarriables;
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    private AudioClip changeFireModeSound;

    public IEnumerator changeFireModeAnimationI;

    public void stopLastCoroutine()
    {
        try
        {
            StopCoroutine(changeFireModeAnimationI);
        }
        catch
        {

        }
    }

    public void playAnimation()
    {
        stopLastCoroutine();
        changeFireModeAnimationI = changeFireModeAnimation();
        //총기 발사모드 변경 애니메이션
        playerStatus.isChangingFireMode = true;
        StartCoroutine(changeFireModeAnimationI);
    }

    //발사 모드 변경 애니메이션
    private IEnumerator changeFireModeAnimation()
    {
        playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;
        playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);

        while (true)
        {
            playerVarriables.leftArmHeader.position = Vector3.Slerp(
                playerVarriables.leftArmHeader.position,
                playerVarriables.gunController.gunScript.safetyDevicePosition.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);

            if (Vector3.Distance(playerVarriables.leftArmHeader.position, playerVarriables.gunController.gunScript.safetyDevicePosition.position) < ErrorRange.changeArm)
            {
                break;
            }
            yield return null;
        }

        playerVarriables.smallSound.PlayOneShot(changeFireModeSound);
        playerVarriables.gunController.gunScript.changeFireMode();
        playerStatus.isChangingFireMode = false;

        while (true)
        {
            playerVarriables.leftArmHeader.position = Vector3.Slerp(
                playerVarriables.leftArmHeader.position,
                playerVarriables.gunController.gunScript.leftHandPosition.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);

            if (Vector3.Distance(playerVarriables.leftArmHeader.position, playerVarriables.gunController.gunScript.leftHandPosition.position) < ErrorRange.changeArm)
            {
                playerVarriables.leftArmController.setTarget(playerVarriables.gunController.gunScript.leftHandPosition);
                break;
            }
            yield return null;
        }
    }
}
