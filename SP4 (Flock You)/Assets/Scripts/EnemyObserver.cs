using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserver : MonoBehaviour
{
    public bool isPlayerInRange;
    public Transform player;
    private bool m_IsPlayerInRange;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            if (player == null)
                player = other.transform;
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.transform == player)
        {
            isPlayerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
