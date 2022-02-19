using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected Transform player;
    protected int MoveSpeed = 1;
    public NavMeshAgent navMeshAgent;

    // Finite State Machine
    public enum FSM { IDLE, PATROL, DISTRACTED, AGRO, STUN };
    public FSM curState;
    protected float Stun_Timer;
    protected float Distract_Timer;
    protected float defaultDetectionRadius;

    //Init Variables
    protected WaypointController waypointController;
    public EnemyObserver enemyObserver;
    protected float DistractTime;



    void Start()
    {
        curState = FSM.PATROL;
        waypointController = GetComponent<WaypointController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultDetectionRadius = enemyObserver.GetComponent<CapsuleCollider>().radius;
        OverridenInit();
    }

    protected virtual void OverridenInit()
    {
        //add any variables that are not part of the base init 
        DistractTime = 3f;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        // can be overiden
        switch (curState)
        {
            case FSM.IDLE:
                break;
            case FSM.PATROL:
                PatrolState();
                break;
            case FSM.DISTRACTED:
                DistractedState();
                break;
            case FSM.AGRO:
                AgroState();
                break;
            case FSM.STUN:
                StunState();
                break;
        }
    }

    protected virtual void PatrolState()
    {
        SetDetectionRange(defaultDetectionRadius);

        if (enemyObserver.isPlayerInRange)
        {
            curState = FSM.AGRO;
            Debug.Log("Enemy switching to AGRO state");
        }
        else
            waypointController.WaypointUpdate();
    }

    protected virtual void DistractedState()
    {
        SetDetectionRange(defaultDetectionRadius * 3f);

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
    }

    protected virtual void AgroState()
    {
        if (enemyObserver.isPlayerInRange)
        {
            // Make enemy chase player
            player = enemyObserver.player;
            SetDetectionRange(defaultDetectionRadius * 4f);
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime; //Consider removing this line if causing issues at your own risk
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }

    protected virtual void SetDetectionRange(float radius)
    {
        enemyObserver.GetComponent<CapsuleCollider>().radius = radius;
    }

    protected virtual void StunState()
    {
        if (Stun_Timer >= 0)
            Stun_Timer -= Time.deltaTime;
        else
        {
            curState = FSM.PATROL;
            GetComponent<NavMeshAgent>().speed = 3.5f;
            Debug.Log("Enemy Woke up");
        }
    }


    public void SetStun(float stunTime)
    {
        curState = FSM.STUN;
        Stun_Timer = stunTime;
        GetComponent<NavMeshAgent>().speed = 0;
        Debug.Log("Enemy Stunned");
    }

    public void Distracted(Vector3 PlayerLastPos)
    {
        curState = FSM.DISTRACTED;
        Distract_Timer = DistractTime;
        //waypointController.MoveToLocation(PlayerLastPos);
        navMeshAgent.SetDestination(PlayerLastPos);
    }

}