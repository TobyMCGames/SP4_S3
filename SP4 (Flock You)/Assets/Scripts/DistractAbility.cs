using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DistractAbility : Ability
{
    private List<GameObject> Enemies = new List<GameObject>();

    public DistractAbility()
    {
        //init properties
        cooldown = 5f;
        AbilityName = "Distract Ability";
    }

    override public void UseAbility()
    {
        CurCooldown = cooldown;
        //get list from DistractDetection and assign as own
        Enemies = player.GetComponentInChildren<DistractDetection>().GetEnemyList();

        //Distract enemies in the list
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
        {
            foreach (GameObject enemy in Enemies)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<Enemy>().Distracted(player.transform.position);
                    Debug.Log(enemy.name + " distracted!");
                }
            }
        }
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            List<int> IDList = new List<int>();
            for (int i = 0; i < Enemies.Count; i++)
                IDList.Add(Enemies[i].GetPhotonView().ViewID);

            photonView.RPC("DistractEnemyList", RpcTarget.AllViaServer, IDList.ToArray(), player.transform.position);
        }
    }
}
