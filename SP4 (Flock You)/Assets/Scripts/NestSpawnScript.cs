using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestSpawnScript : Peckable
{
    //private float health = 0f;
    private GameObject playerWhoSpawnMe = null;

    public void SetMine(GameObject thisplayer)
    {
        //set player who spawned the nest
        playerWhoSpawnMe = thisplayer;
    }

    public GameObject GetWhoSpawnMe()
    {
        //very long function name returning a very long variable name lol
        return playerWhoSpawnMe;
    }

    protected override void Update()
    {
        float curHealth = gameObject.GetComponent<EntityHealth>().GetHealth();

        if (curHealth <= 0)
        {
            //destroy nest if zero health
            Destroy(gameObject);
        }

        if (canBeDamaged)
        {
            //take damage
            gameObject.GetComponent<EntityHealth>().loseHealth(20f);
            Debug.Log("NestSpawn Health: " + curHealth);
            canBeDamaged = false;
        }
    }
}
