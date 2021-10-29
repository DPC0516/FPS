using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingOnExitButtonClick : MonoBehaviour
{
    [SerializeField]
    private bool isExit;

    [SerializeField]
    private SettingPopup settingPopup;

    public void OnClick()
    {
        if (isExit)
        {
            settingPopup.OnExit();
        }
        else
        {
            settingPopup.OnClickSave();
        }
    }
}
