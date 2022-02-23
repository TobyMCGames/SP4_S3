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
        if (enemyObserver.isPlayerInRange)
        {
            curState = FSM.AGRO;
            Debug.Log("Cat switching to AGRO state");
        }
        else if (enemyObserver.isNestInRange)
        {
            nest = enemyObserver.peckable;
            if (nest == null)
            {
                curState = FSM.DISTRACTED;
                Debug.Log("Cat switching to DISTRACTED state");
                Distract_Timer = DistractTime * 3;
                enemyObserver.isNestInRange = false;
                return;
            }
            
            navMeshAgent.SetDestination(nest.position);
            //waypointController.WaitForSecondsRandom(3f, 6f);
            if (Vector3.Distance(transform.position, nest.position) <= 1f)
            {
                nest.GetComponent<NestSpawnScript>().SetCanBeDamaged(true);
            }
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
            if (isMeleeAttacking == true && distanceBetweenPlayer <= 1f)
            {
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    if (playerInvolved.GetComponent<PhotonView>().IsMine)
                        playerInvolved.GetComponent<FYPlayer>().Damaged(1);
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                    playerInvolved.GetComponent<FYPlayer>().Damaged(1);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Peckable")) // Only affects Nestspawn at time of writing
        {
            // Deals damage to nest
            //other.GetComponent<NestSpawnScript>().SetCanBeDamaged(true);
        }
        if (other.CompareTag("Player"))
        {
            isMeleeAttacking = true;
            playerInvolved = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isMeleeAttacking = false;
        }
    }
}
