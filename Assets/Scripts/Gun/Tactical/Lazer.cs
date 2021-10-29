using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    [SerializeField]
    private GameObject lazer;

    private Tactical tactical;

    private void Start()
    {
        tactical = GetComponent<Tactical>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            lazer.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(transform.position, hit.point));
        }
        else
        {
            lazer.transform.localScale = new Vector3(1f, 1f, 1000f);
        }

        if (tactical.isOn)
        {
            lazer.SetActive(true);
        }
        else
        {
            lazer.SetActive(false);
        }
    }
}
