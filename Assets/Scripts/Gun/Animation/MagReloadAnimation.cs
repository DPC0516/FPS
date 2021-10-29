using System.Collections;
using UnityEngine;
using Varriables;

public class MagReloadAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    private PlayerVarriables playerVarriables;
    [SerializeField]
    private Transform magReloadAnimationEndPosition;

    public void stopLastCoroutine()
    {
        try
        {
            StopCoroutine(magIn);
        }
        catch
        {

        }
    }

    private void reloadMag(GameObject originalMag)
    {
        playerVarriables.gunController.gunScript.currentBurstCount = 0;
        playerVarriables.gunController.gunScript.magInstantiate = originalMag;
    }

    public void playAnimation(int index, bool isTargetNone)
    {
        StartCoroutine(magReloadAnimation(index, isTargetNone));
    }

    //탄창 빼기 애니메이션
    private IEnumerator magOutAnimation()
    {

        Transform grabPosition = playerVarriables.gunController.gunScript.getMagScript().magGrabPosition;
        if (playerVarriables.gunController.gunScript.getMagScript().magName == "None")
        {
            playerVarriables.gunController.gunScript.magReloadAnimationHolder.position = playerVarriables.leftArmController.target.position;
            playerVarriables.leftArmController.setTarget(playerVarriables.gunController.gunScript.magReloadAnimationHolder);
        }
        //총기 탄창이 없지 않을시
        else
        {
            playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;
            playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);
            //왼쪽팔 탄창 집기
            while (true)
            {
                playerVarriables.leftArmHeader.position = Vector3.Lerp(
                    playerVarriables.leftArmHeader.position,
                    grabPosition.position,
                    Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);
                if (Vector3.Distance(playerVarriables.leftArmHeader.position, grabPosition.position) < ErrorRange.changeArm)
                {
                    playerVarriables.leftArmController.setTarget(grabPosition);
                    break;
                }
                yield return null;
            }
            playerVarriables.smallSound.PlayOneShot(playerVarriables.gunController.gunScript.magOut);
            //탄창 빼기
            while (true)
            {
                playerVarriables.gunController.gunScript.magReloadAnimationHolder.position =
                    Vector3.Lerp(
                        playerVarriables.gunController.gunScript.magReloadAnimationHolder.position,
                        playerVarriables.gunController.gunScript.magOutPosition.position,
                        Time.deltaTime * playerVarriables.gunController.gunScript.speed[Speed.magInOutSpeed]);

                if (Vector3.Distance(playerVarriables.gunController.gunScript.magReloadAnimationHolder.position, playerVarriables.gunController.gunScript.magOutPosition.position) < ErrorRange.magReload)
                {
                    playerVarriables.gunController.gunScript.magReloadAnimationHolder.position = playerVarriables.gunController.gunScript.magOutPosition.position;
                    break;
                }

                yield return null;
            }
        }
        //탄창 호주머니로
        while (true)
        {
            playerVarriables.gunController.gunScript.magReloadAnimationHolder.position =
                Vector3.Lerp(
                    playerVarriables.gunController.gunScript.magReloadAnimationHolder.position,
                    magReloadAnimationEndPosition.position,
                    Time.deltaTime * playerVarriables.gunController.gunScript.speed[Speed.magToPocketSpeed]);

            if (Vector3.Distance(playerVarriables.gunController.gunScript.magReloadAnimationHolder.position, magReloadAnimationEndPosition.position) < ErrorRange.magReload)
            {
                playerVarriables.gunController.gunScript.magReloadAnimationHolder.position = magReloadAnimationEndPosition.position;
                break;
            }

            yield return null;
        }
        yield break;
    }

    //탄창 넣기 애니메이션
    private IEnumerator magInAnimation(int index, bool isTargetNone)
    {
        Public.setParent(playerVarriables.gunController.gunScript.magInstantiate.transform, playerVarriables.magPositions[index].transform, Vector3.zero, Quaternion.identity);
        GameObject originalMag = playerVarriables.magPositions[index].GetComponent<MagPosition>().mag;
        playerVarriables.magPositions[index].GetComponent<MagPosition>().mag = playerVarriables.gunController.gunScript.magInstantiate;
        Transform grabPosition = originalMag.GetComponent<Mag>().magGrabPosition;

        Public.setParent(originalMag.transform, playerVarriables.gunController.gunScript.magReloadAnimationHolder, Vector3.zero, Quaternion.identity);

        //타겟에 탄창이 없을시
        if (isTargetNone)
        {
            playerVarriables.gunController.gunScript.magReloadAnimationHolder.transform.localPosition = Vector3.zero;
            playerVarriables.gunController.gunScript.magReloadAnimationHolder.transform.localRotation = Quaternion.identity;
        }
        else
        {
            playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;
            playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);
            //왼쪽팔 탄창 집기
            while (true)
            {
                playerVarriables.leftArmHeader.position = Vector3.Lerp(
                    playerVarriables.leftArmHeader.position,
                    grabPosition.position,
                    Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);
                if (Vector3.Distance(playerVarriables.leftArmHeader.position, grabPosition.position) < ErrorRange.changeArm)
                {
                    playerVarriables.leftArmController.setTarget(grabPosition);
                    break;
                }
                yield return null;
            }
            //탄창 호주머니에서 뺴기
            while (true)
            {
                playerVarriables.gunController.gunScript.magReloadAnimationHolder.position =
                    Vector3.Lerp(
                        playerVarriables.gunController.gunScript.magReloadAnimationHolder.position,
                        playerVarriables.gunController.gunScript.magOutPosition.position,
                        Time.deltaTime * playerVarriables.gunController.gunScript.speed[Speed.magToPocketSpeed]);

                if (Vector3.Distance(playerVarriables.gunController.gunScript.magReloadAnimationHolder.position, playerVarriables.gunController.gunScript.magOutPosition.position) < ErrorRange.magReload)
                {
                    playerVarriables.gunController.gunScript.magReloadAnimationHolder.position = playerVarriables.gunController.gunScript.magOutPosition.position;
                    break;
                }

                yield return null;
            }

            playerVarriables.smallSound.PlayOneShot(playerVarriables.gunController.gunScript.magIn);

            //탄창 넣기
            while (true)
            {
                playerVarriables.gunController.gunScript.magReloadAnimationHolder.localPosition =
                    Vector3.Lerp(
                        playerVarriables.gunController.gunScript.magReloadAnimationHolder.localPosition,
                        Vector3.zero,
                        Time.deltaTime * playerVarriables.gunController.gunScript.speed[Speed.magInOutSpeed]);

                if (Vector3.Distance(playerVarriables.gunController.gunScript.magReloadAnimationHolder.localPosition, Vector3.zero) < ErrorRange.magReload)
                {
                    playerVarriables.gunController.gunScript.magReloadAnimationHolder.localPosition = Vector3.zero;
                    break;
                }

                yield return null;
            }
        }

        reloadMag(originalMag);
        playerStatus.isReloading = false;

        playerVarriables.leftArmHeader.position = playerVarriables.leftArmController.target.position;
        playerVarriables.leftArmController.setTarget(playerVarriables.leftArmHeader);
        while (true)
        {
            playerVarriables.leftArmHeader.position = Vector3.Lerp(
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
        yield break;
    }

    private IEnumerator magOut;
    private IEnumerator magIn;

    //탄창 장전 애니메이션
    private IEnumerator magReloadAnimation(int index, bool isTargetNone)
    {
        stopLastCoroutine();

        magReloadAnimationEndPosition.position = playerVarriables.magPositions[index].position;

        magOut = magOutAnimation();
        magIn = magInAnimation(index, isTargetNone);

        StartCoroutine(magOut);
        yield return new WaitForSeconds(playerVarriables.gunController.gunScript.speed[Speed.reloadTime]);
        StopCoroutine(magOut);
        StartCoroutine(magIn);
        yield break;
    }
}
