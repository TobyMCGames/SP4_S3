using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserver : MonoBehaviour
{
    public bool isPlayerInRange;
    public Transform player;

    private bool isWeaponInRange;
    private Transform weapon;


    // Start is called before the first frame update
    void Start()
    {
        isPlayerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //detect player
        if (other.tag == "Player") {
            if (player == null)
            {
                player = other.transform;
            }
            isPlayerInRange = true;
        }
        //detect weaapon
        else if (other.tag == "Weapon")
        {
            if (weapon == null)
            {
                weapon = other.transform;
            }
            isWeaponInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.transform == player)
        {
            player = null;
            isPlayerInRange = false;
        }
        else if (other.tag == "Weapon")
        {
            if(weapon != null)
                weapon = null;
            isWeaponInRange = false;
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
