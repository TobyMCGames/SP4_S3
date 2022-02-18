using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowIcon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.Find("Icon"))
        {
            other.transform.Find("Icon").gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.Find("Icon"))
        {
            other.transform.Find("Icon").gameObject.SetActive(false);
        }
    }
}
