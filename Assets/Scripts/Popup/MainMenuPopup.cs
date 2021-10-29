using UnityEngine;
using UnityEngine.SceneManagement;
using Varriables.Setting;
using Varriables;

public class MainMenuPopup : MonoBehaviour
{
    private void Start()
    {
        Setting.Load();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Setting.graphicSetting.setScreenSize();
    }

    public void OnClickStartGameButton()
    {
        SceneManager.LoadScene("JoinOrCreateRoom");
    }

    public void OnClickSetting()
    {
        SceneManager.LoadScene("Setting");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
