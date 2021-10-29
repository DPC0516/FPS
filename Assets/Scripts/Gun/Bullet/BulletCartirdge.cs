using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;

public class BulletCartridge
{
    public int bulletCount;
    public string bulletType;

    public Sprite bulletCartridgeSprite;

    public BulletCartridge(int bulletCount, string bulletType)
    {
        bulletCartridgeSprite = Resources.Load<Sprite>(Path.bulletCartridgeSpritePath + "BulletCartridge_" +  bulletType);

        this.bulletCount = bulletCount;
        this.bulletType = bulletType;
    }

    public Bullet removeBullet()
    {
        Bullet bullet = new Bullet(bulletType);
        bulletCount--;
        return bullet;
    }

    public void insertBullet()
    {
        bulletCount++;
    }
}
