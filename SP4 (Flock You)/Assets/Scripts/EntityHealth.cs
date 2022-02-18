using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    private float health;
    private float MaxHealth = 100f;

    private void Start()
    {
        health = MaxHealth;
    }

    public void loseHealth(float healthloss)
    {
        health -= healthloss;
    }

    public void gainHealth(float healthgain)
    {
        health += healthgain;
    }

    public void SetHealth(float healthValue)
    {
        health = healthValue;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return MaxHealth;
    }
}
