using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimeCheckpoint : MonoBehaviour
{
    
    [SerializeField] float TimeExtension = 0f;
    [SerializeField] int points = 0;
    bool Collided = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent.parent.GetComponent<TimeTrial>().initTargetPlayer(other.gameObject);
            Collided = true;
        }
    }

    public float GetTimeExtension()
    {
        return TimeExtension;
    }

    public int GetPoints()
    {
        return points;
    }

    public bool isCollided()
    {
        return Collided;
    }
    public void ResetCheckpoint()
    {
        Collided = false;
    }
}

