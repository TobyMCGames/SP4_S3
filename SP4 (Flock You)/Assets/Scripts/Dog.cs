using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Enemy
{
    float distanceBetweenPlayer;
    //Vector3;

    protected override void Update()
    {
        base.Update();
    }

    protected override void AgroState()
    {
        if (enemyObserver.isPlayerInRange)
        {
            // Make enemy chase player
            player = enemyObserver.player;
            SetDetectionRange(defaultDetectionRadius * 4f);
            navMeshAgent.SetDestination(player.position);
            // If dog is within range, pounce on player
            distanceBetweenPlayer = Vector3.Distance(transform.position, player.position); //finds distance
            if (distanceBetweenPlayer <= 2.6f)
            {
                Debug.Log("Distance between dog and player: " + distanceBetweenPlayer);
                // Add more script here to start pouncing

            }
        }
        else
        {
            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime; //Consider removing this line if causing issues at your own risk
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }
}

