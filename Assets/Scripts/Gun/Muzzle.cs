using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    public Transform firePosition;
    public ParticleSystem muzzleEffect;
    public Light pointLight;
    public string muzzleName;
    public Vector3 recoilDecrease;
    public bool isSilencer;

    IEnumerator playLightI;

    public void Start()
    {
        pointLight.enabled = false;
    }

    public void play()
    {
        try
        {
            StopCoroutine(playLightI);
        }
        catch
        {

        }

        muzzleEffect.Play();
        playLightI = playLight();
        StartCoroutine(playLightI);
    }

    private IEnumerator playLight()
    {
        pointLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        pointLight.enabled = false;
    }
}
