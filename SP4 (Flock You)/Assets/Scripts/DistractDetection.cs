using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractDetection : MonoBehaviour
{
    private List<GameObject> Enemies = new List<GameObject>();

    private void OnTriggerEnter(Collider enemy)
    {
        //if detect enemy, add to list
        if(!Enemies.Contains(enemy.gameObject) && enemy.CompareTag("Enemy"))
        {
            Enemies.Add(enemy.gameObject);
        }
    }

    public List<GameObject> GetEnemyList()
    {
        return Enemies;
    }

    private void OnTriggerExit(Collider enemy)
    {
        //remove from list once out of range
        Enemies.Remove(enemy.gameObject);
    }
}
