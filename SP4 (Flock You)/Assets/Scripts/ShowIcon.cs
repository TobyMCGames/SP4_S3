using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowIcon : MonoBehaviour
{
    private List<GameObject> objectList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.childCount > 1)
        {
            if(other.transform.GetChild(0).CompareTag("ObjectIcon") && !objectList.Contains(other.gameObject))
            {
                //other.transform.GetChild(0).gameObject.SetActive(true);
                objectList.Add(other.gameObject);
                GetClosestObject();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.childCount > 1)
        {
            if (other.transform.GetChild(0).CompareTag("ObjectIcon"))
            {
                other.transform.GetChild(0).gameObject.SetActive(false);
                objectList.Remove(other.gameObject);
            }
        }
    }

    public void GetClosestObject()
    {
        GameObject closestObject = null;
        int objectIndex = 0;
        float closestDistance = 0;

        foreach (GameObject thisobject in objectList)
        {
            if(thisobject != null)
            {
                Vector3 posDiff = transform.position - thisobject.transform.position;
                float curDistanceDiff = posDiff.sqrMagnitude;
                if (curDistanceDiff < closestDistance || objectIndex == 0)
                {
                    closestObject = thisobject;
                    closestDistance = curDistanceDiff;
                }
                objectIndex++;
            }
        }

        closestObject.transform.GetChild(0).gameObject.SetActive(true);

    }
}
