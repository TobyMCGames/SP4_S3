using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider healthbar;
    private GameObject myEntity;
    private float EntityMaxHealth;

    void Start()
    {
        healthbar = gameObject.GetComponentInChildren<Slider>();
        myEntity = gameObject.transform.parent.gameObject;
        EntityMaxHealth = myEntity.GetComponent<EntityHealth>().GetMaxHealth();
    }

    void Update()
    {
        float myEntityHealth = myEntity.GetComponent<EntityHealth>().GetHealth();
        healthbar.value = myEntityHealth / EntityMaxHealth;
    }
}
