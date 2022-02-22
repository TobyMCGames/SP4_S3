using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Enemy
{
    float distanceBetweenPlayer;
    //Vector3;
    Rigidbody rigidbody;

    //CharacterController controller;
    float jumpPower = 5f;
    Enemy enemy;
    public bool isPouncing;
    private GameObject playerObj = null;

    protected void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        rigidbody = GetComponent<Rigidbody>();
    }

    //protected override void Update()
    //{
    //    base.Update();
    //}

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
                if (isPouncing == false)
                {
                    Pounce();
                }
            }
            else
            {
                if (isPouncing == false)
                    navMeshAgent.SetDestination(player.position);
            }
        }
        else
        {
            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime; //Consider removing this line if causing issues at your own risk
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }
    //IEnumerator WaitFor1To2s()
    //{
    //    yield return new WaitForSeconds(Random.Range(1f, 2f));
    //}

    protected void Pounce()
    {
        //if (isPouncing == false)
        {
            isPouncing = true;
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false; // turn off NavMeshAgent
            //WaitFor1To2s();
            //Debug.Log("Distance between dog and player: " + distanceBetweenPlayer);
            // Look towards player direction
            transform.LookAt(player.position);
            // Pounce!
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            rigidbody.AddForce(Vector3.forward, ForceMode.Impulse);
            // When dog touches floor, re-enable NavMeshAgent

        }
        //transform.LookAt(player.position);
        //if ((controller.collisionFlags & CollisionFlags.Below) != 0)
        //{
        //    print("touched the floor");
        //    isPouncing = false;
        //    GetComponent<NavMeshAgent>().enabled = true; // resume NavMeshAgent
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            if (isPouncing == true)
            {
                //print("touched the floor");
                isPouncing = false;
                GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true; // resume NavMeshAgent
            }

        }
    }

    //private void OnTriggerExit(Collider other)
    //{
        
    //}
}

