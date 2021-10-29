using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HitEvent : MonoBehaviour
{
    [SerializeField]
    private PlayerVarriables playerVarriables;

    public void hit(float _damage, string _id)
    {
        playerVarriables.PV.RPC("hit", RpcTarget.AllBuffered, _damage, _id);
    }
}
