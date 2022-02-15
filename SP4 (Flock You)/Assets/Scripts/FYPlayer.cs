using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYPlayer : MonoBehaviour
{
    // Singleton Instance
    public static FYPlayer Instance = null;

    // Default SpawnPoint
    private Transform DefaultSpawnPoint;

    // Health & Lives
    public int MaxHealth = 5;
    public int MaxLives = 3;
    public int curHealth;
    public int curLives;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetSpawnPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead())
            return;

        if (!hasHealth())
        {
            --curLives;
            curHealth = MaxHealth;
        }
    }

    public bool hasHealth()
    {
        return curHealth > 0;
    }

    public bool isDead()
    {
        return curLives <= 0;
    }

    public void setDefaultSpawn(Transform _spawnpoint)
    {
        DefaultSpawnPoint = _spawnpoint;
    }

    public void ResetSpawnPoint()
    {
        gameObject.transform.position = DefaultSpawnPoint.position;
    }
}
