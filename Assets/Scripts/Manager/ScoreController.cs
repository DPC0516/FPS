using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Varriables;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private PhotonView PV;

    private ScoreManager scoreManager;

    public int score;
    public int death;
    public int kill;

    void Start()
    {
        scoreManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ScoreManager>();
        if (PV.IsMine)
        {
            score = Public.score;
            death = Public.death;
            kill = Public.kill;
            PV.RPC("setScore", RpcTarget.OthersBuffered, score, death, kill);
        }
        scoreManager.addPlayer(PV.ViewID.ToString(), PV.Owner.NickName, score, death, kill);
    }

    void Update()
    {
        if (PV.IsMine)
        {
            score = Public.score;
            death = Public.death;
            kill = Public.kill;
            PV.RPC("setScore", RpcTarget.OthersBuffered, score, death, kill);
        }
        scoreManager.setPlayerScore(PV.ViewID.ToString(), score, death, kill);
    }

    [PunRPC]
    private void setScore(int _score, int _death, int _kill)
    {
        score = _score;
        death = _death;
        kill = _kill;
    }

    private void OnDestroy()
    {
        scoreManager.removePlayer(PV.ViewID.ToString());
    }
}
