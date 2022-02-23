using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviour
{
    private PhotonView photonView;

    protected Transform player;
    protected Transform nest;
    protected int MoveSpeed = 1;
    public NavMeshAgent navMeshAgent;

    // Finite State Machine
    public enum FSM { IDLE, PATROL, DISTRACTED, AGRO, STUN };
    public FSM curState;

    //Init Variables
    public Collider playerInvolved;
    protected WaypointController waypointController;
    public EnemyObserver enemyObserver;
    protected float DistractTime;
    protected float Stun_Timer;
    protected float Distract_Timer;
    protected float defaultDetectionRadius;
    public float distanceBetweenPlayer;
    protected bool isMeleeAttacking;

    public enum EnemyType{HUMAN, DOG, CAT}
    public EnemyType enemyType;

    void Awake()
    {
        curState = FSM.PATROL;
        isMeleeAttacking = false;
        photonView = GetComponent<PhotonView>();
        waypointController = GetComponent<WaypointController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultDetectionRadius = enemyObserver.GetComponent<CapsuleCollider>().radius;
        OverridenInit();
    }

    protected virtual void OverridenInit()
    {
        //add any variables that are not part of the base init 
        DistractTime = 3f;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!PhotonNetwork.IsMasterClient)
                return;

        // can be overiden
        switch (curState)
        {
            case FSM.IDLE:
                IdleState();
                break;
            case FSM.PATROL:
                PatrolState();
                break;
            case FSM.DISTRACTED:
                DistractedState();
                break;
            case FSM.AGRO:
                AgroState();
                break;
            case FSM.STUN:
                StunState();
                break;
        }

        
    }

    protected virtual void IdleState() // cat exclusive
    {
        return;
    }

    protected virtual void PatrolState()
    {
        SetDetectionRange(defaultDetectionRadius);

        if (enemyObserver.isPlayerInRange)
        {
            curState = FSM.AGRO;
            Debug.Log("Enemy switching to AGRO state");
        }
        else if (enemyType == Enemy.EnemyType.CAT)
        {
            if (enemyObserver.isNestInRange)
            {
                curState = FSM.IDLE;
                Debug.Log("Cat switching to [Nest destroying] state");
            }
            else
            {
                waypointController.WaypointUpdate();
            }
        }
        else
            waypointController.WaypointUpdate();
    }

    protected virtual void DistractedState()
    {
        SetDetectionRange(defaultDetectionRadius * 3f);

        if (enemyObserver.isPlayerInRange)
        {
            curState = FSM.AGRO;
            Debug.Log("Enemy switching to AGRO state");
        }
        if (Distract_Timer >= 0)
            Distract_Timer -= Time.deltaTime;
        else
        {
            Debug.Log("Enemy lose interest");
            waypointController.GetClosestWaypoint();
            curState = FSM.PATROL;
            Distract_Timer = DistractTime;
        }
    }

    protected virtual void AgroState()
    {
        if (enemyObserver.isPlayerInRange)
        {
            // Make enemy chase player
            distanceBetweenPlayer = Vector3.Distance(transform.position, player.position);
            
            player = enemyObserver.player;
            SetDetectionRange(defaultDetectionRadius * 4f);
            navMeshAgent.SetDestination(player.position);

            //Deal damage to player
            if (isMeleeAttacking == true && distanceBetweenPlayer <= 1f)
            {
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    if (playerInvolved.GetComponent<PhotonView>().IsMine)
                        playerInvolved.GetComponent<FYPlayer>().Damaged(1);
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                    playerInvolved.GetComponent<FYPlayer>().Damaged(1);

                Debug.Log("Enemy melee attacked a player");
            }

        }
        else
        {
            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime;
            Debug.Log("Enemy switching to DISTRACTED state");
        }
    }

    protected virtual void SetDetectionRange(float radius)
    {
        enemyObserver.GetComponent<CapsuleCollider>().radius = radius;
    }

    protected virtual void StunState()
    {
        if (Stun_Timer >= 0)
            Stun_Timer -= Time.deltaTime;
        else
        {
            curState = FSM.DISTRACTED;
            Distract_Timer = DistractTime;
            GetComponent<NavMeshAgent>().speed = 3.5f;
            Debug.Log("Enemy Woke up");
        }
    }

    public void SetStun(float stunTime)
    {
        curState = FSM.STUN;
        Stun_Timer = stunTime;
        GetComponent<NavMeshAgent>().speed = 0;
        Debug.Log("Enemy Stunned");
    }

    public void Distracted(Vector3 PlayerLastPos)
    {
        //if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
        //    if (!PhotonNetwork.IsMasterClient)
        //        return;
        curState = FSM.DISTRACTED;
        Distract_Timer = DistractTime;
        //waypointController.MoveToLocation(PlayerLastPos);
        navMeshAgent.SetDestination(PlayerLastPos);
    }

    public void DealDamageToPlayer(Collider playerInvolved)
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            if (playerInvolved.GetComponent<PhotonView>().IsMine)
                playerInvolved.GetComponent<FYPlayer>().Damaged(1);
        }
        else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
            playerInvolved.GetComponent<FYPlayer>().Damaged(1);

        Debug.Log("Enemy melee attacked a player");
    }
}