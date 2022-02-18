using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowIcon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.GetChild(0).CompareTag("ObjectIcon"))
        {
            other.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetChild(0).CompareTag("ObjectIcon"))
        {
            other.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
