using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;
using Photon.Pun;

public class TimeManager : MonoBehaviour
{

    [SerializeField]
    private Material day;
    [SerializeField]
    private Material night;
    [SerializeField]
    private Material middle;

    [SerializeField]
    private Light sun;

    // Start is called before the first frame update
    void Start()
    {
        Public.currentGameTime = 0;
        Public.currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Public.isGameStarted)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Public.currentTime += Time.deltaTime;
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            Public.currentGameTime += Time.deltaTime;
        }
        float currentGameTimeByMaxGameTime = Public.currentGameTime % Public.gameMode.dayNightTime;
        float intensity;
        if (Public.gameMode.isDayNightSystem)
        {
            if (currentGameTimeByMaxGameTime < Public.gameMode.dayNightTime / 2)
            {
                intensity = ((Public.gameMode.dayNightTime / 2) - currentGameTimeByMaxGameTime) / (Public.gameMode.dayNightTime / 2);
            }
            else
            {
                intensity = (currentGameTimeByMaxGameTime - (Public.gameMode.dayNightTime / 2)) / (Public.gameMode.dayNightTime / 2);
            }
        }
        else
        {
            if (Public.gameMode.isDay)
            {
                intensity = Public.maxIntensity;
            }
            else
            {
                intensity = Public.minIntensity;
            }
        }

        if (intensity > 0.55)
        {
            RenderSettings.skybox = day;
            sun.intensity = Mathf.Lerp(sun.intensity, Public.maxIntensity, Time.deltaTime);
        }
        else if (intensity < 0.45)
        {
            RenderSettings.skybox = night;
            sun.intensity = Mathf.Lerp(sun.intensity, Public.minIntensity, Time.deltaTime);
        }
        else
        {
            RenderSettings.skybox = middle;
            sun.intensity = Mathf.Lerp(sun.intensity, (Public.maxIntensity - Public.minIntensity) / 2, Time.deltaTime);
        }
    }
}
