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
        if(other.gameObject.tag == "Interactable" && Interactable == null)
        {
            Interactable = other.gameObject;
            Debug.Log(Interactable.name);
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
            Debug.Log("Item removed");
        }
    }
}
