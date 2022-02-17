using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //get list from DistractDetection and assign as own
        Enemies = player.GetComponentInChildren<DistractDetection>().GetEnemyList();

        //Distract enemies in the list
        foreach(GameObject enemy in Enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().SetDistracted(player.transform.position);
                Debug.Log(enemy.name + " distracted!");
            }
        }
    }
}
