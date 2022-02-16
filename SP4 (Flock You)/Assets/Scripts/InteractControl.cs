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

                //light object
                if (heldObjectWeight >= 1 && heldObjectWeight <= 7)
                {
                    heldObject.transform.localPosition = touchPointLight.transform.position;
                }
                //heavy object
                else if (heldObjectWeight > 7)
                {
                    heldObject.transform.localPosition = touchPointLight.transform.position;
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
