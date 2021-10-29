using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;

public class ToggleTacticalAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerVarriables playerVarriables;
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    private AudioClip toggleTactical;

    private IEnumerator toggleTacticalI;

    public void playAnimation(bool isOn)
    {
        toggleTacticalI = toggleTacticalAnimation(isOn);
        //총기 발사모드 변경 애니메이션
        playerStatus.isTogglingTactical = true;
        StartCoroutine(toggleTacticalI);
    }

    private IEnumerator toggleTacticalAnimation(bool isOn)
    {

        playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;
        playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);

        while (true)
        {
            playerVarriables.leftArmHeader.position = Vector3.Slerp(
                playerVarriables.leftArmHeader.position,
                playerVarriables.gunController.gunScript.tacticalPosition.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);

            if (Vector3.Distance(playerVarriables.leftArmHeader.position, playerVarriables.gunController.gunScript.tacticalPosition.position) < ErrorRange.changeArm)
            {
                break;
            }
            yield return null;
        }

        playerVarriables.smallSound.PlayOneShot(toggleTactical);
        playerVarriables.gunController.gunScript.getTacticalScript().isOn = !isOn;
        playerStatus.isTogglingTactical = false;

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

    public void stopLastCoroutine()
    {
        try
        {
            StopCoroutine(toggleTacticalI);
        }
        catch
        {

        }
    }
}
