using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    private float cooldown;
    private float CurCooldown;

    bool CanUse()
    {
        if (CurCooldown >= 0)
            return false;
        else
            return true;
    }

    void SetCooldown(float AbilityCooldown)
    {
        cooldown = AbilityCooldown;
    }

    public virtual void UseAbility()
    {
        CurCooldown = cooldown;
    }

    private void Update()
    {
        if (!CanUse()) CurCooldown -= Time.deltaTime;
    }
}
