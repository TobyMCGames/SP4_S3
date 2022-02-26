using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TimeCheckpoint : MonoBehaviour
{

    bool Collided = false;
    [SerializeField] float TimeExtension = 0f;
    [SerializeField] float points = 0f;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collided = true;
        }
        
    }

    public bool IsCollided()
    {
        return Collided;
    }

    public float GetTimeExtension()
    {
        return TimeExtension;
    }

    public float GetPoints()
    {
        return points;
    }

}
