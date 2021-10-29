using UnityEngine;
using Varriables;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using Varriables.Setting;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

//Player의 움직임 관리
public class PlayerController : MonoBehaviour
{
    public string userName;

    //맵을 표시할 카메라
    private Camera mapCamera;
    //위아래로 움직일 Player의 카메라
    [SerializeField]
    private Transform playerCamera;
    //총기 홀더 Transform
    [SerializeField]
    private Transform gunHolderMovementHolder;
    //FreeLook 홀더
    [SerializeField]
    private Transform playerCameraFreeLookHolder;
    //물리이동을 위한 Player의 Rigidbody
    [SerializeField]
    private Rigidbody playerRigidbody;
    //걷기 소리
    [SerializeField]
    private AudioClip walk;
    //점프 소리
    [SerializeField]
    private AudioClip jump;
    //착지 소리
    [SerializeField]
    private AudioClip land;
    //심장박동 소리
    [SerializeField]
    private AudioClip heartBeatUp;
    [SerializeField]
    private AudioClip heartBeatDown;
 
    //피킹 소리
    [SerializeField]
    private AudioClip peek;
    [SerializeField]
    private Image ImageUI_HP;
    [SerializeField]
    private Image ImageUI_stamina;

    [SerializeField]
    private GameObject parachute;

    [SerializeField]
    private PostProcessVolume abnormalStatusVolume;
    [SerializeField]
    private PostProcessVolume hpStatusVolume;

    //Player의 최대 위/아래 각도
    private int maxPlayerRotationUp = -75;
    private int maxPlayerRotationDown = 35;
    private const int idleMaxPlayerRotationUp = -75;
    private const int idleMaxPlayerRotationDown = 35;
    private const int crawlMaxPlayerRotationUp = -15;
    private const int crawlMaxPlayerRotationDown = 5;

    //Player의 최대 자유시점 좌우 각도
    private const int maxPlayerFreeLookRotation = 80;
    
    //기본 이동 속도
    private const float defaultMoveSpeed = 2.5f;
    //현재 속도 배수
    private float multipler = 1f;

    //현재 카메라 각도
    private float playerRotationX = 0f;
    private float playerRotationY = 0f;
    private float playerFreeLookRotationX = 0f;

    //걷는 소리 내는 간격
    private float playWalkSoundSpeed;
    private const float playWalkSoundSpeedMax = 0.6f;
    private const float playWalkSoundSpeedMin = 0.2f;
    private const float runAddForceSpeed = 2f;

    //피킹 각도
    private const float peekAngle = 25f;
    //피킹 속도
    private const float peekSpeed = 5f;

    //피킹 홀더
    [SerializeField]
    private Transform peekHolder;

    private const float minHP = 0f;
    private const float maxHP = 100f;
    private float HP = maxHP;

    [SerializeField]
    private PlayerStatus playerStatus;

    [SerializeField]
    private Transform bagPosition;

    [SerializeField]
    private GameObject parachutePrefab;

    [SerializeField]
    private Transform leftLeg;
    [SerializeField]
    private Transform rightLeg;

    [SerializeField]
    private AudioClip onHit;

    [SerializeField]
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private PlayerVarriables playerVarriables;

    private const float smoothAmount = 2.5f;
    private const float maxAmount = 0.1f;

    private const float aimMaxAmount = 0.01f;

    private float drag = 6f;
    private const float maxDrag = 2f;
    private const float angularDrag = 1f;

    private const float dropDamageMultipler = 2f;

    private const float rigidbodyAccelarationMultipler = 1300f;

    private const float parachutVerticalAngle = 20f;

    private const float dropDamageHeight = -10f;
    private float height;
    private float damage;
    private float lastDamage;

    public float stamina;
    public float staminaMultipler;
    private const float staminaDecreasePerSec = 3.5f;
    private const float staminaDecreaseByJump = 20f;
    private const float staminaDecreaseMultiplerByDamage = 1.5f;
    private const float minStamina = 0f;
    private const float maxStamina = 100f;
    private const float maxStaminaMultipler = 3f;
    private const float minStaminaMultipler = 1f;
    private const float staminaRecoverSpeed = 0.25f;

    private float abnormalStatePower;
    private const float abnormalStatePowerMax = 100f;
    private const float abnormalStateRestoreSpeed = 0.25f;
    private const float abnormalStatePowerDamagerMultipler = 1.35f;

    private const float HPStatusInDecreaseSpace = 0.2f;
    private const float HPStatusInDecreaseSpeed = 2f;
    private const float HPStatusStartValue = 65f;
    private const float HPStatusHeartBeatSoundStartValue = 50f;
    private const float HPStatusGrainStartValue = 30f;

    //다리 구부리는 각도
    private const float legAnimationAngle = 25f;
    private float legDegree = 0;

    //골반
    [SerializeField]
    private Transform Pelvis;
    //웅크리기 시 골반 각도
    private Vector3 pelvisCrouchRotation = new Vector3(20, 0, 0);
    private Vector3 pelvisIdleRotation = new Vector3(0, 0, 0);
    //카메라
    [SerializeField]
    private Transform cameraRotationHolder;

    private Vector3 cameraCrouchRotation = new Vector3(-20, 0, 0);
    private Vector3 cameraIdleRotation = new Vector3(0, 0, 0);
    private Vector3 cameraCrawlRotation = new Vector3(-90, 0, 0);

    //웅크리기 속도
    private const float crouchSpeed = 4f;

    //에임 흔들림 홀더
    [SerializeField]
    private Transform aimBreakHolder;
    //부드러운 에임 흔들림을 위한 헤더
    [SerializeField]
    private Transform aimBreakHeader;
    //에임 흔들림 복구 속도
    private const float restoreAimBreakSpeed = 1.5f;
    //에임 흔들림 가속 속도
    private const float addForceAimBreakSpeed = 5f;
    //데미지에 따른 에임 흔들림 배수
    private const float onHitAimBreakDamageMultipler = 0.5f;

    [SerializeField]
    IEnumerator idleI;
    [SerializeField]
    IEnumerator crouchI;
    [SerializeField]
    IEnumerator crawlI;

    [SerializeField]
    private Transform crawlHolder;
    private Vector3 crawlRotation = new Vector3(90f, 0f, 0f);
    private Vector3 idleRotation = new Vector3(0f, 0f, 0f);

    //탄창 UI
    [SerializeField]
    private GameObject MagUI;

    //데미지 입힌사람 리스트
    private List<string> killers = new List<string>();

    [PunRPC]
    private void deployParachute()
    {
        playerStatus.isParachute = true;
        playerRigidbody.drag = maxDrag;
        parachute.SetActive(true);
    }

    [PunRPC]
    private void undeployParachute()
    {
        playerStatus.isParachute = false;
        playerRigidbody.drag = 0f;
        parachute.SetActive(false);
        Instantiate(parachutePrefab, bagPosition.position, bagPosition.rotation);
    }

    private void Start()
    {
        if (playerVarriables.PV.IsMine)
        {
            Public.id = playerVarriables.PV.ViewID.ToString();
            mapCamera = GameObject.FindGameObjectWithTag("TopCamera").GetComponentsInChildren<Camera>()[0];
            playerStatus.isMap = false;
            playerVarriables.PV.RPC("setUserName", RpcTarget.AllBuffered, Public.userName);
            StartCoroutine(updateScore());
            playWalkSoundSpeed = playWalkSoundSpeedMax;
            StartCoroutine(playMoveAnimation());
            playerVarriables.PV.RPC("deployParachute", RpcTarget.All);
        }
    }

    [PunRPC]
    private void playMoveSoundOneShot()
    {
        playerVarriables.footStep.PlayOneShot(walk);
    }

    [PunRPC]
    private void playLandSoundOneShot()
    {
        playerVarriables.footStep.PlayOneShot(land);
    }

    [PunRPC]
    private void playJumpSoundOneShot()
    {
        playerVarriables.smallSound.PlayOneShot(jump);
    }

    [PunRPC]
    private void playHeartBeatUpSoundOneShot()
    {
        playerVarriables.heartBeatSound.PlayOneShot(heartBeatUp);
    }

    [PunRPC]
    private void playHeartBeatDownSoundOneShot()
    {
        playerVarriables.heartBeatSound.PlayOneShot(heartBeatDown);
    }

    [PunRPC]
    private void playPeekSound()
    {
        playerVarriables.smallSound.clip = peek;
        playerVarriables.smallSound.Play();
    }

    [PunRPC]
    private void killLog(string _loserName)
    {
        if (playerVarriables.PV.IsMine)
        {
            if (Public.isGameStarted)
            {
                Public.kill++;
            }
            Public.logManager.AddLog("You killed " + _loserName);
        }
    }

    [PunRPC]
    private void setUserName(string _userName)
    {
        userName = _userName;
    }

    [PunRPC]
    private void playOnHitSoundOneShot()
    {
        playerVarriables.smallSound.PlayOneShot(onHit);
    }

    [PunRPC]
    public void hit(float damage, string id)
    {
        if (playerVarriables.PV.IsMine)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject player = null;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerController>().playerVarriables.PV.ViewID.ToString() == id)
                {
                    player = players[i];
                }
            }
            if (player != null)
            {
                OnDamage(damage, player.GetComponent<PlayerController>().userName);
            }
            if (HP <= minHP)
            {
                if(player != null)
                {
                    player.GetComponent<PlayerController>().playerVarriables.PV.RPC("killLog", RpcTarget.All, userName);
                    destroy(killers[killers.Count - 1]);
                }
            }
        }
    }

    private void OnDamage(float damage, string killerName)
    {
        killers.Add(killerName);
        HP -= damage;
        stamina -= damage * staminaDecreaseMultiplerByDamage;
        stamina = Mathf.Clamp(stamina, minStamina, maxStamina);
        if (playerVarriables.PV.IsMine)
        {
            if (damage > 10)
            {
                playerVarriables.PV.RPC("playOnHitSoundOneShot", RpcTarget.All);
                float onHitAimBreak = damage * onHitAimBreakDamageMultipler;
                Vector3 force = new Vector3(
                    Random.Range(-onHitAimBreak, onHitAimBreak), 
                    Random.Range(-onHitAimBreak, onHitAimBreak), 
                    Random.Range(-onHitAimBreak, onHitAimBreak));
                addForceAimBreak(force);
                addAbnormalPower(damage * abnormalStatePowerDamagerMultipler);
            }
        }
    }

    public void destroy(string killerName)
    {
        string log = "You have killed by " + killerName;
        Public.logManager.AddLog(log);
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().setPopup(true, Popup.DeployPopup);
        Public.leftTime = Public.defaultLeftTime;
        if (Public.isGameStarted)
        {
            Public.death++;
        }
        PhotonNetwork.Destroy(gameObject);
    }

    private void Update()
    {
        if (!Public.isPause)
        {
            if (playerVarriables.PV.IsMine)
            {
                checkMap();
                playerRotate();
                playerMove();
                checkJump();
                checkCrouch();
                checkCrawl();
            }
        }
        if (playerVarriables.PV.IsMine)
        {
            moveAimBreakRotation();
            restoreAimBreak();
            updateStamina();
            updateImageUI();
            checkPeek();
            checkDamage();
            updateParachuteDrag();
            restoreAbnormalStatePower();
            updateHPStatus();

            if (!playerStatus.isGround)
            {
                capsuleCollider.enabled = false;
                capsuleCollider.enabled = true;
            }
        }
    }

    private void stopLastCoroutine()
    {
        try
        {
            StopCoroutine(crouchI);
        }
        catch
        {
        }
        try
        {
            StopCoroutine(crawlI);
        }
        catch
        {

        }
        try
        {

            StopCoroutine(idleI);
        }
        catch
        {

        }
    }

    private void checkCrouch()
    {
        if (Input.GetKeyDown(Key.Crouch) && playerStatus.isGround)
        {
            playerStatus.isCrouch = !playerStatus.isCrouch;
            if (playerStatus.isCrouch)
            {
                playerStatus.isCrawl = false;
                stopLastCoroutine();
                crouchI = crouch();
                StartCoroutine(crouchI);
            }
            else
            {
                stopLastCoroutine();
                idleI = idle();
                StartCoroutine(idleI);
            }
        }
        if (playerStatus.wasCrouch != playerStatus.isCrouch)
        {
        }
        playerStatus.wasCrouch = playerStatus.isCrouch;
    }

    private void checkCrawl()
    {
        if (Input.GetKeyDown(Key.Crawl) && playerStatus.isGround)
        {
            playerStatus.isCrawl = !playerStatus.isCrawl;
            if (playerStatus.isCrawl)
            {
                playerStatus.isCrouch = false;
                stopLastCoroutine();
                crawlI = crawl();
                StartCoroutine(crawlI);
            }
            else
            {
                stopLastCoroutine();
                idleI = idle();
                StartCoroutine(idleI);
            }
        }
        if (playerStatus.wasCrawl != playerStatus.isCrawl)
        {
        }
        playerStatus.wasCrawl = playerStatus.isCrawl;
    }

    private void checkDamage()
    {
        if (transform.position.y < -10f)
        {
            destroy("Your Mistake");
        }
        if (HP <= minHP)
        {
            destroy(killers[killers.Count - 1]);
        }
        lastDamage = damage;
        if (playerRigidbody.velocity.y < dropDamageHeight)
        {
            height = Mathf.Abs(playerRigidbody.velocity.y - dropDamageHeight);
            damage = height * dropDamageMultipler;
        }
        else
        {
            damage = 0;
        }
    }

    private void checkMap()
    {
        if (Input.GetKeyDown(Key.Map))
        {
            playerStatus.isMap = !playerStatus.isMap;
        }
        if (playerStatus.isMap)
        {
            mapCamera.enabled = true;
        }
        else
        {
            mapCamera.enabled = false;
        }
    }

    private void updateImageUI()
    {
        ImageUI_HP.fillAmount = HP / maxHP;
        ImageUI_stamina.fillAmount = stamina / maxStamina;
        MagUI.SetActive(
            (Input.GetKey(KeyCode.LeftControl)
            || playerStatus.isReloading
            || playerStatus.isPreparedInsertBulletInMag
            || Input.GetKey(Key.Relaod)
            || Input.GetKey(Key.InsertBulletInMag))
            && !Public.isPause);
    }

    private void checkJump()
    {
        //점프
        if (Input.GetKeyDown(Key.Jump) && playerStatus.isGround && !playerStatus.isCrawl && !playerStatus.isCrouch)
        {
            if (stamina > staminaDecreaseByJump)
            {
                playerVarriables.PV.RPC("playJumpSoundOneShot", RpcTarget.All);
                stamina -= staminaDecreaseByJump;
                playerRigidbody.AddForce(Vector3.up * 400f, ForceMode.Impulse);
            }
        }
    }

    private void playerRotate()
    {
        //키보드 입력 감지
        float isV = Input.GetAxisRaw("Vertical");

        //마우스 이동 감지
        float isX = Input.GetAxisRaw("Mouse X");
        float isY = Input.GetAxisRaw("Mouse Y");
        float toRotateX = 0f;
        float toRotateY = 0f;

        //감도에 의해 이동 각도 계산
        if (playerStatus.isAim)
        {
            toRotateX = isX * Setting.defaultSetting.mouseSensitivityX * Public.weaponLoadout.getGunScript(Public.weaponLoadout.currentWeaponType).getSightScript().sightFOVMultipler * Time.deltaTime;
            toRotateY = isY * Setting.defaultSetting.mouseSensitivityY * Public.weaponLoadout.getGunScript(Public.weaponLoadout.currentWeaponType).getSightScript().sightFOVMultipler * Time.deltaTime;
        }
        else
        {
            toRotateX = isX * Setting.defaultSetting.mouseSensitivityX * Time.deltaTime;
            toRotateY = isY * Setting.defaultSetting.mouseSensitivityY * Time.deltaTime;
        }

        if (Input.GetKey(Key.FreeLook))
        {
            playerFreeLookRotationX += toRotateX;
            playerFreeLookRotationX = Mathf.Clamp(playerFreeLookRotationX, -maxPlayerFreeLookRotation, maxPlayerFreeLookRotation);
            playerCameraFreeLookHolder.transform.localRotation = Quaternion.Euler(0f, playerFreeLookRotationX, 0f);
            playerStatus.isFreeLook = true;
        }
        else
        {
            playerStatus.isFreeLook = false;

            playerCameraFreeLookHolder.transform.localRotation = Quaternion.Slerp(
                playerCameraFreeLookHolder.transform.localRotation, 
                Quaternion.identity, 
                DefaultSetting.changeFreeLookRotation * Time.deltaTime);
            playerFreeLookRotationX = 0;
            //X각도 변환
            playerRotationX += toRotateX;
            transform.Rotate(0f, toRotateX, 0f);

        }

        if (playerStatus.isCrawl)
        {
            maxPlayerRotationUp = 
                (int) Mathf.Lerp(maxPlayerRotationUp, crawlMaxPlayerRotationUp, Time.deltaTime * crouchSpeed);
            maxPlayerRotationDown =
                (int)Mathf.Lerp(maxPlayerRotationDown, crawlMaxPlayerRotationDown, Time.deltaTime * crouchSpeed);
        }
        else
        {
            maxPlayerRotationUp =
                (int)Mathf.Lerp(maxPlayerRotationUp, idleMaxPlayerRotationUp, Time.deltaTime * crouchSpeed);
            maxPlayerRotationDown =
                (int)Mathf.Lerp(maxPlayerRotationDown, idleMaxPlayerRotationDown, Time.deltaTime * crouchSpeed);
        }

        //Player의 위/아래 최고 각도에 의해 Y 이동 각도 재계산
        playerRotationY -= toRotateY;
        playerRotationY = Mathf.Clamp(playerRotationY, maxPlayerRotationUp, maxPlayerRotationDown);

        //Y각도 변환
        playerCamera.localRotation = Quaternion.Euler(playerRotationY, 0f, 0f);

    }

    private void playerMove()
    {
        //키보드 입력 감지
        float isH = Input.GetAxisRaw("Horizontal");
        float isV = Input.GetAxisRaw("Vertical");

        float moblility = Public.weaponLoadout.getGunScript(Public.weaponLoadout.currentWeaponType).mobility;
        //현재 이동 속도
        float moveSpeed;

        if (Input.GetKey(Key.Run) 
            && !playerStatus.isReloading 
            && !playerStatus.isPreparedInsertBulletInMag
            && playerStatus.isMoving 
            && !playerStatus.isAim 
            && !playerStatus.isPeeking 
            && playerStatus.isGround
            && !playerStatus.isParachute
            && !playerStatus.isCrouch
            && !playerStatus.isCrawl)
        {
            multipler = Mathf.Lerp(multipler, moblility * staminaMultipler, Time.deltaTime * runAddForceSpeed);
            playWalkSoundSpeed = Mathf.Lerp(playWalkSoundSpeed, playWalkSoundSpeedMin, Time.deltaTime * runAddForceSpeed);
            playerStatus.isRunning = true;
            stamina -= staminaDecreasePerSec * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, minStamina, maxStamina);
        }
        else
        {
            playerStatus.isRunning = false;
            multipler = Mathf.Lerp(multipler, 1 * Public.weaponLoadout.getGunScript(Public.weaponLoadout.currentWeaponType).mobility, Time.deltaTime * runAddForceSpeed);
            playWalkSoundSpeed = Mathf.Lerp(playWalkSoundSpeed, playWalkSoundSpeedMax, Time.deltaTime * runAddForceSpeed);
        }

        if (playerStatus.isParachute && !(isV == 0))
        {
            playerStatus.isMoving = true;
            Vector3 dir = transform.forward * isV;

            moveSpeed = defaultMoveSpeed * multipler;

            playerRigidbody.AddForce(dir * moveSpeed * rigidbodyAccelarationMultipler * Time.deltaTime, ForceMode.Acceleration);
        }
        //키보드가 움직였을시
        else if (!(isH == 0 && isV == 0))
        {
            playerStatus.isMoving = true;

            Vector3 dir;
            if (playerStatus.isWall && (isV == 1))
            {
                dir = Quaternion.Euler(0f, transform.localRotation.y, 0f) * new Vector3(isH, 0, 0);
            }
            else
            {
                dir = Quaternion.Euler(0f, transform.localRotation.y, 0f) * new Vector3(isH, 0, isV);
            }

            moveSpeed = defaultMoveSpeed * multipler;

            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
        else
        {
            playerStatus.isMoving = false;
        }
    }

    private void updateParachuteDrag()
    {
        if (playerStatus.isParachute)
        {
            //키보드 입력 감지
            float isV = Input.GetAxisRaw("Vertical");

            if (!(isV == 0) && !Public.isPause)
            {
                drag = Mathf.Lerp(drag, angularDrag, Time.deltaTime);
                if (Mathf.Abs(angularDrag - drag) < 0.1f)
                {
                    drag = angularDrag;
                }
            }
            else
            {
                drag = Mathf.Lerp(drag, maxDrag, Time.deltaTime);
                if (Mathf.Abs(maxDrag - drag) < 0.1f)
                {
                    drag = maxDrag;
                }
            }
            playerRigidbody.drag = drag;
            if (!(isV == 0))
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(parachutVerticalAngle * isV, transform.localRotation.eulerAngles.y, 0f), Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, playerRotationX, 0f), Time.deltaTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, playerRotationX, 0f), Time.deltaTime * 10f);
            if (Mathf.Abs(transform.rotation.x) < 0.01f)
            {
                transform.rotation = Quaternion.Euler(0f, playerRotationX, 0f);
            }
        }
    }

    private void updateStamina()
    {
        if (!playerStatus.isRunning || playerStatus.isParachute)
        {
            stamina = Mathf.Lerp(stamina, maxStamina, Time.deltaTime * staminaRecoverSpeed);
        }
        staminaMultipler = minStaminaMultipler + (maxStaminaMultipler - minStaminaMultipler) * (stamina / maxStamina);
    }
    
    private void checkPeek()
    {
        if(
            Input.GetKey(Key.PeekLeft) 
            && Input.GetKey(Key.PeekRight)
            && !Public.isPause )
        {
            playerStatus.isPeeking = false;
            peekHolder.transform.localRotation = Quaternion.Slerp(peekHolder.transform.localRotation, Quaternion.identity, peekSpeed * Time.deltaTime);
        }
        else if (
            (Input.GetKey(Key.PeekLeft) || Input.GetKey(Key.PeekRight)) 
            && !playerStatus.isJumping 
            && !playerStatus.isFreeLook 
            && !Public.isPause 
            && !playerStatus.isCrawl)
        {
            if (Input.GetKey(Key.PeekRight))
            {
                playerStatus.isPeeking = true;
                peekHolder.transform.localRotation = Quaternion.Slerp(peekHolder.transform.localRotation, Quaternion.Euler(0f, 0f, -peekAngle), peekSpeed * Time.deltaTime);
            }
            if (Input.GetKey(Key.PeekLeft))
            {
                playerStatus.isPeeking = true;
                peekHolder.transform.localRotation = Quaternion.Slerp(peekHolder.transform.localRotation, Quaternion.Euler(0f, 0f, peekAngle), peekSpeed * Time.deltaTime);
            }
        }
        else
        {
            playerStatus.isPeeking = false;
            peekHolder.transform.localRotation = Quaternion.Slerp(peekHolder.transform.localRotation, Quaternion.identity, peekSpeed * Time.deltaTime);
        }
        if (playerStatus.wasPeeking != playerStatus.isPeeking)
        {
            playerVarriables.PV.RPC("playPeekSound", RpcTarget.All);
        }
        playerStatus.wasPeeking = playerStatus.isPeeking;
    }

    private void addForceAimBreak(Vector3 power)
    {
        aimBreakHeader.Rotate(power);
    }

    private void moveAimBreakRotation()
    {
        aimBreakHolder.localRotation =
            Quaternion.Slerp(
                aimBreakHolder.localRotation,
                aimBreakHeader.localRotation,
                Time.deltaTime * addForceAimBreakSpeed);
    }

    private void restoreAimBreak()
    {
        aimBreakHeader.localRotation =
            Quaternion.Slerp(
                aimBreakHeader.localRotation,
                Quaternion.identity,
                Time.deltaTime * restoreAimBreakSpeed * staminaMultipler);
        aimBreakHolder.localRotation = aimBreakHeader.localRotation;
    }

    private void addAbnormalPower(float power)
    {
        abnormalStatePower += power;
        abnormalStatePower = Mathf.Clamp(abnormalStatePower, 0f, abnormalStatePowerMax);
    }

    private void restoreAbnormalStatePower()
    {
        abnormalStatePower = Mathf.Lerp(abnormalStatePower, 0f, Time.deltaTime * abnormalStateRestoreSpeed);
        Grain grain;
        Vignette vignette;
        if (abnormalStatusVolume.profile.TryGetSettings(out grain) && abnormalStatusVolume.profile.TryGetSettings(out vignette))
        {
            if (abnormalStatePower / abnormalStatePowerMax < 0.1f)
            {
                grain.intensity.value = 0f;
                vignette.intensity.value = 0f;
            }
            else
            {
                grain.intensity.value = abnormalStatePower / abnormalStatePowerMax;
                vignette.intensity.value = abnormalStatePower / abnormalStatePowerMax;
            }
        }
    }

    private void updateHPStatus()
    {
        if (HP < HPStatusStartValue)
        {
            Vignette vignette;
            float intensity = 1 - (HP / HPStatusStartValue);
            if (hpStatusVolume.profile.TryGetSettings(out vignette))
            {
                if (playerStatus.isIncrease)
                {
                    vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, intensity, Time.deltaTime * HPStatusInDecreaseSpeed);
                    if (Mathf.Abs(intensity - vignette.intensity.value) < 0.05f)
                    {
                        playerStatus.isIncrease = !playerStatus.isIncrease;
                    }
                }
                else
                {

                    vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, intensity - HPStatusInDecreaseSpace, Time.deltaTime * HPStatusInDecreaseSpeed);
                    if (Mathf.Abs((intensity - HPStatusInDecreaseSpace) - vignette.intensity.value) < 0.05f)
                    {
                        playerStatus.isIncrease = !playerStatus.isIncrease;
                    }
                }
            }
            if (HP < HPStatusHeartBeatSoundStartValue)
            {
                if (playerStatus.wasIncrease != playerStatus.isIncrease)
                {
                    if (playerStatus.isIncrease)
                    {
                        playerVarriables.PV.RPC("playHeartBeatUpSoundOneShot", RpcTarget.All);
                    }
                    else
                    {
                        playerVarriables.PV.RPC("playHeartBeatDownSoundOneShot", RpcTarget.All);
                    }
                }
            }
            Grain grain;
            if (hpStatusVolume.profile.TryGetSettings(out grain))
            {
                if (HP < HPStatusGrainStartValue)
                {
                    float grainIntenisty = 1 -  (HP / (maxHP - HPStatusGrainStartValue));
                    grain.intensity.value = grainIntenisty;
                }
                else
                {
                    grain.intensity.value = 0f;
                }
            }
            playerStatus.wasIncrease = playerStatus.isIncrease;
        }
    }

    private IEnumerator updateScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Transform middle = GameObject.FindGameObjectWithTag("Middle").transform;
            if (Vector3.Distance(middle.position, transform.position) < 10)
            {
                if (Public.isGameStarted)
                {
                    Public.score++;
                }
            }
        }
    }

    private IEnumerator playMoveAnimation()
    {
        Vector3 right = new Vector3(maxAmount, 0f, 0f);
        Vector3 left = new Vector3(-maxAmount, 0f, 0f);
        Vector3 bottom = new Vector3(0f, -maxAmount, 0f);
        Vector3 aimRight = new Vector3(aimMaxAmount, 0f, 0f);
        Vector3 aimLeft = new Vector3(-aimMaxAmount, 0f, 0f);
        Vector3 aimBottom = new Vector3(0f, -aimMaxAmount, 0f);
        Vector3[] vector3s = new Vector3[] { right, bottom, left, bottom, aimRight, aimBottom, aimLeft, aimBottom };
        int status = 0;
        int isAim = 0;
        float errorRange = 0.05f;

        while (true) {
            if (playerStatus.isAim)
            {
                isAim = 4;
                errorRange = 0.005f;
            }
            else
            {
                isAim = 0;
                errorRange = 0.05f;
            }
            if (playerStatus.isMoving 
                && playerStatus.isGround 
                && !Public.isPause
                && !playerStatus.isParachute)
            {
                if (!playerStatus.isChangingGun)
                {
                    gunHolderMovementHolder.localPosition =
                   Vector3.Lerp(
                       gunHolderMovementHolder.localPosition,
                       vector3s[status + isAim],
                       smoothAmount * multipler * Time.deltaTime);

                    if (Vector3.Distance(gunHolderMovementHolder.localPosition, vector3s[status + isAim]) < errorRange)
                    {
                        status++;
                        if (vector3s.Length / 2 <= status)
                        {
                            status = 0;
                        }
                    }
                }

                if (playerStatus.isRight)
                {
                    legDegree = Mathf.Lerp(legDegree, legAnimationAngle, 5f * multipler * Time.deltaTime);
                    if (Mathf.Abs(legAnimationAngle - legDegree) < 2f)
                    {
                        playerVarriables.PV.RPC("playMoveSoundOneShot", RpcTarget.All);
                        legDegree = legAnimationAngle;
                        playerStatus.isRight = false;
                    }
                }
                else
                {
                    legDegree = Mathf.Lerp(legDegree, -legAnimationAngle, 5f * multipler * Time.deltaTime);
                    if (Mathf.Abs(-legAnimationAngle - legDegree) < 2f)
                    {
                        playerVarriables.PV.RPC("playMoveSoundOneShot", RpcTarget.All);
                        legDegree = -legAnimationAngle;
                        playerStatus.isRight = true;
                    }
                }
            }
            else
            {
                gunHolderMovementHolder.localPosition =
                    Vector3.Slerp(
                        gunHolderMovementHolder.localPosition,
                        Vector3.zero,
                        smoothAmount * multipler * Time.deltaTime);

                legDegree = Mathf.Lerp(legDegree, 0f, 5f * multipler * Time.deltaTime);
            }

            leftLeg.localRotation = Quaternion.Euler(-legDegree, 0f, 0f);
            rightLeg.localRotation = Quaternion.Euler(legDegree, 0f, 0f);

            yield return null;
        }
    }

    private IEnumerator crouch()
    {
        while (true)
        {
            playerVarriables.leftLegController.LegUp.localRotation =
                Quaternion.Slerp(
                    playerVarriables.leftLegController.LegUp.localRotation,
                    Quaternion.Euler(playerVarriables.leftLegController.legUpCrouchRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.rightLegController.LegUp.localRotation =
                Quaternion.Slerp(
                    playerVarriables.rightLegController.LegUp.localRotation,
                    Quaternion.Euler(playerVarriables.rightLegController.legUpCrouchRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.leftLegController.LegDown.localRotation =
                Quaternion.Slerp(
                    playerVarriables.leftLegController.LegDown.localRotation,
                    Quaternion.Euler(playerVarriables.leftLegController.legDownCrouchRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.rightLegController.LegDown.localRotation =
                Quaternion.Slerp(
                    playerVarriables.rightLegController.LegDown.localRotation,
                    Quaternion.Euler(playerVarriables.rightLegController.legDownCrouchRotation),
                    Time.deltaTime * crouchSpeed);

            Pelvis.localRotation =
                Quaternion.Slerp(
                    Pelvis.localRotation,
                    Quaternion.Euler(pelvisCrouchRotation),
                    Time.deltaTime * crouchSpeed);

            cameraRotationHolder.localRotation =
                Quaternion.Slerp(
                    cameraRotationHolder.localRotation,
                    Quaternion.Euler(cameraCrouchRotation),
                    Time.deltaTime * crouchSpeed);

            crawlHolder.localRotation =
                Quaternion.Slerp(
                    crawlHolder.localRotation,
                    Quaternion.Euler(idleRotation),
                    Time.deltaTime * crouchSpeed);

            if (cameraRotationHolder.localRotation.eulerAngles.x - cameraCrouchRotation.x < 0.01f)
            {
                playerVarriables.leftLegController.LegUp.localRotation =
                    Quaternion.Euler(playerVarriables.leftLegController.legUpCrouchRotation);
                playerVarriables.rightLegController.LegUp.localRotation =
                    Quaternion.Euler(playerVarriables.rightLegController.legUpCrouchRotation);
                playerVarriables.leftLegController.LegDown.localRotation =
                    Quaternion.Euler(playerVarriables.leftLegController.legDownCrouchRotation);
                playerVarriables.rightLegController.LegDown.localRotation =
                    Quaternion.Euler(playerVarriables.rightLegController.legDownCrouchRotation);
                Pelvis.localRotation = Quaternion.Euler(pelvisCrouchRotation);
                cameraRotationHolder.localRotation = Quaternion.Euler(cameraCrouchRotation);
                crawlHolder.localRotation = Quaternion.Euler(idleRotation);
                break;
            }

            yield return null;
        }

        yield break;
    }
    
    private IEnumerator idle()
    {
        while (true)
        {
            playerVarriables.leftLegController.LegUp.localRotation =
                Quaternion.Slerp(
                    playerVarriables.leftLegController.LegUp.localRotation,
                    Quaternion.Euler(playerVarriables.leftLegController.legUpIdleRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.rightLegController.LegUp.localRotation =
                Quaternion.Slerp(
                    playerVarriables.rightLegController.LegUp.localRotation,
                    Quaternion.Euler(playerVarriables.rightLegController.legUpIdleRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.leftLegController.LegDown.localRotation =
                Quaternion.Slerp(
                    playerVarriables.leftLegController.LegDown.localRotation,
                    Quaternion.Euler(playerVarriables.leftLegController.legDownIdleRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.rightLegController.LegDown.localRotation =
                Quaternion.Slerp(
                    playerVarriables.rightLegController.LegDown.localRotation,
                    Quaternion.Euler(playerVarriables.rightLegController.legDownIdleRotation),
                    Time.deltaTime * crouchSpeed);

            Pelvis.localRotation =
                Quaternion.Slerp(
                    Pelvis.localRotation,
                    Quaternion.Euler(pelvisIdleRotation),
                    Time.deltaTime * crouchSpeed);

            cameraRotationHolder.localRotation =
                Quaternion.Slerp(
                    cameraRotationHolder.localRotation,
                    Quaternion.Euler(cameraIdleRotation),
                    Time.deltaTime * crouchSpeed);

            crawlHolder.localRotation =
                Quaternion.Slerp(
                    crawlHolder.localRotation,
                    Quaternion.Euler(idleRotation),
                    Time.deltaTime * crouchSpeed);

            if (cameraRotationHolder.localRotation.eulerAngles.x - cameraIdleRotation.x < 0.01f)
            {
                playerVarriables.leftLegController.LegUp.localRotation = 
                    Quaternion.Euler(playerVarriables.leftLegController.legUpIdleRotation);
                playerVarriables.rightLegController.LegUp.localRotation =
                    Quaternion.Euler(playerVarriables.rightLegController.legUpIdleRotation);
                playerVarriables.leftLegController.LegDown.localRotation =
                    Quaternion.Euler(playerVarriables.leftLegController.legDownIdleRotation);
                playerVarriables.rightLegController.LegDown.localRotation =
                    Quaternion.Euler(playerVarriables.rightLegController.legDownIdleRotation);
                Pelvis.localRotation = Quaternion.Euler(pelvisIdleRotation);
                cameraRotationHolder.localRotation = Quaternion.Euler(cameraIdleRotation);
                crawlHolder.localRotation = Quaternion.Euler(idleRotation);
                break;
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator crawl()
    {
        while (true)
        {
            playerVarriables.leftLegController.LegUp.localRotation =
                   Quaternion.Slerp(
                       playerVarriables.leftLegController.LegUp.localRotation,
                       Quaternion.Euler(playerVarriables.leftLegController.legUpIdleRotation),
                       Time.deltaTime * crouchSpeed);

            playerVarriables.rightLegController.LegUp.localRotation =
                Quaternion.Slerp(
                    playerVarriables.rightLegController.LegUp.localRotation,
                    Quaternion.Euler(playerVarriables.rightLegController.legUpIdleRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.leftLegController.LegDown.localRotation =
                Quaternion.Slerp(
                    playerVarriables.leftLegController.LegDown.localRotation,
                    Quaternion.Euler(playerVarriables.leftLegController.legDownIdleRotation),
                    Time.deltaTime * crouchSpeed);

            playerVarriables.rightLegController.LegDown.localRotation =
                Quaternion.Slerp(
                    playerVarriables.rightLegController.LegDown.localRotation,
                    Quaternion.Euler(playerVarriables.rightLegController.legDownIdleRotation),
                    Time.deltaTime * crouchSpeed);

            Pelvis.localRotation =
                Quaternion.Slerp(
                    Pelvis.localRotation,
                    Quaternion.Euler(pelvisIdleRotation),
                    Time.deltaTime * crouchSpeed);

            cameraRotationHolder.localRotation =
                   Quaternion.Slerp(
                       cameraRotationHolder.localRotation,
                       Quaternion.Euler(cameraCrawlRotation),
                       Time.deltaTime * crouchSpeed);

            crawlHolder.localRotation =
                Quaternion.Slerp(
                    crawlHolder.localRotation,
                    Quaternion.Euler(crawlRotation),
                    Time.deltaTime * crouchSpeed);

            if (cameraRotationHolder.localRotation.eulerAngles.x - cameraCrawlRotation.x < 0.01f)
            {
                playerVarriables.leftLegController.LegUp.localRotation =
                    Quaternion.Euler(playerVarriables.leftLegController.legUpIdleRotation);
                playerVarriables.rightLegController.LegUp.localRotation =
                    Quaternion.Euler(playerVarriables.rightLegController.legUpIdleRotation);
                playerVarriables.leftLegController.LegDown.localRotation =
                    Quaternion.Euler(playerVarriables.leftLegController.legDownIdleRotation);
                playerVarriables.rightLegController.LegDown.localRotation =
                    Quaternion.Euler(playerVarriables.rightLegController.legDownIdleRotation);
                Pelvis.localRotation = Quaternion.Euler(pelvisIdleRotation);
                cameraRotationHolder.localRotation = Quaternion.Euler(cameraCrawlRotation);
                crawlHolder.localRotation = Quaternion.Euler(crawlRotation);
                break;
            }

            yield return null;
        }

        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("PlayerCollider")
            && !collision.collider.CompareTag("EmptyShell") 
            && !collision.collider.CompareTag("Bullet") )
        {
            if (playerStatus.isParachute)
            {
                playerVarriables.PV.RPC("undeployParachute", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerVarriables.PV.IsMine)
        {
            if(other.CompareTag("DieArea"))
            {
                destroy("Your Mistake");
            }
            if (!playerStatus.isGround)
            {
                if (!other.CompareTag("PlayerCollider") && !other.CompareTag("EmptyShell") && !other.CompareTag("Bullet"))
                {
                    Public.DebugLog("Trigger Enter", other.name, playerVarriables.PV);
                    OnDamage(lastDamage, "Drop Damage");
                    playerStatus.isGround = true;
                    playerStatus.isJumping = false;
                    playerVarriables.PV.RPC("playLandSoundOneShot", RpcTarget.All);
                }
                else
                {
                    playerStatus.isGround = false;
                    playerStatus.isJumping = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerVarriables.PV.IsMine)
        {
            if (!other.CompareTag("PlayerCollider") && !other.CompareTag("EmptyShell") && !other.CompareTag("Bullet"))
            {
                Public.DebugLog("Trigger Exit", other.name, playerVarriables.PV);
                playerStatus.isGround = false;
                playerStatus.isJumping = true;
            }
        }
    }
}