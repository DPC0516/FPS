using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworkInit : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GunChanger gunChanger;
    [SerializeField]
    private AudioListener audioListener;
    [SerializeField]
    private Camera cam1;
    [SerializeField]
    private Camera cam2;
    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private GameObject mapPing;
    [SerializeField]
    private Rigidbody playerRigidbody;
    [SerializeField]
    private GameObject abnormalStatus;
    [SerializeField]
    private GameObject HPStatus;


    [SerializeField]
    private PhotonView PV;

    void Awake()
    {
        if (!PV.IsMine)
        {
            gunChanger.enabled = false;
            audioListener.enabled = false;
            cam1.enabled = false;
            cam2.enabled = false;
            UI.SetActive(false);
            mapPing.SetActive(false);
            playerRigidbody.isKinematic = true;
            abnormalStatus.SetActive(false);
            HPStatus.SetActive(false);
        }
    }
}
