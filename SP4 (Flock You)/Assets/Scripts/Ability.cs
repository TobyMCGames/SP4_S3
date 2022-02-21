using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    protected Transform player;

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
            return true;
    }

    public void SetPlayer(Transform _player)
    {
        player = _player;
    }

    public virtual void UseAbility()
    {
        //*** Will be overrided *** 
        //CurCooldown = cooldown;
    }

    public void Update()
    {
        //countdown for cooldown time
        if (!CanUse()) CurCooldown -= Time.deltaTime;
    }
}
