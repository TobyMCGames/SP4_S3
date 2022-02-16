using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject POV_Agro;
    public GameObject POV_DeAgro;
    public enum FSM { IDLE, PATROL, AGRO, STUN }
    public FSM curState;
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

                break;
        }
    }
}