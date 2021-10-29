using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;

public class PlayerStatus : MonoBehaviour
{
    public bool isAim = false;
    public bool isReloading = false;
    public bool isChangingGun = false;
    public bool isPreparedInsertBulletInMag = false;
    public bool isWall = false;
    public bool isInsertingBulletInMag = false;
    public bool isChangingFireMode = false;
    public bool wasAim = false;
    public bool isSliderAnimation = false;
    public bool isTogglingTactical = false;
    public bool isFreeLook = false;
    public bool isJumping = false;
    public bool isRunning = false;
    public bool isMap = false;
    public bool isParachute = false;
    public bool isRight = false;
    public bool isGround = false;
    public bool isMoving = false;
    public bool isPeeking = false;
    public bool wasPeeking = false;
    public bool isGrenade = false;
    public bool isAddingAimBreakForce = false;
    public bool isIncrease = true;
    public bool wasIncrease = true;
    public bool isCrouch = false;
    public bool wasCrouch = false;
    public bool isCrawl = false;
    public bool wasCrawl = false;

    [HideInInspector]
    public bool isHitScan = false;
    [HideInInspector]
    public bool isMakeRecoil = true;
}
