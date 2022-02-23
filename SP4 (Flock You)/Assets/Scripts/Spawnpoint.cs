using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawnpoint : MonoBehaviour
{
    public Text displaypoints;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Interactable"))
        {
            float objectpoints = other.GetComponent<Rigidbody>().mass * 10;
            Destroy(other.gameObject);
        }
    }

    private void Update()
    {

    }

}
