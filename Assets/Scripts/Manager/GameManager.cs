using UnityEngine;
using Varriables;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Varriables.Setting;
public class GameManager : MonoBehaviour, IPhotonViewCallback
{
    //팝업창
    [SerializeField]
    private GameObject[] Popups;
    [SerializeField]
    private bool[] PopupsStatus;
    [SerializeField]
    private Camera UI_Camera;
    [SerializeField]
    private Text TextUI_Winner;
    [SerializeField]
    private GameObject TextUI_WinnerGameObject;

    [SerializeField]
    private AudioListener audioListener;

    public void LoadMap()
    {
        GameObject map = Resources.Load(Path.mapPath + Public.gameMode.mapName + "_Map") as GameObject;
        Instantiate(map, Vector3.zero, Quaternion.identity);
    }

    void Start()
    {
        Setting.graphicSetting.setScreenSize();
        Public.isPause = false;
        Public.isGameStarted = false;
        PopupsStatus = new bool[Popups.Length];
        TextUI_WinnerGameObject.SetActive(false);
    }

    void Update()
    {
        bool status = getStatus();
        if (status)
        {
            //마우스 커서 표시
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Public.isPause = true;
        }
        else
        {
            //마우스 커서 숨기기 및 가운데 고정
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Public.isPause = false;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            setPopup(true, Popup.ExitPopup);
        }
        if (!findCamera())
        {
            UI_Camera.enabled = true;
            audioListener.enabled = true;
        }
        else
        {
            UI_Camera.enabled = false;
            audioListener.enabled = false;
        }
    }

    public void setPopup(bool isActive, int index)
    {
        Popups[index].SetActive(isActive);
        PopupsStatus[index] = isActive;
        if (index == Popup.DeployPopup && isActive)
        {
            Popups[index].GetComponentInChildren<DeployPopup>().initAll();
        }
    }

    private bool getStatus()
    {
        for(int i = 0; i< PopupsStatus.Length; i++)
        {
            if (PopupsStatus[i])
            {
                return true;
            }
        }
        return false;
    }

    private bool findCamera()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i< gameObjects.Length; i++)
        {
            if (gameObjects[i].GetComponentInChildren<PhotonView>().IsMine)
            {
                return true;
            }
        }
        return false;
    }

    public void winner(string name)
    {
        TextUI_Winner.text = "Winner is\n" + name;
        TextUI_WinnerGameObject.SetActive(true);
        StartCoroutine(disconnectDelay(5f));
    }

    public IEnumerator disconnectDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
}
