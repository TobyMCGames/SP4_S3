using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointController : MonoBehaviour
{
    //public List<Transform> waypoints = new List<Transform>();
    public List<Transform> waypoints;
    Transform currentTarget;
    private int _index = 1;

    public NavMeshAgent navMeshAgent;

    private bool Reverse = false;
    private bool atEnd = false;
    private bool moving = true;

    int m_CurrentWaypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if(waypoints.Count > 0 && waypoints[0] != null)
        {
            currentTarget = waypoints[_index];
            navMeshAgent.SetDestination(currentTarget.position);
        }
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
