using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployPopupButton : MonoBehaviour
{
    [SerializeField]
    private string displayMode;

    [SerializeField]
    private DeployPopup deployPopup;

    public void onClick()
    {
        deployPopup.onDisplayModeChange(displayMode);
    }
}
