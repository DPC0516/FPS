using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeployPopupScrollVIewItem : MonoBehaviour
{
    public void onClick()
    {
        string text = GetComponentInChildren<Text>().text;
        GameObject.FindGameObjectWithTag("DeployPopup").GetComponentInChildren<DeployPopup>().onItemEnter(text);
    }
}
