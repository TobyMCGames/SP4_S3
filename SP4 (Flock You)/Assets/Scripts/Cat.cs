using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Enemy
{
    protected void Start()
    {
        enemyType = EnemyType.CAT;
    }

    protected override void AgroState()
    {
        if (enemyObserver.isPlayerInRange)
        {
            // Make enemy chase player
            player = enemyObserver.player;
            navMeshAgent.speed = 6.5f;
            SetDetectionRange(defaultDetectionRadius * 4f);
            navMeshAgent.SetDestination(player.position);
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
        if (other.tag == "Peckable") // Only affects Nestspawn at time of writing
        {
            // Deals damage to nest
            other.GetComponent<NestSpawnScript>().SetCanBeDamaged(true);
        }
        if (other.tag == "Player")
        {

        }
    }
}
