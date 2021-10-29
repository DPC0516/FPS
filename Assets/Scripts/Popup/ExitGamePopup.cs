using UnityEngine.SceneManagement;
using UnityEngine;
using Varriables;
using Photon.Pun;
using Varriables.Setting;

public class ExitGamePopup : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;

    private void Start()
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void OnExitButtonClick()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnCancleButtonClick()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().setPopup(false, Popup.ExitPopup);
    }

    public void OnSuicideButtonClick()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().userName == Public.userName)
            {
                players[i].GetComponent<PlayerController>().destroy("Suicide");
                GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().setPopup(false, Popup.ExitPopup);
            }
        }
    }

    public void OnStartClick()
    {
        startButton.SetActive(false);
        GameObject.FindGameObjectWithTag("Initializer").GetComponent<GameStartInitializer>().onStart();
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().setPopup(false, Popup.ExitPopup);
    }
}
