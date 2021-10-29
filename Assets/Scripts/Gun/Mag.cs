using UnityEngine;
using System.Collections.Generic;
using Varriables;

public class Mag : MonoBehaviour
{
    public string magName;

    public int bulletMaxReloaded;
    public List<Bullet> bulletReloaded = new List<Bullet>();

    public string[] availableBulletType;

    public int index;

    public float insertBulletSpeed;

    public Sprite magImage;

    public Transform magEntrancePosition;
    public Transform magGrabPosition;

    public void init(int _index, int bulletCount)
    {
        index = _index;
        for(int i = 0; i < bulletCount; i++)
        {
            Bullet bullet = new Bullet(availableBulletType[0]);
            bulletReloaded.Add(bullet);
        }
    }

    public bool insertBullet(Bullet bullet)
    {
        if(bulletReloaded.Count < bulletMaxReloaded)
        {
            if (isBulletAvailable(bullet.bulletType))
            {
                bulletReloaded.Add(bullet);
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public bool isBulletAvailable(string bulletType)
    {
        for(int i = 0; i < availableBulletType.Length; i++)
        {
            if (availableBulletType[i] == bulletType)
            {
                return true;
            }
        }
        return false;
    }

    public Bullet removeBullet()
    {
        Bullet bullet = bulletReloaded[bulletReloaded.Count - 1];
        bulletReloaded.RemoveAt(bulletReloaded.Count - 1);
        return bullet;
    }
}
