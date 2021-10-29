using UnityEngine;
using Varriables;
using Photon.Pun;

public class GameModeInitializer : MonoBehaviour
{
    [SerializeField]
    private PhotonView PV;

    public void Start()
    {
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("initGameMode", RpcTarget.OthersBuffered,
                    Public.gameMode.maxPlayer,
                    Public.gameMode.gameModeName,
                    Public.gameMode.isScore,
                    Public.gameMode.isKill,
                    Public.gameMode.isTime,
                    Public.gameMode.targetScore,
                    Public.gameMode.targetKill,
                    Public.gameMode.targetTime,
                    Public.gameMode.mapName,
                    Public.gameMode.isDayNightSystem,
                    Public.gameMode.isDay,
                    Public.gameMode.dayNightTime);
                GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().LoadMap();
            }
        }
    }

    [PunRPC]
    private void initGameMode(
        int maxPlayer, 
        string gameModeName, 
        bool isScore, 
        bool isKill, 
        bool isTime, 
        int targetScore, 
        int targetKill, 
        int targetTime, 
        string mapName,
        bool isDayNightSystem,
        bool isDay,
        int dayNightTime)
    {
        Public.gameMode = new GameMode(maxPlayer, gameModeName, isScore, isKill, isTime, targetScore, targetKill, targetTime, mapName, isDayNightSystem, isDay, dayNightTime);
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().LoadMap();
    }

}
