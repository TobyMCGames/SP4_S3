using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDetection : MonoBehaviour
{
    private bool collided;
    private GameObject Interactable = null;
    public GameObject GetInteractable() { return Interactable; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Interactable") && Interactable == null)
        {
            Interactable = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Interactable == null)
            return;
        else if (other.gameObject.tag == "Interactable")
        {
            if (other.gameObject == Interactable)
                Interactable = null;
        }
    }
}
