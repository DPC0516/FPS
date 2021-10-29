using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Varriables;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject spawnHolder;
    [SerializeField]
    private GameObject spawnPosition;

    [SerializeField]
    private bool isSpawnMiddle;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = Public.userName;
        Public.DebugLog("NetworkManager", Public.isJoin.ToString(), null);
        if (Public.isJoin)
        {
            Public.DebugLog("NetworkManager", "Join", null);
            PhotonNetwork.JoinRoom(Public.roomName);
        }
        else
        {
            Public.DebugLog("NetworkManager", "Create", null);
            PhotonNetwork.CreateRoom(Public.roomName, new RoomOptions { MaxPlayers = (byte)Public.gameMode.maxPlayer }, null);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Public.DebugLog("NetworkManager", "Join Fail", null);
        Public.DebugLog("NetworkManager", message, null);
        Public.logManager.AddLog(message);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Public.DebugLog("NetworkManager", "Create Fail", null);
        Public.DebugLog("NetworkManager", message, null);
        Public.logManager.AddLog(message);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnJoinedRoom()
    {
        Public.DebugLog("NetworkManager", "OnJoined", null);
        if (PhotonNetwork.IsMasterClient)
        {
            Public.currentGameTime = Random.Range(0f, Public.gameMode.dayNightTime);
            PhotonNetwork.Instantiate("Prefabs/Initializer", Vector3.zero, Quaternion.identity);
        }
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().setPopup(true, Popup.DeployPopup);
        PhotonNetwork.Instantiate("Prefabs/ScoreController", Vector3.zero, Quaternion.identity);
    }

    public void spawn(float delayTime)
    {
        spawnHolder.transform.Rotate(0f, Random.Range(0, 360), 0f);
        StartCoroutine(spawnDelay(delayTime));
    }

    private IEnumerator spawnDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (isSpawnMiddle)
        {
            PhotonNetwork.Instantiate("Prefabs/Player", new Vector3(0, -5, 0), Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Prefabs/Player", spawnPosition.transform.position, Quaternion.identity);
        }
    }
}
