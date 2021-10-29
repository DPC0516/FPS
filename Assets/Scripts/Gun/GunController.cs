//네임스페이스 선언
using UnityEngine;
using Varriables;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

//총기 관리 클래스
public class GunController : MonoBehaviourPunCallbacks
{
    //****************************************************************************** 변수

    //탄창 없음 프리팹
    [SerializeField]
    private GameObject noneMagPrefab;
    //총알 프리팹
    [SerializeField]
    private GameObject bulletPrefab;

    //총기 홀더 Transform
    public Transform gunHolder;
    //반동 조작 Transform
    [SerializeField]
    private Transform recoilHolder;
    //반동 조작 헤더 Transform
    [SerializeField]
    private Transform recoilHolderHeader;
    //반동 조작 Transform
    [SerializeField]
    private Transform recoilGunHolder;
    //총기 흔들림 조작 Transform
    [SerializeField]
    private Transform gunHolderSwayHolder;
    //레이캐스트 위치
    [SerializeField]
    private Transform raycastPosition;
    //총 최대 사거리
    [SerializeField]
    private Transform gunRange;

    //플레이어 카메라들
    [SerializeField]
    private Camera[] playerCamera;

    //탄약 없음 소리
    [SerializeField]
    private AudioClip empty;
    //조준 소리
    [SerializeField]
    private AudioClip aim;

    //UI
    [SerializeField]
    private Text TextUI_bulletReloaded;
    [SerializeField]
    private Text TextUI_bulletLeft;
    [SerializeField]
    private Text TextUI_fireMode;
    [SerializeField]
    private Text TextUI_bulletInChamber;
    [SerializeField]
    private Text TextUI_isJam;

    [HideInInspector]
    //현재 총 스크립트
    public Gun gunScript;

    private int cartridgeNum = Public.maxCartridge;

    //플레이어 공용 변수
    [SerializeField]
    private PlayerVarriables playerVarriables;
    //플레이어 상태
    [SerializeField]
    private PlayerStatus playerStatus;

    //애니매이션 변수
    [SerializeField]
    private MagReloadAnimation magReloadAnimation;
    [SerializeField]
    private SliderReloadAnimation sliderReloadAnimation;
    [SerializeField]
    private ChangeFireModeAnimation changeFireModeAnimation;
    [SerializeField]
    private ToggleTacticalAnimation toggleTacticalAnimation;
    [SerializeField]
    private ChangeGunAnimation changeGunAnimation;
    [SerializeField]
    private InsertBulletInMagAnimation insertBulletInMagAnimation;

    //****************************************************************************** 유니티 기본 함수

    private void Start()
    {
        //현재 오브젝트가 나 자신일때
        if (playerVarriables.PV.IsMine)
        {
            //초기화
            init();
        }
    }

    private void Update()
    {
        //정지 상태가 아니면
        if (!Public.isPause)
        {
            //현재 오브젝트가 나 자신일때
            if (playerVarriables.PV.IsMine)
            {
                if (!playerStatus.isChangingGun && !playerStatus.isPreparedInsertBulletInMag )
                {
                    if (!playerStatus.isRunning
                        && !(gunScript.fireMode == FireMode.FIREMODE_SAFE))
                    {
                        //발사 확인
                        checkFire();
                    }
                    if (!playerStatus.isReloading && !playerStatus.isChangingFireMode && !playerStatus.isTogglingTactical)
                    {
                        //장전 확인
                        checkReload();
                        //발사모드 변경 확인
                        checkChangeFireMode();
                        //전술 도구 모드 변경 확인
                        checkTactical();
                    }
                    //총기 흔들림
                    gunSway();
                }
                if (!playerStatus.isChangingGun && !playerStatus.isReloading)
                {
                    checkInsertBulletInMag();
                }
            }
        }

        if (playerVarriables.PV.IsMine)
        {
            //UI 업데이트
            setTextUI();
            //정조준 확인
            checkAim();
            if (gunScript.bulletInChamber.Count == 0)
            {
                gunScript.isEmpty = true;
            }
            else
            {
                gunScript.isEmpty = false;
            }
        }

        //총기 홀더 방향 업데이트
        setGunHolderRotation();
        //반동 부드럽게 업데이트
        setRecoilHolderRotation();

        try
        {
            //반동 초기화
            restoreRecoil();
        }
        catch { }
    }

    //****************************************************************************** 초기화 함수

    //초기화
    private void init()
    {
        WeaponLoadout _weaponLoadout = Public.getWeaponLoadout(playerVarriables.PV, playerVarriables.weaponLoadout);

        gunScript = _weaponLoadout.getGunScript(_weaponLoadout.currentWeaponType);

        //총기를 총기 거치대로 이동
        Public.setParent(_weaponLoadout.primaryWeapon.transform, playerVarriables.bag.primaryGunHolder, Vector3.zero, Quaternion.identity);

        Public.setParent(_weaponLoadout.secondaryWeapon.transform, playerVarriables.bag.secondaryGunHolder, Vector3.zero, Quaternion.identity);

        //탄창 탄창 거치대로 이동
        for (int i = 0; i < _weaponLoadout.mags.Count; i++)
        {
            Public.setParent(_weaponLoadout.mags[i].transform, playerVarriables.magPositions[i].transform, Vector3.zero, Quaternion.identity);
            playerVarriables.magPositions[i].GetComponent<MagPosition>().mag = _weaponLoadout.mags[i];
        }

        //현재 총기를 GunHolder에 할당
        if (_weaponLoadout.currentWeaponType == WeaponType.primary)
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, gunHolder, Vector3.zero, Quaternion.identity);

            Public.setParent(_weaponLoadout.secondaryWeapon.transform, playerVarriables.bag.secondaryGunHolder, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Public.setParent(_weaponLoadout.primaryWeapon.transform, playerVarriables.bag.primaryGunHolder, Vector3.zero, Quaternion.identity);

            Public.setParent(_weaponLoadout.secondaryWeapon.transform, gunHolder, Vector3.zero, Quaternion.identity);
        }

        //네트워크 동기화를 위한 변수 전달
        if (playerVarriables.PV.IsMine)
        {
            List<Mag> magList = _weaponLoadout.getMagScripts(WeaponType.weaponTypes[WeaponType.all]);
            string[] mags = new string[magList.Count];

            for (int i = 0; i < magList.Count; i++)
            {
                mags[i] = magList[i].magName;
            }

            playerVarriables.PV.RPC("initRPC", RpcTarget.OthersBuffered,
                _weaponLoadout.getGunScript(WeaponType.primary).gunName,
                _weaponLoadout.getGunScript(WeaponType.secondary).gunName,
                _weaponLoadout.getGunScript(WeaponType.primary).getSightScript().sightName,
                _weaponLoadout.getGunScript(WeaponType.secondary).getSightScript().sightName,
                _weaponLoadout.getGunScript(WeaponType.primary).getTacticalScript().tacticalName,
                _weaponLoadout.getGunScript(WeaponType.secondary).getTacticalScript().tacticalName,
                _weaponLoadout.getGunScript(WeaponType.primary).getMuzzleScript().muzzleName,
                _weaponLoadout.getGunScript(WeaponType.secondary).getMuzzleScript().muzzleName,
                _weaponLoadout.getGunScript(WeaponType.primary).getHandleScript().handleName,
                _weaponLoadout.getGunScript(WeaponType.secondary).getHandleScript().handleName,
                mags); ;
        }
        initGun();
    }

    //초기화 RPC
    [PunRPC]
    private void initRPC(
        string primaryGunName, 
        string secondaryGunName,
        string primarySightName,
        string secondarySightName, 
        string primaryTacticalName, 
        string secondaryTacticalName, 
        string primaryMuzzleName,
        string secondaryMuzzleName,
        string primaryHandleName,
        string secondaryHandleName,
        string[] mags)
    {
        playerVarriables.weaponLoadout.initMag();

        //총기 초기화
        GameObject primaryGun = Path.loadGun(primaryGunName);
        GameObject secondaryGun = Path.loadGun(secondaryGunName);

        playerVarriables.weaponLoadout.primaryWeapon = Instantiate(primaryGun, Vector3.down * 2, Quaternion.identity);
        playerVarriables.weaponLoadout.secondaryWeapon = Instantiate(secondaryGun, Vector3.down * 2, Quaternion.identity);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).init();
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).init();
        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).initGun();
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).initGun();

        //탄창 초기화
        for (int i = 0; i < mags.Length; i++)
        {
            GameObject mag = Path.loadMag(mags[i]);
            playerVarriables.weaponLoadout.mags[i] = Instantiate(mag, Vector3.down * 2, Quaternion.identity);
            playerVarriables.weaponLoadout.getMagScript(i).init(i, playerVarriables.weaponLoadout.getMagScript(i).bulletMaxReloaded);
        }

        GameObject primaryWeaponMag = Instantiate(noneMagPrefab, Vector3.down * 2, Quaternion.identity);
        GameObject secondaryWeaponMag = Instantiate(noneMagPrefab, Vector3.down * 2, Quaternion.identity);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).magInstantiate = primaryWeaponMag;
        Public.setParent(
            playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).magInstantiate.transform,
            playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).magReloadAnimationHolder,
            Vector3.zero,
            Quaternion.identity);
        ComponentLoader.getMagScript(primaryWeaponMag).init(Public.maxMag + 1, 0);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).magInstantiate = secondaryWeaponMag;

        Public.setParent(
            playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).magInstantiate.transform,
            playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).magReloadAnimationHolder,
            Vector3.zero,
            Quaternion.identity);
        ComponentLoader.getMagScript(secondaryWeaponMag).init(Public.maxMag + 2, 0);

        //조준경 초기화
        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).sightPrefab = Path.loadSight(primarySightName);
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).sightPrefab = Path.loadSight(secondarySightName);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).initSight();
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).initSight();

        //전술 도구 초기화
        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).tacticalPrefab = Path.loadTactical(primaryTacticalName);
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).tacticalPrefab = Path.loadTactical(secondaryTacticalName);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).initTactical(WeaponType.primary);
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).initTactical(WeaponType.secondary);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).muzzlePrefab = Path.loadMuzzle(primaryMuzzleName);
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).muzzlePrefab = Path.loadMuzzle(secondaryMuzzleName);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).initMuzzle();
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).initMuzzle();

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).handlePrefab = Path.loadHandle(primaryHandleName);
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).handlePrefab = Path.loadHandle(secondaryHandleName);

        playerVarriables.weaponLoadout.getGunScript(WeaponType.primary).initHandle();
        playerVarriables.weaponLoadout.getGunScript(WeaponType.secondary).initHandle();

        init();
    }

    //총기 초기화
    public void initGun()
    {
        //코루틴 종료
        StopAllCoroutines();

        gunRange.localPosition = new Vector3(0, 0, gunScript.range);
        gunRange.localRotation = Quaternion.identity;

        //손 타겟 초기화
        playerVarriables.leftArmController.setTarget(gunScript.leftHandPosition);
        playerVarriables.rightArmController.setTarget(gunScript.triggerPosition);
    }

    //****************************************************************************** 총기 변경 함수

    //총기 변경
    public void changeGun(int toWeaponType)
    {
        changeGunAnimation.playAnimation(toWeaponType);

        //네트워크 동기화를 위한 변수 전달
        if (playerVarriables.PV.IsMine)
        {
            playerVarriables.PV.RPC("changeGunRPC", RpcTarget.OthersBuffered,
                toWeaponType);
        }
    }

    //총기 변경 RPC
    [PunRPC]
    private void changeGunRPC(int toWeaponType)
    {
        changeGun(toWeaponType);
    }

    //****************************************************************************** 업데이트 함수

    //UI 업데이트
    private void setTextUI()
    {
        List<Mag> mags = Public.weaponLoadout.getMagScripts(gunScript.gunName);
        int bulletLeft = 0;
        for(int i = 0; i < mags.Count; i++)
        {
            bulletLeft += mags[i].bulletReloaded.Count;
        }

        TextUI_isJam.enabled = gunScript.isJam;

        TextUI_bulletReloaded.text = gunScript.getMagScript().bulletReloaded.Count + "/" + gunScript.getMagScript().bulletMaxReloaded;

        TextUI_bulletInChamber.text = "+" + gunScript.bulletInChamber.Count;

        TextUI_bulletLeft.text = bulletLeft.ToString();

        TextUI_fireMode.text = FireMode.FIREMODE_STRING_LIST[gunScript.fireMode];
    }

    //총기 반동 업데이트
    private void setRecoilHolderRotation()
    {
        //반동을 부드럽게 표현
        recoilHolder.transform.localRotation =
            Quaternion.Slerp(
                recoilHolder.transform.localRotation,
                recoilHolderHeader.transform.localRotation,
                50 * Time.deltaTime);
    }

    //총기 방향 업데이트
    private void setGunHolderRotation()
    {
        //전방에 장애물이 있을시 총구 위로
        RaycastHit hit;
        if(Physics.Raycast(raycastPosition.transform.position, raycastPosition.transform.rotation * Vector3.forward, out hit, gunScript.isWallLength))
        {
            if (!hit.collider.CompareTag("PlayerCollider") && !hit.collider.CompareTag("EmptyShell") && !hit.collider.CompareTag("Bullet"))
            {
                gunHolder.transform.rotation =
                        Quaternion.Slerp(gunHolder.transform.rotation,
                        Quaternion.Euler(-90f, gunHolder.transform.rotation.eulerAngles.y, gunHolder.transform.rotation.eulerAngles.z),
                        PlayerVarriables.smoothAmount * Time.deltaTime);
                playerStatus.isWall = true;
            }
        }
        else
        {
            playerStatus.isWall = false;
        }

        //정조준이 아닐시 총구를 피격당하는 쪽으로
        if (!playerStatus.isAim && !playerStatus.isWall)
        {
            if (Physics.Raycast(raycastPosition.transform.position, raycastPosition.transform.rotation * Vector3.forward, out hit, gunScript.range))
            {
                Vector3 vec = hit.point - gunHolder.transform.position;
                vec.Normalize();
                Quaternion q = Quaternion.LookRotation(vec);
                gunHolder.transform.rotation =
                    Quaternion.Slerp(gunHolder.transform.rotation,
                    q,
                    PlayerVarriables.smoothAmount * Time.deltaTime);
            }
            else
            {
                Vector3 vec = gunRange.position - gunHolder.transform.position;
                vec.Normalize();
                Quaternion q = Quaternion.LookRotation(vec);
                gunHolder.transform.rotation =
                    Quaternion.Slerp(gunHolder.transform.rotation,
                    q,
                    PlayerVarriables.smoothAmount * Time.deltaTime);
            }
        }
    }

    //정조준 상태 확인
    private void checkAim()
    {
        Vector3 aimPosition = gunScript.gunAimPosition;
        Vector3 defaultPosition = gunScript.gunDefaultPosition;

        if(playerStatus.isAim != playerStatus.wasAim)
        {
            playerVarriables.PV.RPC("playAimSound", RpcTarget.All);
        }
        playerStatus.wasAim = playerStatus.isAim;

        if (Input.GetMouseButton(Key.Aim) 
            && !playerStatus.isJumping
            && !playerStatus.isFreeLook 
            && !playerStatus.isChangingGun 
            && !playerStatus.isChangingFireMode 
            && !playerStatus.isPreparedInsertBulletInMag
            && !playerStatus.isWall
            && !Public.isPause
            && !playerStatus.isReloading
            && !playerStatus.isParachute
            )
        {
            playerStatus.isAim = true;

            float y = 
                aimPosition.y + 
                (gunScript.gunHeaderPosition.localPosition.y - gunScript.sightPosition.localPosition.y) 
                - gunScript.getSightScript().sightCenter.localPosition.y;


            //정조준 상태로 바꾸기
            Vector3 aimPositionWithSight = new Vector3(aimPosition.x, y, aimPosition.z);
            
            gunHolder.transform.localPosition = Vector3.Slerp(
                gunHolder.transform.localPosition,
                aimPositionWithSight,
                Time.deltaTime * gunScript.speed[Speed.aimSpeed]);

            gunHolder.transform.localRotation = Quaternion.Slerp(
                gunHolder.transform.localRotation,
                Quaternion.identity,
                Time.deltaTime * gunScript.speed[Speed.aimSpeed]);

            if (Vector3.Distance(gunHolder.transform.localPosition, aimPositionWithSight) < ErrorRange.aimPosition)
            {
                gunHolder.transform.localPosition = aimPositionWithSight;
                gunHolder.transform.localRotation = Quaternion.identity;
            }   

            //카메라 FOV 조절
            for (int i = 0; i < playerCamera.Length; i++)
            {
                playerCamera[i].fieldOfView = Mathf.Lerp(
                    playerCamera[i].fieldOfView,
                    Public.defaultFOV * gunScript.getSightScript().sightFOVMultipler,
                    Time.deltaTime * gunScript.speed[Speed.aimSpeed]);
            }

        }
        else
        {
            playerStatus.isAim = false;
            
            //정조준 상태 풀기
            gunHolder.transform.localPosition = Vector3.Slerp(
                gunHolder.transform.localPosition, 
                defaultPosition, 
                Time.deltaTime * gunScript.speed[Speed.aimSpeed]); 
            
            if (Vector3.Distance(gunHolder.transform.localPosition, defaultPosition) < ErrorRange.aimPosition)
            {
                gunHolder.transform.localPosition = defaultPosition;
            }

            //카메라 FOV 조절
            for (int i = 0; i < playerCamera.Length; i++)
            {
                playerCamera[i].fieldOfView = Mathf.Lerp(
                    playerCamera[i].fieldOfView,
                    Public.defaultFOV,
                    Time.deltaTime * gunScript.speed[Speed.aimSpeed]);
            }
        }
    }

    //장전 확인
    private void checkReload()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //약실 장전 확인
            if (Input.GetKey(Key.ReloadChamber))
            {
                if (!playerStatus.isReloading)
                {
                    playerStatus.isReloading = true;
                    playerVarriables.PV.RPC("sliderReloadAnimationRPC", RpcTarget.All,
                        gunScript.speed[Speed.reloadChamberTime],
                        gunScript.speed[Speed.sliderPullSpeed],
                        gunScript.speed[Speed.sliderLetSpeed]);
                }
            }
        }
        //탄창 장전 확인
        if (Input.GetKey(Key.Relaod))
        {
            for (int i = 0; i < Key.Alphas.Length; i++)
            {
                if (Input.GetKey(Key.Alphas[i]))
                {
                    reloadAnimation(i);
                }
            }
        }
    }

    //장전 애니메이션
    private void reloadAnimation(int index)
    {
        if (gunScript.isMagAvailable(playerVarriables.magPositions[index].GetComponent<MagPosition>().getMagScript().magName))
        {
            playerStatus.isReloading = true;
            playerVarriables.PV.RPC("magReloadAnimationRPC", RpcTarget.All, index, false);
        }
        else if (playerVarriables.magPositions[index].GetComponent<MagPosition>().getMagScript().magName == "None")
        {
            if (gunScript.getMagScript().magName == "None")
            {
                return;
            }
            playerStatus.isReloading = true;
            playerVarriables.PV.RPC("magReloadAnimationRPC", RpcTarget.All, index, true);
        }
    }

    //전술 도구 확인
    private void checkTactical()
    {
        if (Input.GetKeyDown(Key.Tactical))
        {
            if (gunScript.getTacticalScript().tacticalName != "None")
            {
                playerVarriables.PV.RPC("toggleTacticalRPC", RpcTarget.All, gunScript.getTacticalScript().isOn);
            }
        }
    }

    //총기 발사 확인
    private void checkFire()
    {
        if (Input.GetMouseButtonDown(Key.Fire) && (gunScript.isEmpty || gunScript.isJam))
        {
            playerVarriables.PV.RPC("playEmptySoundOneShot", RpcTarget.All);
        }
        if (gunScript.bulletInChamber.Count > 0 && !playerStatus.isReloading && !gunScript.isJam)
        {
            if (gunScript.fireMode == FireMode.FIREMODE_SEMI)
            {
                if (Input.GetMouseButtonDown(Key.Fire))
                {
                    fire();
                }
            }
            else
            {
                if (Input.GetMouseButton(Key.Fire))
                {
                    if (gunScript.fireMode == FireMode.FIREMODE_BURST)
                    {
                        if (gunScript.currentBurstCount < gunScript.burstCount)
                        {
                            gunScript.currentBurstCount++;
                            fire();

                        }
                    }
                    else
                    {
                        fire();
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(Key.Fire))
        {
            gunScript.currentBurstCount = 0;
        }
    }

    //총기 발사모드 변경 확인
    private void checkChangeFireMode()
    {
        if (Input.GetKeyDown(Key.ChangeFireMode))
        {
            playerVarriables.PV.RPC("changeFireModeRPC", RpcTarget.All);
        }
    }

    //총기 흔들림
    private void gunSway()
    {
        if (playerStatus.isAim)
        {
            float positionX = -Input.GetAxis("Mouse X") * PlayerVarriables.aimSwayAmount;
            float positionY = -Input.GetAxis("Mouse Y") * PlayerVarriables.aimSwayAmount;

            Mathf.Clamp(positionX, -PlayerVarriables.aimMaxAmount, PlayerVarriables.aimMaxAmount);
            Mathf.Clamp(positionY, -PlayerVarriables.aimMaxAmount, PlayerVarriables.aimMaxAmount);

            Vector3 swayPosition = new Vector3(positionX, positionY, 0f);

            gunHolderSwayHolder.transform.localPosition =
                Vector3.Lerp(
                    gunHolderSwayHolder.transform.localPosition,
                    swayPosition, Time.deltaTime *
                    PlayerVarriables.aimSmoothAmount);
        }
        else
        {
            float positionX = -Input.GetAxis("Mouse X") * PlayerVarriables.swayAmount;
            float positionY = -Input.GetAxis("Mouse Y") * PlayerVarriables.swayAmount;

            Mathf.Clamp(positionX, -PlayerVarriables.maxAmount, PlayerVarriables.maxAmount);
            Mathf.Clamp(positionY, -PlayerVarriables.maxAmount, PlayerVarriables.maxAmount);

            Vector3 swayPosition = new Vector3(positionX, positionY, 0f);

            gunHolderSwayHolder.transform.localPosition =
                Vector3.Lerp(
                    gunHolderSwayHolder.transform.localPosition,
                    swayPosition, Time.deltaTime *
                    PlayerVarriables.smoothAmount);
        }
    }

    private int targetNum = 0;

    //탄창에 총알 넣기 체크
    private void checkInsertBulletInMag()
    {
        if (!playerStatus.isPreparedInsertBulletInMag && !playerStatus.isParachute)
        {
            if (Input.GetKey(Key.InsertBulletInMag))
            {
                for (int i = 0; i < Key.Alphas.Length; i++)
                {
                    if (Input.GetKey(Key.Alphas[i]))
                    {
                        if (!(playerVarriables.magPositions[i].GetComponent<MagPosition>().getMagScript().magName == "None"))
                        {
                            cartridgeNum = Public.weaponLoadout.bulletCartridges.Count;
                            targetNum = i;
                            playerVarriables.PV.RPC("prepareInsertBulletInMagAnimationRPC", RpcTarget.All, i);
                        }
                    }
                }
            }
        }
        else
        {
            if (playerStatus.isPreparedInsertBulletInMag)
            {
                if (!playerStatus.isInsertingBulletInMag)
                {
                    if (Input.GetKey(Key.Cancel))
                    {
                        playerVarriables.PV.RPC("unprepareInsertBulletInMagAnimationRPC", RpcTarget.All, targetNum);
                    }
                    else
                    {
                        if (cartridgeNum != Public.weaponLoadout.bulletCartridges.Count)
                        {
                            if (
                                playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript().isBulletAvailable(Public.weaponLoadout.bulletCartridges[cartridgeNum].bulletType)
                                && playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript().bulletReloaded.Count < playerVarriables.magPositions[targetNum].GetComponent<MagPosition>().getMagScript().bulletMaxReloaded
                                && Public.weaponLoadout.bulletCartridges[cartridgeNum].bulletCount > 0)
                            {
                                playerVarriables.PV.RPC("insertBulletInMagAnimationRPC", RpcTarget.All, cartridgeNum, targetNum, Public.weaponLoadout.bulletCartridges[cartridgeNum].bulletType);
                            }
                        }
                    }
                }
                for (int i = 0; i < Public.weaponLoadout.bulletCartridges.Count; i++)
                {
                    if (Input.GetKey(Key.Alphas[i]))
                    {
                        cartridgeNum = i;
                        return;
                    }
                }
                cartridgeNum = Public.weaponLoadout.bulletCartridges.Count;
            }
            else
            {
                cartridgeNum = Public.weaponLoadout.bulletCartridges.Count;
            }
        }
    }

    //****************************************************************************** 애니매이션 함수

    private void stopLastCoroutines()
    {
        magReloadAnimation.stopLastCoroutine();
        changeFireModeAnimation.stopLastCoroutine();
        toggleTacticalAnimation.stopLastCoroutine();
    }

    [PunRPC]
    private void changeFireModeRPC()
    {
        stopLastCoroutines();

        changeFireModeAnimation.playAnimation();
    }

    [PunRPC]
    private void toggleTacticalRPC(bool isOn)
    {
        stopLastCoroutines();

        toggleTacticalAnimation.playAnimation(isOn);
    }

    [PunRPC]
    private void magReloadAnimationRPC(int index, bool isTargetNone)
    {
        stopLastCoroutines();

        magReloadAnimation.playAnimation(index, isTargetNone);
    }

    [PunRPC]
    private void sliderReloadAnimationRPC(float waitTime, float pullSpeed, float letSpeed)
    {
        sliderReloadAnimation.playAnimation(waitTime, pullSpeed, letSpeed);
    }

    //****************************************************************************** 약실 장전 함수

    //약실 지연 장전
    private IEnumerator reloadChamberDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        reloadChamber();
    }

    //약실 장전
    public void reloadChamber()
    {
       gunScript.isJam = false;
        int bulletToReloadInChamber = gunScript.bulletMaxInChamber - gunScript.bulletInChamber.Count;
        if (playerVarriables.gunController.gunScript.getMagScript().bulletReloaded.Count != 0)
        {
            for (int i = 0; i < bulletToReloadInChamber; i++)
            {
                if (!gunScript.isJam)
                {
                    gunScript.isJam = !gunScript.insertBulletInChamber(gunScript.getMagScript().removeBullet());
                }
            }
        }
    }

    //****************************************************************************** 탄창 채우기 함수

    [PunRPC]
    private void prepareInsertBulletInMagAnimationRPC(int targetNum)
    {
        insertBulletInMagAnimation.playPrepareAnimation(targetNum);
    }

    [PunRPC]
    private void unprepareInsertBulletInMagAnimationRPC(int targetNum)
    {
        insertBulletInMagAnimation.playUnPrepareAnimation(targetNum);
    }

    [PunRPC]
    private void insertBulletInMagAnimationRPC(int _cartridgeNum, int targetNum, string bulletType)
    {
        insertBulletInMagAnimation.playInsertAnimation(_cartridgeNum, targetNum, bulletType);
    }


    //****************************************************************************** 발사 함수

    //발사
    private void fire()
    {
        //총알 발사
        bulletFire();
        //총구 화염 파티클 실행
        gunScript.getMuzzleScript().play();
        //총구 소음 재생
        if (gunScript.getMuzzleScript().isSilencer)
        {
            playerVarriables.gunSilencerSound.PlayOneShot(gunScript.silencerFireSound);
        }
        else
        {
            playerVarriables.gunSound.PlayOneShot(gunScript.fireSound);
        }
        gunScript.isEmptyShellInChamber = true;
        //약실 총알 감소
        gunScript.lastFired = gunScript.removeBulletInChamber();
        if (playerStatus.isMakeRecoil)
        {
            //반동 재생
            makeRecoil(gunScript.lastFired.bulletType);
        }

        //약실 장전
        if (gunScript.bulletInChamber.Count == 0)
        {
            if(gunScript.fireMode == FireMode.FIREMODE_BURST)
            {
                StartCoroutine(reloadChamberDelay(gunScript.speed[Speed.burstSpeed]));
            }
            else
            {
                if (gunScript.isHalfAuto)
                {
                    StartCoroutine(reloadChamberDelay(gunScript.speed[Speed.fireSpeed]));
                }
            }
        }
        //발사 RPC
        playerVarriables.PV.RPC("fireNetwork", RpcTarget.Others, gunScript.lastFired.bulletType, playerStatus.isMakeRecoil);
    }

    //발사 RPC
    [PunRPC]
    public void fireNetwork(string emptyShell, bool isMakeRecoil)
    {
        //총구 화염 파티클 실행
        gunScript.getMuzzleScript().play();
        //총구 소음 재생
        if (gunScript.getMuzzleScript().isSilencer)
        {
            playerVarriables.gunSilencerSound.PlayOneShot(gunScript.silencerFireSound);
        }
        else
        {
            playerVarriables.gunSound.PlayOneShot(gunScript.fireSound);
        }
        if (isMakeRecoil)
        {
            //반동 재생
            makeRecoil(emptyShell);
        }
    }

    //총알 발사
    private void bulletFire()
    {
        if (playerStatus.isHitScan)
        {
#pragma warning disable CS0162
            GameObject bulletTrajectoryInstantiate = PhotonNetwork.Instantiate(Path.bulletPrefabPath + "BulletTrajectory", Vector3.zero, Quaternion.identity);
            if (playerStatus.isAim)
            {
                bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().fireAngle = gunScript.getSightScript().sightCenter.rotation;
                bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().fireAngleForward =
                    new Vector3(gunScript.getSightScript().sightCenter.forward.x, 0f, gunScript.getSightScript().sightCenter.forward.z);
                bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().firePosition = gunScript.getSightScript().sightCenter.position;
            }
            else
            {
                bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().fireAngle = gunHolder.rotation;
                bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().fireAngleForward =
                    new Vector3(gunHolder.forward.x, 0f, gunHolder.forward.z);
                bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().firePosition = gunHolder.position;
            }
            bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().firePositionGun = gunScript.firePosition.position;
            bulletTrajectoryInstantiate.GetComponent<BulletTrajectory>().gunScript = gunScript;
        }
        else
        {
#pragma warning disable CS0162
            GameObject bulletColliderInstantiate;
            if (playerStatus.isAim)
            {
                bulletColliderInstantiate = 
                    Instantiate(bulletPrefab, gunScript.getSightScript().sightCenter.position, gunScript.getSightScript().sightCenter.rotation);
            }
            else
            {
                bulletColliderInstantiate = 
                     Instantiate(bulletPrefab, gunHolder.position, gunHolder.rotation);
            }
            bulletColliderInstantiate.GetComponent<BulletCollider>().gunScript = gunScript;
            bulletColliderInstantiate.GetComponent<BulletCollider>().fire();
        }
    }

    //반동 재생
    private void makeRecoil(string emptyShell)
    {
        if (gunScript.isHalfAuto)
        {
            gunScript.sliderReloadAnimationHolder.transform.position = gunScript.sliderReloadAnimationEndPosition.transform.position;
            GameObject emptyShellPrefab = Resources.Load(Path.emptyShellPrefaPath + "EmptyShell_" + emptyShell) as GameObject;
            Instantiate(emptyShellPrefab, gunScript.emptyShellPosition.transform.position, gunScript.emptyShellPosition.transform.rotation);
            gunScript.isEmptyShellInChamber = false;
        }

        float recoilDecreaseByMuzzleX = gunScript.getMuzzleScript().recoilDecrease.x * gunScript.recoilPowerVector.x;
        float recoilDecreaseByMuzzleY = gunScript.getMuzzleScript().recoilDecrease.y * gunScript.recoilPowerVector.y;
        float recoilDecreaseByMuzzelZ = gunScript.getMuzzleScript().recoilDecrease.z * gunScript.recoilPowerVector.z;

        float recoilDecreaseByHandleX = gunScript.getHandleScript().recoilDecrease.x * gunScript.recoilPowerVector.x;
        float recoilDecreaseByHandleY = gunScript.getHandleScript().recoilDecrease.y * gunScript.recoilPowerVector.y;
        float recoilDecreaseByHandleZ = gunScript.getHandleScript().recoilDecrease.z * gunScript.recoilPowerVector.z;

        float recoilDecreaseTotalX = recoilDecreaseByMuzzleX + recoilDecreaseByHandleX;
        float recoilDecreasetotalY = recoilDecreaseByMuzzleY + recoilDecreaseByHandleY;
        float recoilDecreasetotalZ = recoilDecreaseByMuzzelZ + recoilDecreaseByHandleZ;

        recoilHolderHeader.transform.Rotate(
            -(gunScript.recoilPowerVector.y - recoilDecreasetotalY), 
            Random.Range(-(gunScript.recoilPowerVector.x - recoilDecreaseTotalX), 
            gunScript.recoilPowerVector.x - recoilDecreaseTotalX), 
            0f);

        recoilGunHolder.transform.localPosition = Vector3.zero;
        recoilGunHolder.transform.Translate(0f, 0f, gunScript.recoilPowerVector.z - recoilDecreasetotalZ);
    }

    //반동 초기화
    private void restoreRecoil()
    {
        recoilGunHolder.localPosition = Vector3.Slerp(
            recoilGunHolder.localPosition,
            Vector3.zero,
            Time.deltaTime * gunScript.speed[Speed.restoreRecoilSpeed]);

        recoilHolderHeader.localRotation = Quaternion.Slerp(
            recoilHolder.localRotation,
            Quaternion.identity,
            Time.deltaTime * gunScript.speed[Speed.restoreRecoilSpeed]);

        if(Vector3.Distance(recoilGunHolder.localPosition, Vector3.zero) < ErrorRange.recoilGunHolder)
        {
            recoilGunHolder.localPosition = Vector3.zero;
        }

        if (!playerStatus.isSliderAnimation)
        {
            gunScript.sliderReloadAnimationHolder.localPosition = Vector3.Slerp(
                gunScript.sliderReloadAnimationHolder.localPosition,
                Vector3.zero,
                Time.deltaTime * gunScript.speed[Speed.sliderRestoreRecoilSpeed]);
        }
    }

    //****************************************************************************** 사운드 RPC 함수

    [PunRPC]
    private void playEmptySoundOneShot()
    {
        playerVarriables.smallSound.PlayOneShot(empty);
    }

    [PunRPC]
    private void playAimSound()
    {
        playerVarriables.smallSound.clip = aim;
        playerVarriables.smallSound.Play();
    }
}
