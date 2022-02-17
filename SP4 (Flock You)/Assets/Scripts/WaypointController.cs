using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointController : MonoBehaviour
{
    public List<Transform> waypoints;
    Transform currentTarget;
    protected int _index = 1;

    public NavMeshAgent navMeshAgent;

    protected bool Reverse = false;
    protected bool atEnd = false;
    protected bool moving = true;

    private bool moveAway = false;
    private List<float> waypointsDistDiff;

    int m_CurrentWaypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (waypoints.Count > 0 && waypoints[0] != null)
        {
            currentTarget = waypoints[_index];
            navMeshAgent.SetDestination(currentTarget.position);
        }
        else
            Debug.Log("No waypoint targets set!");

        moveAway = false;
    }

    public void SetMoveAwayStatus(bool status)
    {
        moveAway = status;
    }

    public bool GetMoveAwayStatus()
    {
        return moveAway;
    }

    public void StartAI()
    {
        enabled = true;
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    public void WaypointUpdate()
    {
        // Get current speed percent of agent and set the speed parameter of the animator
        float speedPercent = navMeshAgent.velocity.magnitude / navMeshAgent.speed;

        if (currentTarget != null)
        {
            //Check if the agent has arrived the target position
            if ((Vector3.Distance(transform.position, currentTarget.position) <= 2f) && moving)
            {
                // Set moving to false to prevent this if statement from constantly running while at target position
                moving = false;
                Debug.Log("Moving = false");
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
                Debug.Log("At end");
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
        Debug.Log("Moving = true");
    }
}
