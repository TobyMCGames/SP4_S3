using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractControl : MonoBehaviour
{
    private PhotonView photonView;
    private GameObject heldObject;
    private GameObject peckableObject;
    private GameObject player;
    private float heldObjectWeight;
    public GameObject touchPointLight, touchPointHeavy;
    private float speedChange;
    private float playerSpeed;

    private void Awake()
    {
        // init variables
        player = gameObject;
        photonView = gameObject.GetPhotonView();
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Released L Mouse btn in interactable controls");
            //default speed
            player.GetComponent<MovePlayer>().movemul = player.GetComponent<MovePlayer>().GetDefaultMoveMul();
        }

        if(Input.GetMouseButtonDown(0))
        {
            peckableObject = player.GetComponentInChildren<InteractDetection>().GetPeckable();
            if (peckableObject != null)
            {
                peckableObject.GetComponent<Peckable>().SetCanBeDamaged(true);
            }

        }

        //if click and hold
        if (Input.GetMouseButton(0))
        {
            heldObject = GetComponentInChildren<InteractDetection>().GetInteractable();
            if (heldObject != null)
            {
                heldObjectWeight = heldObject.GetComponent<Rigidbody>().mass;

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
