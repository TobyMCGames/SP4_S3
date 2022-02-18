using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestSpawnScript : MonoBehaviour
{
    //private float health = 0f;
    private GameObject playerWhoSpawnMe = null;
    private bool canBeDamaged = false;

    private void Start()
    {
        //Init variables
        //health = gameObject.GetComponent<EntityHealth>().GetHealth();
    }

    public void SetMine(GameObject thisplayer)
    {
        //set player who spawned the nest
        playerWhoSpawnMe = thisplayer;
    }

    private void OnTriggerEnter(Collider playerInteractDetect)
    {
        //check if its the player interact capsule thing
        if (playerInteractDetect.transform.CompareTag("InteractDetect"))
        {
            //check if the nest is not the one created by player, else can damage that nest
            if (playerInteractDetect.transform.parent.gameObject != playerWhoSpawnMe)
            {
                canBeDamaged = true;
            }
        }
    }

    private void OnTriggerExit(Collider playerInteractDetect)
    {
        //reset canBeDamaged once player out of range
        if (playerInteractDetect.transform.CompareTag("InteractDetect"))
        {
            canBeDamaged = false;
        }
    }

    private void Update()
    {
        float curHealth = gameObject.GetComponent<EntityHealth>().GetHealth();

        if (curHealth <= 0)
        {
            //destroy nest if zero health
            Destroy(gameObject);
        }

        if (Input.GetMouseButtonDown(0) && canBeDamaged)
        {
            //take damage
            gameObject.GetComponent<EntityHealth>().loseHealth(20f);
            Debug.Log("NestSpawn Health: " + curHealth);
        }
    }
}
