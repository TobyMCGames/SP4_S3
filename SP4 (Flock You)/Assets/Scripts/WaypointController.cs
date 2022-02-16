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
        //if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && waypoints.Count > 0)
        //{
        //    m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Count;
        //    navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        //}

        // Get current speed percent of agent and set the speed parameter of the animator
        float speedPercent = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
        
        if (currentTarget != null)
        {
            //Check if the agent has arrived the target position
            if ((Vector3.Distance(transform.position, currentTarget.position) <= 2f) && moving)
            {
                // Set moving to false to prevent this if statement from constantly running while at target position
                moving = false;
                StartCoroutine("MoveToNextWaypoint");
            }

        }
    }

    IEnumerator MoveToNextWaypoint() {
        if (!Reverse)
        {
            _index++;
        }

        //If not at last waypoint and not going in reverse
        if (_index < waypoints.Count && !Reverse)
        {
            //Pause a random amount of time before going to the first point
            if (_index == 1)
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            currentTarget = waypoints[_index];
        }
        else
        {
            if (!atEnd)
            {
                atEnd = true;
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }

            _index--;
            Reverse = true;

            //if next waypoint is the first waypoint reset _inReverse and _atEnd
            if (_index == 0)
            {
                Reverse = false;
                atEnd = false;
            }

            currentTarget = waypoints[_index];
        }
        //Move agent to next waypoint
        navMeshAgent.SetDestination(currentTarget.position);
        moving = true;
    }
}
