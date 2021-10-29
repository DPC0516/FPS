using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalLight : MonoBehaviour
{
    [SerializeField]
    private Light pointLight;

    private Tactical tactical;

    private void Start()
    {
        tactical = GetComponent<Tactical>();
    }

    void Update()
    {
        if (tactical.isOn)
        {
            pointLight.enabled = true;
        }
        else
        {
            pointLight.enabled = false;
        }
    }
}
