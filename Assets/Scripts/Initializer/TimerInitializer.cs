using UnityEngine;
using Photon.Pun;
using Varriables;

public class TimerInitializer : MonoBehaviour
{
    [SerializeField]
    private PhotonView PV;

    public void Update()
    {
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("initTime", RpcTarget.OthersBuffered,
                    Public.currentTime, Public.currentGameTime);
            }
        }
    }

    [PunRPC]
    private void initTime(float currentTime, float currentGameTime)
    {
        Public.currentTime = currentTime;
        Public.currentGameTime = currentGameTime;
    }
}
