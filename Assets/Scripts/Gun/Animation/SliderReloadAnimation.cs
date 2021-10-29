using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;
using Photon.Pun;

public class SliderReloadAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;
    [SerializeField]
    private PlayerVarriables playerVarriables;

    public void playAnimation(float waitTime, float pullSpeed, float letSpeed)
    {
        StartCoroutine(sliderReloadAnimation(waitTime, pullSpeed, letSpeed));
    }

    //슬라이더 당기기 애니메이션
    private IEnumerator pullSlider(float pullSpeed)
    {
        playerVarriables.rightArmHeader.position = playerVarriables.gunController.gunScript.triggerPosition.position;
        playerVarriables.rightArmController.setTarget(playerVarriables.rightArmHeader);
        //오른쪽팔 슬라이더 집기
        while (true)
        {
            playerVarriables.rightArmHeader.position = Vector3.Lerp(
                playerVarriables.rightArmHeader.position,
                playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);
            if (Vector3.Distance(playerVarriables.rightArmHeader.position, playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position) < ErrorRange.changeArm)
            {
                playerVarriables.rightArmController.setTarget(playerVarriables.gunController.gunScript.sliderReloadAnimationHolder);
                break;
            }
            yield return null;
        }
        playerVarriables.smallSound.PlayOneShot(playerVarriables.gunController.gunScript.sliderPull);
        //슬라이더 당기기
        while (true)
        {
            playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position =
                Vector3.Lerp(
                    playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position,
                    playerVarriables.gunController.gunScript.sliderReloadAnimationEndPosition.position,
                    Time.deltaTime * pullSpeed);

            if (Vector2.Distance(playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position, playerVarriables.gunController.gunScript.sliderReloadAnimationEndPosition.position) < ErrorRange.sliderAnimation)
            {
                playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position = playerVarriables.gunController.gunScript.sliderReloadAnimationEndPosition.position;
                break;
            }

            yield return null;
        }
        if (playerVarriables.PV.IsMine)
        {
            //탄피 생성 스크립트
            if (playerVarriables.gunController.gunScript.bulletInChamber.Count > 0)
            {
                PhotonNetwork.Instantiate(Path.bulletsPrefabPath + "Bullet_" + playerVarriables.gunController.gunScript.bulletInChamber[0].bulletType, playerVarriables.gunController.gunScript.emptyShellPosition.transform.position, playerVarriables.gunController.gunScript.emptyShellPosition.transform.rotation);
                playerVarriables.gunController.gunScript.removeBulletInChamber();
            }
            if (playerVarriables.gunController.gunScript.isEmptyShellInChamber)
            {
                playerVarriables.gunController.gunScript.isEmptyShellInChamber = false;
                PhotonNetwork.Instantiate(Path.emptyShellPrefaPath + "EmptyShell_" + playerVarriables.gunController.gunScript.lastFired.bulletType, playerVarriables.gunController.gunScript.emptyShellPosition.transform.position, playerVarriables.gunController.gunScript.emptyShellPosition.transform.rotation);
            }
        }
    }

    //슬라이더 놓기 애니메이션
    private IEnumerator letSlider(float letSpeed)
    {
        playerVarriables.smallSound.PlayOneShot(playerVarriables.gunController.gunScript.sliderLet);
        //슬라이더 놓기
        while (true)
        {
            playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.localPosition =
                Vector3.Lerp(
                    playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.localPosition,
                    Vector3.zero,
                    Time.deltaTime * letSpeed);

            if (Vector2.Distance(playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.localPosition, Vector3.zero) < ErrorRange.sliderAnimation)
            {
                playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.localPosition = Vector3.zero;
                break;
            }

            yield return null;
        }
        if (playerVarriables.PV.IsMine)
        {
            playerVarriables.gunController.reloadChamber();
        }
        playerStatus.isReloading = false;
        playerVarriables.rightArmHeader.position = playerVarriables.gunController.gunScript.sliderReloadAnimationHolder.position;
        playerVarriables.rightArmController.setTarget(playerVarriables.rightArmHeader);
        //오른쪽팔 손잡이 집기
        while (true)
        {
            playerVarriables.rightArmHeader.position = Vector3.Lerp(
                playerVarriables.rightArmHeader.position,
                playerVarriables.gunController.gunScript.triggerPosition.position,
                Time.deltaTime * PlayerVarriables.armChangeTargetSpeed);
            if (Vector3.Distance(playerVarriables.rightArmHeader.position, playerVarriables.gunController.gunScript.triggerPosition.position) < ErrorRange.changeArm)
            {
                playerVarriables.rightArmController.setTarget(playerVarriables.gunController.gunScript.triggerPosition);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator pull;
    private IEnumerator let;

    //슬라이더 장전 애니메이션
    private IEnumerator sliderReloadAnimation(float waitTime, float pullSpeed, float letSpeed)
    {
        try
        {
            StopCoroutine(let);
        }
        catch { }

        pull = pullSlider(pullSpeed);
        let = letSlider(letSpeed);

        playerStatus.isSliderAnimation = true;
        StartCoroutine(pull);
        yield return new WaitForSeconds(waitTime);
        StopCoroutine(pull);
        //슬라이더 놓기
        StartCoroutine(let);
        playerStatus.isSliderAnimation = false;
        yield break;
    }
}
