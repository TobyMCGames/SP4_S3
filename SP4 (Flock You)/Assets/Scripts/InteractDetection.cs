using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDetection : MonoBehaviour
{
    private GameObject Interactable = null;
    private GameObject PeckableObject = null;
    public GameObject GetInteractable() { return Interactable; }
    public GameObject GetPeckable() { return PeckableObject; }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.CompareTag("Interactable") || other.gameObject.CompareTag("FetchItem")) && Interactable == null)
        {
            Interactable = other.gameObject;
        }

        if (other.gameObject.CompareTag("Peckable") && PeckableObject == null)
        {
            PeckableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Interactable == null && PeckableObject == null)
            return;

        else if (other.gameObject.tag == "Interactable" || other.gameObject.CompareTag("FetchItem"))
        {
            Interactable = null;
        }

        else if (other.gameObject.tag == "Peckable")
        {
            PeckableObject = null;
        }
    }
}
