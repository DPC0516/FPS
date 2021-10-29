using UnityEngine;
using Varriables;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    public string gunName;
    public string gunClass;

    public GameObject defaultSightPrefab;
    public GameObject defaultTacticalPrefab;
    public GameObject defaultMuzzlePrefab;
    public GameObject defaultHandlePrefab;
    public GameObject sightPrefab;
    public GameObject sliderPrefab;
    public GameObject tacticalPrefab;
    public GameObject muzzlePrefab;
    public GameObject handlePrefab;

    public GameObject sightInstantiate;
    public GameObject sliderInstantiate;
    public GameObject magInstantiate;
    public GameObject tacticalInstantiate;
    public GameObject muzzleInstantiate;
    public GameObject handleInstantiate;

    public Vector3 gunDefaultPosition;
    public Vector3 gunAimPosition;
    public Transform firePosition;
    public Transform muzzlePosition;
    public Transform emptyShellPosition;
    public Transform triggerPosition;
    public Transform handlePosition;
    public Transform leftHandPosition;
    public Transform sightPosition;
    public Transform gunHeaderPosition;
    public Transform magPosition;
    public Transform magOutPosition;
    public Transform sliderPosition;
    public Transform sliderReloadAnimationEndPosition;
    public Transform magReloadAnimationHolder;
    public Transform sliderReloadAnimationHolder;
    public Transform safetyDevicePosition;
    public Transform tacticalPosition;

    public int bulletMaxInChamber;
    public List<Bullet> bulletInChamber = new List<Bullet>();

    public string[] availableBulletType;

    public GameObject[] availableMag;

    public GameObject[] availableSight;

    public GameObject[] availableTactical;

    public GameObject[] availableMuzzle;

    public GameObject[] availableHandle;

    public int defaultFireMode;
    public int[] fireModes;

    public int burstCount;

    public float[] speed;

    public float range;

    public Vector3 recoilPowerVector;

    public AudioClip fireSound;
    public AudioClip silencerFireSound;

    public int currentBurstCount = 0;
    public int fireMode;

    public AudioClip magOut;
    public AudioClip magIn;
    public AudioClip sliderPull;
    public AudioClip sliderLet;

    public bool isSightWithSlider;
    public bool isHalfAuto;
    public bool isEmptyShellInChamber = false;

    public float damage;
    public float headShotMultipler;

    public bool isJam;

    public bool isEmpty;

    public float mobility;

    public float isWallLength;

    public Bullet lastFired;

    public float bulletSpeed;

    public void initMuzzle()
    {
        muzzleInstantiate = Instantiate(muzzlePrefab, muzzlePosition);
        firePosition = getMuzzleScript().firePosition;
    }

    public void initGun()
    {
        sliderInstantiate = Instantiate(sliderPrefab, sliderReloadAnimationHolder);
    }

    public void initSight()
    {
        sightInstantiate = Instantiate(sightPrefab, sightPosition);
        if (isSightWithSlider)
        {
            sightPosition.parent = sliderReloadAnimationHolder;
        }
    }

    public void initTactical(int weaponType)
    {
        tacticalInstantiate = Instantiate(tacticalPrefab, tacticalPosition);
        tacticalInstantiate.GetComponent<Tactical>().weaponType = weaponType;
    }

    public void initHandle()
    {
        handleInstantiate = Instantiate(handlePrefab, handlePosition);
        leftHandPosition = handleInstantiate.GetComponent<Handle>().handlePosition;
    }

    public void init()
    {
        sightPrefab = defaultSightPrefab;

        fireMode = defaultFireMode;
    }

    public void changeFireMode()
    {
        int fireModeIndex = 0;
        for (int i = 0; i < fireModes.Length; i++)
        {
            if (fireMode == fireModes[i])
            {
                fireModeIndex = i;
            }
        }
        if (fireModeIndex == fireModes.Length - 1)
        {
            fireMode = fireModes[0];
        }
        else
        {
            fireMode = fireModes[fireModeIndex + 1];
        }
    }

    public Sight getSightScript()
    {
        return ComponentLoader.getSightScript(sightInstantiate);
    }

    public Mag getMagScript()
    {
        return ComponentLoader.getMagScript(magInstantiate);
    }

    public Tactical getTacticalScript()
    {
        return ComponentLoader.getTacticalScript(tacticalInstantiate);
    }

    public Handle getHandleScript()
    {
        return ComponentLoader.getHandleScript(handleInstantiate);
    }

    public bool insertBulletInChamber(Bullet bullet)
    {
        bulletInChamber.Add(bullet);
        return isBulletAvailable(bullet.bulletType);
    }

    public Bullet removeBulletInChamber()
    {
        Bullet bullet = bulletInChamber[bulletInChamber.Count - 1];
        bulletInChamber.RemoveAt(bulletInChamber.Count - 1);
        return bullet;
    }

    public bool isBulletAvailable(string bulletType)
    {
        for (int i = 0; i < availableBulletType.Length; i++)
        {
            if (availableBulletType[i] == bulletType)
            {
                return true;
            }
        }
        return false;
    }

    public bool isMagAvailable(string magName)
    {
        for(int i = 0; i < availableMag.Length; i++)
        {
            if(ComponentLoader.getMagScript(availableMag[i]).magName == magName)
            {
                return true;
            }
        }
        return false;
    }

    public bool isSightAvailable(string sightName)
    {
        for (int i = 0; i < availableSight.Length; i++)
        {
            if (ComponentLoader.getSightScript(availableSight[i]).sightName == sightName)
            {
                return true;
            }
        }
        return false;
    }

    public Muzzle getMuzzleScript()
    {
        return muzzleInstantiate.GetComponent<Muzzle>();
    }
}
