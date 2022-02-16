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

    // Player scripts
    public DropLoadControl DropLoadControl_script;

    //LayEggAbility layEggAbility = new LayEggAbility();
    LayEggAbility layEggAbility;

    private void Awake()
    {
        Instance = this;
        curHealth = MaxHealth;
        curLives = MaxLives;
        layEggAbility = GetComponent<AbilityControl>().GetLayEggAbility();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            ResetSpawnPoint();
            UI_Manager.Instance.UpdateHealth();
            UI_Manager.Instance.UpdateLives();
        }

        // Testing
        if (Input.GetButtonDown("Jump"))
        {
            Damaged(1);
        }
    }

    public bool hasHealth()
    {
        return curHealth > 0;
    }

    public bool isDead()
    {
        return curLives <= 0 && !hasHealth();
    }

    public void setDefaultSpawn(Transform _spawnpoint)
    {
        DefaultSpawnPoint = _spawnpoint;
    }

    public void ResetSpawnPoint()
    {
        GameObject NestPrefab = Resources.Load("NestSpawn") as GameObject;
        if (layEggAbility.GetNest() != null)
        {
            gameObject.transform.position = layEggAbility.GetNest().transform.position;
            Destroy(layEggAbility.GetNest());
        }
        else 
            gameObject.transform.position = DefaultSpawnPoint.position;
    }

    public void Damaged(int _damage)
    {
        curHealth -= _damage;
        UI_Manager.Instance.UpdateHealth();
    }
}
