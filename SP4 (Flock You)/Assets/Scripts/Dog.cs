using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dog : Enemy
{
    float distanceBetweenPlayer;
    Rigidbody rigidbody;
    float jumpPower = 20f;
    Enemy enemy;
    public bool isPouncing;
    bool pounceSuccess;
    private GameObject playerObj = null;

    protected void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        rigidbody = GetComponent<Rigidbody>();
        isPouncing = false;
        pounceSuccess = false;
        enemyType = EnemyType.DOG;
    }

    protected override void AgroState()
    {
        if (isPouncing)
            return;
        if (enemyObserver.isPlayerInRange)
        {
            // Make enemy chase player
            player = enemyObserver.player;
            SetDetectionRange(defaultDetectionRadius * 4f);
            navMeshAgent.SetDestination(player.position);
            // If dog is within range, pounce on player
            distanceBetweenPlayer = Vector3.Distance(transform.position, player.position); //finds distance
            if (distanceBetweenPlayer <= 2.6f)
            {
                Pounce();
            }
            else
            {
                navMeshAgent.SetDestination(player.position);
            }
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = true; // resume NavMeshAgent
            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime;
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }

    protected void Pounce()
    {
        //if (isPouncing == false)
        {
            isPouncing = true;
            GetComponent<NavMeshAgent>().enabled = false; // turn off NavMeshAgent
            //Debug.Log("Distance between dog and player: " + distanceBetweenPlayer);
            // Look towards player direction
            transform.LookAt(player.position);
            // Pounce!
            //transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            rigidbody.AddForce(Vector3.forward, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            if (isPouncing == true)
            {
                isPouncing = false;
                GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true; // resume NavMeshAgent
            }
            //GetComponent<NavMeshAgent>().enabled = true; // resume NavMeshAgent
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPouncing)
        {
            //Pounce successful!
        }
    }
}