using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LayEggAbility : Ability
{

    private GameObject NestPrefab;
    private GameObject InitNest;

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
        if (player.GetComponent<MovePlayer>().GetIsGrounded())
        {
            CurCooldown = cooldown;

            if (NestPrefab == null)
                NestPrefab = Resources.Load("NestSpawn") as GameObject;

            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
            {
                if (InitNest == null)
                {
                    //spawn nest
                    InitNest = Object.Instantiate(NestPrefab, player.position, player.rotation);
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
            else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                if (InitNest == null)
                {
                    //spawn nest
                    InitNest = PhotonNetwork.Instantiate("NestSpawn", player.position, player.rotation);
                    InitNest.GetComponent<NestSpawnScript>().SetMine(player.gameObject);
                    Debug.Log("LayEggAbility: Nest Created!");
                }
                else
                {
                    //delete last spawn
                    PhotonNetwork.Destroy(InitNest);
                    //spawn new nest
                    InitNest = PhotonNetwork.Instantiate("NestSpawn", player.position, player.rotation);
                    InitNest.GetComponent<NestSpawnScript>().SetMine(player.gameObject);
                }
            }
        }
    }
}
