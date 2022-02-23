using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserver : MonoBehaviour
{
    public bool isPlayerInRange;
    public Transform player;

    private bool isWeaponInRange;
    private Transform weapon;

    public bool isNestInRange; // cat exclusive
    public Transform peckable;


    // Start is called before the first frame update
    void Start()
    {
        isPlayerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //detect player
        if (other.CompareTag("Player")) {
            if (player == null)
            {
                player = other.transform;
            }
            isPlayerInRange = true;
        }
        //detect weapon
        else if (other.CompareTag("Weapon"))
        {
            if (weapon == null)
            {
               weapon = other.transform;
            }
            isWeaponInRange = true;
        }
        //detect nest (cat exclusive)
        else if (other.CompareTag("Peckable"))
        {
            if (peckable == null)
            {
                peckable = other.transform;
            }
            isNestInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.transform == player)
        {
            player = null;
            isPlayerInRange = false;
        }
        else if (other.CompareTag("Weapon"))
        {
            if(weapon != null)
            {
                weapon = null;
            }
            isWeaponInRange = false;
        }
        else if (other.CompareTag("Peckable"))
        {
            peckable = null;
            isNestInRange = false;
        }
    }

    public Transform GetWeaponTransform()
    {
        return weapon;
    }

    public bool WeaponInRange()
    {
        return isWeaponInRange;
    }

}
