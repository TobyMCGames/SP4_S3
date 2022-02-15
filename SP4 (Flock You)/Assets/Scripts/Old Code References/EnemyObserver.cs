using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObserver : MonoBehaviour
{
    public bool isPlayerInRange;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && player == other.transform) {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerInRange = false;
        }
    }
}
