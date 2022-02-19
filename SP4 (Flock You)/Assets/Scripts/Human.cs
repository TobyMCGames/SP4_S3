using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Enemy
{
    private bool hasWeapon;
    private GameObject EnemyHand;
    private Transform WeaponHolder;
    private GameObject HeldWeapon;
    private bool pickupWeapon;

    protected override void OverridenInit()
    {
        DistractTime = 6f;
        hasWeapon = false;
        pickupWeapon = false;
        EnemyHand = gameObject.transform.Find("hand").gameObject;
        WeaponHolder = EnemyHand.transform.GetChild(0);
    }

    protected override void AgroState()
    {
        if (enemyObserver.isPlayerInRange)
        {
            SetDetectionRange(defaultDetectionRadius * 4f);

            //if human sees a weapon and it deosnt have one
            if (enemyObserver.WeaponInRange() == true && HeldWeapon == null)
            {
                //walk towards weapon 
                navMeshAgent.SetDestination(enemyObserver.GetWeaponTransform().position);
                pickupWeapon = true;
            }
            else
            {
                // Make enemy chase player
                player = enemyObserver.player;
                navMeshAgent.SetDestination(player.position);
            }

            // if human already has a weapon
            if(HeldWeapon != null)
            {
                //make hand face player
                EnemyHand.transform.LookAt(player.transform);
                //make weapon shoot
                HeldWeapon.GetComponent<Weapon>().canShoot = true;
            }
        }
        else
        {
            if (HeldWeapon != null)
            {
                //make weapon stop shooting
                HeldWeapon.GetComponent<Weapon>().canShoot = false;
            }

            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime; 
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(pickupWeapon)
        {
            GameObject weapon = other.gameObject;
            //set the weapon that is picked up as held weapon
            HeldWeapon = weapon;

            // spawn weapon at player hand
            HeldWeapon.transform.SetParent(WeaponHolder);
            weapon.transform.localPosition = new Vector3(0, 0, 0);
            weapon.transform.localRotation = new Quaternion(0,0,0,0);
            Destroy(weapon.GetComponent<Rigidbody>());
            pickupWeapon = false;
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
