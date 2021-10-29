using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Varriables;

public class MagPosition : MonoBehaviour
{
    public GameObject mag;

    public GameObject magPositionUI;

    public Mag getMagScript()
    {
        return ComponentLoader.getMagScript(mag);
    }

    private void Update()
    {
        magPositionUI.GetComponentsInChildren<Image>()[1].sprite = getMagScript().magImage;
        magPositionUI.GetComponentInChildren<Text>().text = getMagScript().bulletReloaded.Count + "/" + getMagScript().bulletMaxReloaded;
    }
}
