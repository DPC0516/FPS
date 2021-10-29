using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Varriables.Setting;
using UnityEngine.SceneManagement;

public class SettingPopup : MonoBehaviour
{
    [SerializeField]
    private InputField width;
    [SerializeField]
    private InputField height;
    [SerializeField]
    private Toggle isFullScreen;

    [SerializeField]
    private Slider sensitivityXS;
    [SerializeField]
    private InputField sensitivityXI;

    [SerializeField]
    private Slider sensitivityYS;
    [SerializeField]
    private InputField sensitivityYI;

    void Start()
    {
        width.text = Setting.graphicSetting.width.ToString();
        height.text = Setting.graphicSetting.height.ToString();
        isFullScreen.isOn = Setting.graphicSetting.isFullScreen;
        sensitivityXI.text = (Setting.defaultSetting.mouseSensitivityX/10f).ToString();
        sensitivityXS.value = Setting.defaultSetting.mouseSensitivityX / Setting.defaultSetting.mouseSensitivityMax;
        sensitivityYI.text = (Setting.defaultSetting.mouseSensitivityY/10f).ToString();
        sensitivityYS.value = Setting.defaultSetting.mouseSensitivityY / Setting.defaultSetting.mouseSensitivityMax;
    }

    public void OnChangeValue(bool isX, bool isSlider, float value)
    {
        if (isX)
        {
            if (isSlider)
            {
                sensitivityXI.text = (Setting.defaultSetting.mouseSensitivityMax * value / 10f).ToString();
            }
            else
            {
                sensitivityXS.value = value * 10 / Setting.defaultSetting.mouseSensitivityMax;
            }
        }
        else
        {
            if (isSlider)
            {
                sensitivityYI.text = (Setting.defaultSetting.mouseSensitivityMax * value / 10f).ToString();
            }
            else
            {
                sensitivityYS.value = value * 10 / Setting.defaultSetting.mouseSensitivityMax;
            }
        }
    }

    public void OnClickSave()
    {
        Setting.graphicSetting.width = int.Parse(width.text);
        Setting.graphicSetting.height = int.Parse(height.text);
        Setting.graphicSetting.isFullScreen = isFullScreen.isOn;
        Setting.defaultSetting.mouseSensitivityX = float.Parse(sensitivityXI.text) * 10;
        Setting.defaultSetting.mouseSensitivityY = float.Parse(sensitivityYI.text) * 10;

        Setting.graphicSetting.setScreenSize();

        Setting.Save();
    }

    public void OnExit()
    {
        SceneManager.LoadScene("MainMenu");   
    }
}
