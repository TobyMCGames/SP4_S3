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
        Enemies = player.GetComponentInChildren<DistractDetection>().GetEnemyList();

        foreach(GameObject enemy in Enemies)
        {
            if (enemy != null)
                Debug.Log(enemy.name);
            else
                Debug.Log("Distract Ability: No enemies detected!");
        }
    }
}
