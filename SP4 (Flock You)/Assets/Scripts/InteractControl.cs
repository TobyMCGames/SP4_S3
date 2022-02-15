using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractControl : MonoBehaviour
{
    private GameObject heldObject;
    private GameObject heldObjectTP;
    private GameObject player;
    private float heldObjectWeight;
    private GameObject touchPointLight, touchPointHeavy;
    private float speedChange;
    private float playerSpeed;

    private void Start()
    {
        // init variables
        player = gameObject;
        touchPointLight = player.transform.Find("TouchPoint_Light").gameObject;
        touchPointHeavy = player.transform.Find("TouchPoint_Heavy").gameObject;
    }

    void Update()
    {
        //default speed
        player.GetComponent<PlayerMovement2>().moveSpeed = 0.3f;

        //if click
        if (Input.GetMouseButton(0))
        {
            heldObject = GetComponentInChildren<InteractDetection>().GetInteractable();
            if (heldObject != null)
            {
                //heldObjectTP = heldObject.transform.GetChild(0).gameObject;
                heldObjectWeight = heldObject.GetComponent<Rigidbody>().mass;

                //light object
                if (heldObjectWeight >= 1 && heldObjectWeight <= 7)
                {
                    heldObject.transform.position = touchPointLight.transform.position;
                }
                //heavy object
                else if (heldObjectWeight > 7)
                {
                    heldObject.transform.position = touchPointHeavy.transform.position;
                }

                //change player speed according to weight of object
                playerSpeed = player.GetComponent<PlayerMovement2>().moveSpeed;
                speedChange = playerSpeed * 1/2;
                //set player speed to new speed
                player.GetComponent<PlayerMovement2>().moveSpeed = speedChange;
            }
        }
    }
}
