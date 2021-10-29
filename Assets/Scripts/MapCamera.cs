using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapCamera : MonoBehaviour
{
    private Transform target = null;
    private float mapFOV = 60;
    private float maxMapFOV = 60;
    private float minMapFOV = 20;
    private float speed = 20;
    
    void Start()
    {
        getTarget();
    }

    void Update()
    {
        mapFOV -= Input.GetAxis("Mouse ScrollWheel") * speed;
        mapFOV = Mathf.Clamp(mapFOV, minMapFOV, maxMapFOV);
        gameObject.GetComponent<Camera>().fieldOfView = mapFOV; 
        if(target != null)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
        else
        {
            getTarget();
        }
    }

    private void getTarget()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].GetComponent<PhotonView>().IsMine)
            {
                target = gameObjects[i].transform;
                return;
            }
        }
        target = null;
    }   
}
