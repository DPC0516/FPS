using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;

public class Bullet {

    public string bulletType;
    public int penetrationPower;
    public int mass;
     
    public Bullet(string bulletType)
    {
        this.bulletType = bulletType;
    }
}
