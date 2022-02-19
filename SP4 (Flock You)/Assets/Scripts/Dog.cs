using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        switch (curState)
        {
            case FSM.IDLE:
                break;
            case FSM.PATROL:
                if (enemyObserver.isPlayerInRange)
                {
                    curState = FSM.AGRO;
                    Debug.Log("Enemy switching to AGRO state");
                }
                else
                {
                    waypointController.WaypointUpdate();
                }
                break;
            case FSM.DISTRACTED:
                if (enemyObserver.isPlayerInRange)
                {
                    curState = FSM.AGRO;
                    Debug.Log("Enemy switching to AGRO state");
                }
                if (Distract_Timer >= 0)
                    Distract_Timer -= Time.deltaTime;
                else
                {
                    Debug.Log("Enemy lose interest");
                    waypointController.GetClosestWaypoint();
                    curState = FSM.PATROL;
                }
                break;
            case FSM.AGRO:
                if (enemyObserver.isPlayerInRange)
                {
                    // Make enemy chase player
                    player = enemyObserver.player;
                    //var distance = Vector3.Distance(player.position, transform.position); // calculates distance between player and enemy
                    navMeshAgent.SetDestination(player.position);
                }
                else
                {
                    curState = FSM.DISTRACTED;
                    Distract_Timer = 3f; //Consider removing this line if causing issues at your own risk
                    Debug.Log("Enemy switching to DISTRACTED state");
                }
                break;
            case FSM.STUN:
                if (Stun_Timer >= 0)
                    Stun_Timer -= Time.deltaTime;
                else
                {
                    curState = FSM.PATROL;
                    //GetComponent<NavMeshAgent>().speed = 3.5f;
                    Debug.Log("Enemy Woke up");
                }
                break;
        }
    }

}

