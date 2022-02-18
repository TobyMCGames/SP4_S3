using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Poop : MonoBehaviour
{
    private Vector3 direction;
    private float Velocity;
    // Use Players PhotonView Instead in Multiplayer (maybe) or use GO instead
    Transform pooper = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (pooper != null)
            if (other.transform == pooper || other.transform.parent.CompareTag("Player"))
                return;

        if (other.tag == "Enemy")
        {
            other.transform.GetComponent<Enemy>().SetStun(5.0f);
        }
        Debug.Log("Poop drop on non-enemy");
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignPooper(Transform _pooper)
    {
        pooper = _pooper;
    }
}
