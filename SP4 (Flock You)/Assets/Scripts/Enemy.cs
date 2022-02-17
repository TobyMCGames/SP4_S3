using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject POV_Agro;
    public GameObject POV_DeAgro;
    
    // Finite State Machine
    public enum FSM { IDLE, PATROL, AGRO, STUN }
    public FSM curState;
    float Stun_Timer;

    //public Transform[] Waypoints;
    WaypointController waypointController;


    // Start is called before the first frame update
    void Start()
    {
        curState = FSM.PATROL;
        waypointController = GetComponent<WaypointController>();

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
            case FSM.AGRO:
                
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