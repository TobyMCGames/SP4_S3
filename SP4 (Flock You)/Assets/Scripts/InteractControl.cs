using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractControl : MonoBehaviour
{
    private GameObject heldObject;
    private GameObject player;
    private int heldObjectWeight;
    private GameObject touchPointLight, touchPointHeavy;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        touchPointLight = player.transform.Find("TouchPoint_Light").gameObject;
        touchPointHeavy = player.transform.Find("TouchPoint_Heavy").gameObject;
    }

    void Update()
    {
        Debug.Log(touchPointLight.transform.position);
        if (Input.GetMouseButton(0))
        {
            heldObject = GetComponentInChildren<InteractDetection>().GetInteractable();

            if(heldObject != null)
            {
                Debug.Log("HeldObject: " + heldObject.name);
                heldObjectWeight = heldObject.GetComponent<Interactable>().weight;

                if (heldObjectWeight >= 5 && heldObjectWeight < 20)
                {
                    heldObject.transform.position = touchPointLight.transform.position;
                }
                else if (heldObjectWeight >= 20)
                {
                    heldObject.transform.position = touchPointHeavy.transform.position;
                }
            }
        }
    }
}
