using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointController : MonoBehaviour
{
    public List<Transform> waypoints;
    Transform currentTarget;
    protected int index = 1;

    public NavMeshAgent navMeshAgent;

    protected bool Reverse = false;
    protected bool atEnd = false;
    protected bool moving = true;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (waypoints.Count > 0 && waypoints[0] != null)
        {
            currentTarget = waypoints[index];
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
        
    }

    public void WaypointUpdate()
    {
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
            index++;
        }

        //If not at last waypoint and not going in reverse
        if (index < waypoints.Count && !Reverse)
        {
            //Pause a random amount of time before going to the first point
            if (index == 1)
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            currentTarget = waypoints[index];
        }
        else
        {
            if (!atEnd)
            {
                atEnd = true;
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }

            index--;
            Reverse = true;

            //if next waypoint is the first waypoint reset Reverse and atEnd
            if (index == 0)
            {
                Reverse = false;
                atEnd = false;
            }

            currentTarget = waypoints[index];
        }
        //Move agent to next waypoint
        navMeshAgent.SetDestination(currentTarget.position);
        moving = true;
    }

    public void ChasePlayer()
    {


        currentTarget = waypoints[index];
        navMeshAgent.SetDestination(currentTarget.position);

    }
}
