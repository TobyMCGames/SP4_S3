using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    //use List so waypoints can be added in Runtime
    //public Transform[] waypoints;
    public List<Transform> waypoints = new List<Transform>();

    int m_CurrentWaypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartAI()
    {
        enabled = true;
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && waypoints.Count > 0)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Count;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
    }
}
