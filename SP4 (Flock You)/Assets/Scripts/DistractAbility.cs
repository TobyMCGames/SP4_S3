using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistactAbility : Ability
{
    public DistactAbility()
    {
        //init properties
        cooldown = 5f;
        AbilityName = "Distract Ability";
    }

    override public void UseAbility()
    {
        CurCooldown = cooldown;
    }
}
