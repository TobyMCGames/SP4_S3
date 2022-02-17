using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    //Finite State Machine
    public enum FSM { IDLE, PATROL, DISTRACTED, AGRO, STUN }
    public FSM curState;
    float Stun_Timer;
    float Distract_Timer;

    //Init Variables
    WaypointController waypointController;
    private NavMeshAgent navMeshAgent;
    public GameObject POV_Agro;
    public GameObject POV_DeAgro;

    void Start()
    {
        curState = FSM.PATROL;
        waypointController = GetComponent<WaypointController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case FSM.IDLE:        
                break;
            case FSM.PATROL:
                waypointController.WaypointUpdate();
                break;
            case FSM.DISTRACTED:
                if (Distract_Timer >= 0)
                    Distract_Timer -= Time.deltaTime;
                else
                {
                    curState = FSM.PATROL;
                    Debug.Log("Enemy lose interest");
                }
                break;
            case FSM.AGRO:
                break;
            case FSM.STUN:
                if (Stun_Timer >= 0)
                    Stun_Timer -= Time.deltaTime;
                else
                {
                    curState = FSM.PATROL;
                    Debug.Log("Enemy Woke up");
                }
                break;
        }
    }

    public void SetStun(float stunTime)
    {
        curState = FSM.STUN;
        Stun_Timer = stunTime;
        Debug.Log("Enemy Stunned");
    }

    private void Distracted(Vector3 PlayerLastPos)
    {
        Distract_Timer = 3f;
        navMeshAgent.SetDestination(PlayerLastPos);
    }

    public void SetDistracted(Vector3 SetPlayerLastPos)
    {
        curState = FSM.DISTRACTED;
        //waypointController.SetMoveAwayStatus(true);
        Distracted(SetPlayerLastPos);
    }
}