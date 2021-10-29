using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Varriables;

public class GameStartInitializer : MonoBehaviour
{
    [SerializeField]
    private PhotonView PV;

    public void onStart()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().userName == Public.userName)
            {
                players[i].GetComponent<PlayerController>().destroy("Game Initializer");
            }
        }
        Public.isGameStarted = !Public.isGameStarted;
        Public.logManager.AddLog("Game Started");
        PV.RPC("onStart", RpcTarget.OthersBuffered, Public.isGameStarted);
    }

    [PunRPC]
    private void onStart(bool isGameStarted)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().userName == Public.userName)
            {
                players[i].GetComponent<PlayerController>().destroy("Game Initializer");
            }
        }
        Public.isGameStarted = isGameStarted;
    }
}
