using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractControl : MonoBehaviour
{
    private GameObject heldObject;
    private GameObject player;
    private float heldObjectWeight;
    public GameObject touchPointLight, touchPointHeavy;
    private float speedChange;
    private float playerSpeed;

    private void Start()
    {
        // init variables
        player = gameObject;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Released L Mouse btn in interactable controls");
            //default speed
            player.GetComponent<MovePlayer>().movemul = player.GetComponent<MovePlayer>().GetDefaultMoveMul();
        }

        //if click
        if (Input.GetMouseButton(0))
        {
            heldObject = GetComponentInChildren<InteractDetection>().GetInteractable();
                if (heldObject != null)
                {
                    //heldObjectTP = heldObject.transform.GetChild(0).gameObject;
                    //Debug.Log(heldObjectTP);
                    heldObjectWeight = heldObject.GetComponent<Rigidbody>().mass;
                    //Transform touchpoint = heldObject.transform.Find("touchpoint").transform;
                    //Vector3 touchpointOffset = transform.localPosition - touchpoint.localPosition;

                    //light object
                    if (heldObjectWeight >= 1 && heldObjectWeight <= 7)
                    {
                        heldObject.transform.position = touchPointLight.transform.position;
                        heldObject.GetComponent<Collider>().enabled = false;
                    }
                    //heavy object
                    else if (heldObjectWeight > 7)
                    {
                        heldObject.transform.position = touchPointHeavy.transform.position;
                    }

                    //change player speed according to weight of object
                    speedChange = player.GetComponent<MovePlayer>().GetDefaultMoveMul() * 0.5f;
                    //set player speed to new speed
                    player.GetComponent<MovePlayer>().movemul = speedChange;
                }
        }
        else
        {
            if (heldObject != null)
            {
                heldObject.GetComponent<Collider>().enabled = true;
            }
        }
    }
}
