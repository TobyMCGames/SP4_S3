using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject POV_Agro;
    public GameObject POV_DeAgro;
    private Transform player;
    private int MoveSpeed = 1;
    public NavMeshAgent navMeshAgent;

    // Finite State Machine
    public enum FSM { IDLE, PATROL, AGRO, STUN }
    public FSM curState;
    float Stun_Timer;

    //public Transform[] Waypoints;
    WaypointController waypointController;

    EnemyObserver enemyObserver;


    // Start is called before the first frame update
    void Start()
    {
        curState = FSM.PATROL;
        waypointController = GetComponent<WaypointController>();
        enemyObserver = GetComponent<EnemyObserver>();
    }

    // Update is called once per frame
    void Update()
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
            case FSM.AGRO:
                if (enemyObserver.isPlayerInRange)
                {
                    // Make enemy chase player
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                    //var distance = Vector3.Distance(player.position, transform.position);
                    navMeshAgent.SetDestination(player.position);
                }
                else
                {
                    curState = FSM.PATROL;
                    Debug.Log("Enemy switching to PATROL state");
                }
                break;
            case FSM.STUN:
                if (Stun_Timer > 0)
                    Stun_Timer -= Time.deltaTime;
                else
                {
                    curState = FSM.PATROL;
                    GetComponent<NavMeshAgent>().speed = 3.5f;
                    Debug.Log("Enemy Woke up");
                }
                break;
        }
    }

    public void SetStun(float stunTime)
    {
        curState = FSM.STUN;
        Stun_Timer = stunTime;
        GetComponent<NavMeshAgent>().speed = 0;
        Debug.Log("Enemy Stunned");
    }
}