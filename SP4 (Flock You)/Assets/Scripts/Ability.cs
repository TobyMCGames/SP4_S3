using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public float cooldown = 0;
    public float CurCooldown = 0;
    public string AbilityName;

    public bool CanUse()
    {
        if (CurCooldown >= 0)
        {
            //Debug.Log("Ability Recharging: " + CurCooldown);
            return false;
        }
        else
        {
            return true;
        }
    }

    public virtual void UseAbility()
    {
        CurCooldown = cooldown;
    }

    public void Update()
    {
        if (!CanUse()) CurCooldown -= Time.deltaTime;
    }
}
