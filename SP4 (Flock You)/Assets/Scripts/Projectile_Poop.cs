using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Poop : MonoBehaviour
{
    private Vector3 direction;
    private float Velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy")
        {
            Debug.Log("Poop drop on non-enemy");
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
