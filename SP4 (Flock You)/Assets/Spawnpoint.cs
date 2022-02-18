using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawnpoint : MonoBehaviour
{
    PlayerValuesManager playerValuesManager;
    public Text displaypoints;

    private void Start()
    {
        playerValuesManager = new PlayerValuesManager();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Interactable"))
        {
            float objectpoints = other.GetComponent<Rigidbody>().mass * 10;
            playerValuesManager.PlusPoints(objectpoints);
            Destroy(other.gameObject);
        }
    }

    private void Update()
    {
        displaypoints.text = "Points: " + playerValuesManager.GetPoints();
    }

}
