using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public float cooldown = 0;
    public float CurCooldown = 0;

    public bool CanUse()
    {
        if (CurCooldown >= 0)
        {
            Debug.Log("Ability recharging...");
            return false;
        }
        else
        {
            Debug.Log("Ability Ready!");
            return true;
        }
    }

    public void SetCooldown(float AbilityCooldown)
    {
        cooldown = AbilityCooldown;
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
