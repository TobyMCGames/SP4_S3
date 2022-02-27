using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cat : Enemy
{
    protected void Start()
    {
        enemyType = EnemyType.CAT;
    }

    protected override void IdleState()
    {
        if (enemyObserver.isNestInRange)
        {
            //this runs when the cat hits the nest once
            nest = enemyObserver.peckable;
            if (nest != null) // nest is present
            {
                if (Vector3.Distance(transform.position, nest.position) <= 1.8f)
                {
                    navMeshAgent.speed = 0f;
                    nest.GetComponent<NestSpawnScript>().SetCanBeDamaged(true);
                }
                else // went out of range
                {
                    navMeshAgent.speed = 3.5f;
                    navMeshAgent.SetDestination(nest.position);
                }
            }
            else // nest gon
            {
                enemyObserver.isNestInRange = false;
                navMeshAgent.speed = 3.5f;
            }
            curState = FSM.DISTRACTED;
            Debug.Log("Cat switching to DISTRACTED state");
            Distract_Timer = DistractTime;
        }
        else
        {
            curState = FSM.PATROL;
            Debug.Log("Enemy switching to PATROL state");
        }
    }

    protected override void AgroState()
    {
        if (enemyObserver.isPlayerInRange)
        {
            player = enemyObserver.player;
            // Make enemy chase player
            distanceBetweenPlayer = Vector3.Distance(transform.position, player.position);

            navMeshAgent.speed = 6.5f;
            SetDetectionRange(defaultDetectionRadius * 4f);
            navMeshAgent.SetDestination(player.position);

            //Deal damage to player
            if (enemyObserver.isPlayerInRange == true && distanceBetweenPlayer <= 8.3f)
            {
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                        player.GetComponent<FYPlayer>().Damaged(1);
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                    player.GetComponent<FYPlayer>().Damaged(1);

                Debug.Log("Enemy melee attacked a player");
            }
        }
        else
        {
            curState = FSM.DISTRACTED;
            navMeshAgent.speed = 3.5f;
            Distract_Timer = DistractTime;
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        playerInvolved = other;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        isMeleeAttacking = false;
    //    }
    //}
}
