using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractDetection : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject distractRange;
    private List<GameObject> Enemies = new List<GameObject>();

    void Start()
    {
        distractRange = transform.Find("DistractRange").gameObject;
    }

    private void OnTriggerEnter(Collider enemy)
    {
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
        Enemies.Remove(enemy.gameObject);
    }
}
