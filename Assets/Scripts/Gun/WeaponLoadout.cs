using UnityEngine;
using Varriables;
using System.Collections.Generic;

public class WeaponLoadout
{
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    public List<GameObject> grenade = new List<GameObject>();

    public List<GameObject> mags = new List<GameObject>();

    public int currentWeaponType = WeaponType.primary;

    public List<BulletCartridge> bulletCartridges = new List<BulletCartridge>();

    public void initBulletCartridge()
    {
        bulletCartridges = new List<BulletCartridge>();
    }

    public void addBulletCartridge(string cartridge, int count)
    {
        BulletCartridge bulletCartridge = new BulletCartridge(count, cartridge);
        bulletCartridges.Add(bulletCartridge);
    }

    public void initMag()
    {
        mags = new List<GameObject>();
        for (int i = 0; i < Public.maxMag; i++)
        {
            mags.Add(null);
        }
    }

    public void initGrenade()
    {
        grenade = new List<GameObject>();
        for(int i = 0; i < Public.maxGrenades; i++)
        {
            grenade.Add(null);
        }
;    }

    public Gun getGunScript(int weaponType)
    {
        if(weaponType == WeaponType.primary)
        {
            return ComponentLoader.getGunScript(primaryWeapon);
        }
        else
        {
            return ComponentLoader.getGunScript(secondaryWeapon);
        }
    }

    public List<Mag> getMagScripts(string magName)
    {
        List<Mag> result = new List<Mag>();
        if (magName == WeaponType.weaponTypes[WeaponType.all])
        {
            for (int i = 0; i < mags.Count; i++)
            {
                result.Add(getMagScript(i));
            }
            return result;
        }
        for(int i = 0; i < mags.Count; i++)
        {
            if (getMagScript(i).magName == magName)
            {
                result.Add(getMagScript(i));
            }
        }
        return result;
    }

    public Mag getMagScript(int index)
    {
        return ComponentLoader.getMagScript(mags[index]);
    }
}