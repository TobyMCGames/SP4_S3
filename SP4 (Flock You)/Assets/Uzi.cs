using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uzi : Weapon
{
    protected override void Start()
    {
        //init variables (can be overriden)
        numShots = 0;
        firerate = 0.2f;
        MaxAmmo = 100;
        MaxMag = 30;
        maxNumShot = 7;
        waitToShoot = 1f;
        FirerateTimer = 0f;
        waitToShootTimer = 0f;
    }
}
