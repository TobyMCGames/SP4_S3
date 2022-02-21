using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayEggAbility : Ability
{

    private GameObject NestPrefab;
    private GameObject InitNest;
    private Transform player;

    public LayEggAbility()
    {
        //init properties
        cooldown = 10f; 
        AbilityName = "Lay Egg Ability";
    }

    public GameObject GetNest()
    {
        return InitNest;
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
            InitNest.GetComponent<NestSpawnScript>().SetMine(player.gameObject);
            Debug.Log("LayEggAbility: Nest Created!");
        }
        else
        {
            //delete last spawn
            Object.Destroy(InitNest);
            //spawn new nest
            InitNest = Object.Instantiate(NestPrefab, player.transform.position, player.transform.rotation);
            InitNest.GetComponent<NestSpawnScript>().SetMine(player.gameObject);
        }

    }
}
