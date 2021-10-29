using UnityEngine;
using Photon.Pun;

public class PlayerVarriables : MonoBehaviour
{
    //왼쪽 팔
    public ArmController leftArmController;
    //오른쪽 팔
    public ArmController rightArmController;

    //왼쪽 다리
    public LegController leftLegController;
    //오른쪽 다리
    public LegController rightLegController;

    //팔 위치 변경 헤더
    public Transform leftArmHeader;
    public Transform rightArmHeader;

    //총기 흔들림 변수
    public const float swayAmount = 0.01f;
    public const float smoothAmount = 6f;
    public const float maxAmount = 0.04f;
    public const float aimSwayAmount = 0.005f;
    public const float aimSmoothAmount = 6f;
    public const float aimMaxAmount = 0.0001f;

    //총기 변경 속도
    public const float changeGunSpeed = 10f;

    //손 타겟 바꾸는 속도
    public const float armChangeTargetSpeed = 10f;

    public AudioSource smallSound;
    public AudioSource gunSound;
    public AudioSource gunSilencerSound;
    public AudioSource footStep;
    public AudioSource heartBeatSound;

    public PhotonView PV;

    public Transform[] magPositions;

    public Transform[] grenadePositions;

    public GunController gunController;
    public PlayerController playerController;
    public Bag bag;

    public WeaponLoadout weaponLoadout = new WeaponLoadout();
}
