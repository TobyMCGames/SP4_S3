using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayEggAbility : Ability
{

    private GameObject NestPrefab;
    private GameObject InitNest;

    public LayEggAbility()
    {
        //init properties
        cooldown = 1f; 
        AbilityName = "Lay Egg Ability";
    }

    override public void UseAbility()
    {
        CurCooldown = cooldown;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(InitNest == null)
        {
            //spawn nest
            NestPrefab = Resources.Load("NestSpawn") as GameObject;
            InitNest = Object.Instantiate(NestPrefab, player.transform.position, player.transform.rotation);
        }
        else
        {
            //delete last spawn
            Object.Destroy(InitNest);
            InitNest = Object.Instantiate(NestPrefab, player.transform.position, player.transform.rotation);
        }

    }
}
