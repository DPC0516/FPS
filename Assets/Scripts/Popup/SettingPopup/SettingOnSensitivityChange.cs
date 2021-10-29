using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Varriables.Setting;

public class SettingOnSensitivityChange : MonoBehaviour
{
    [SerializeField]
    private SettingPopup settingPopup;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private bool isX;
    [SerializeField]
    private bool isSlider;

    public void OnValueChange()
    {
        if (isSlider)
        {
            settingPopup.OnChangeValue(isX, isSlider, slider.value);
        }
        else
        {
            try
            {
                if (float.Parse(inputField.text) > Setting.defaultSetting.mouseSensitivityMax / 10)
                {
                    inputField.text = (Setting.defaultSetting.mouseSensitivityMax / 10).ToString();
                    settingPopup.OnChangeValue(isX, isSlider, Setting.defaultSetting.mouseSensitivityMax / 10);
                }
                else if (float.Parse(inputField.text) < 0)
                {
                    inputField.text = "0";
                    settingPopup.OnChangeValue(isX, isSlider, 0);
                }
                else
                {
                    settingPopup.OnChangeValue(isX, isSlider, float.Parse(inputField.text));
                }
            }
            catch
            {
                return;
            }
        }
    }
}
