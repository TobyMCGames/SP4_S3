using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NestSpawnScript : Peckable
{
    private PhotonView photonView;
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
        if (photonView == null)
            photonView = gameObject.GetPhotonView();

        
        float curHealth = gameObject.GetComponent<EntityHealth>().GetHealth();

        if (curHealth <= 0)
        {
            //destroy nest if zero health
            if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.SINGLE_PLAYER)
                Destroy(gameObject);
            else if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
                photonView.RPC("NestDestroyed", RpcTarget.AllViaServer);
                
        }

        if (canBeDamaged)
        {
            //take damage
            gameObject.GetComponent<EntityHealth>().loseHealth(20f);
            Debug.Log("NestSpawn Health: " + curHealth);
            canBeDamaged = false;
        }
    }

    [PunRPC]
    private void NestDestroyed()
    {
        Destroy(gameObject);
    }
}
