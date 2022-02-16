using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayEggAbility : Ability
{

    private GameObject NestPrefab;
    private GameObject InitNest;

    public LayEggAbility()
    {
        cooldown = 1f; 
        AbilityName = "Lay Egg Ability";
    }

    override public void UseAbility()
    {
        CurCooldown = cooldown;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(InitNest == null)
        {
            NestPrefab = Resources.Load("NestSpawn") as GameObject;
            Object.Instantiate(NestPrefab, player.transform.position, player.transform.rotation);
            InitNest = Object.Instantiate(NestPrefab, player.transform.position, player.transform.rotation);
        }
        else
        {
            Object.Destroy(InitNest);
        }

    }
}
