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

    void Awake()
    {
        // init variables
        player = gameObject;
        photonView = gameObject.GetPhotonView();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if(Input.GetMouseButtonDown(0))
        {
            peckableObject = player.GetComponentInChildren<InteractDetection>().GetPeckable();
            if (peckableObject != null) // if object is peckable
            {
                if(peckableObject.name == "NestSpawn(Clone)") // if object is a nest
                {
                    if(peckableObject.GetComponent<NestSpawnScript>().GetWhoSpawnMe() != gameObject) //If the nest owner is different from the current player
                    {
                        peckableObject.GetComponent<NestSpawnScript>().SetCanBeDamaged(true); // Deals damage to nest
                    }
                }
                else
                    peckableObject.GetComponent<Peckable>().SetCanBeDamaged(true); // Deals damage to any peckable object
            }

        }

        //if click and hold
        if (Input.GetMouseButton(0))
        {
            heldObject = GetComponentInChildren<InteractDetection>().GetInteractable();
            if (heldObject != null && heldObject.transform.parent != touchPointLight.transform && heldObject.transform.parent != touchPointHeavy.transform)
            {
                heldObjectWeight = heldObject.GetComponent<Rigidbody>().mass;

                //light object
                if (heldObjectWeight >= 1 && heldObjectWeight <= 7)
                {
                    if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                        heldObject.GetPhotonView().RPC("SetHolder", RpcTarget.AllViaServer, photonView.ViewID, 2);
                    else
                        heldObject.GetComponent<InteractItem>().SetHolder(touchPointLight.transform);
                    //heldObject.transform.position = touchPointLight.transform.position;
                    heldObject.GetComponent<Collider>().enabled = false;
                }
                //heavy object
                else if (heldObjectWeight > 7)
                {
                    if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                        heldObject.GetPhotonView().RPC("SetHolder", RpcTarget.AllViaServer, photonView.ViewID, 3);
                    else
                        heldObject.GetComponent<InteractItem>().SetHolder(touchPointHeavy.transform);
                    //heldObject.transform.position = touchPointHeavy.transform.position;
                }

                //change player speed according to weight of object
                speedChange = player.GetComponent<MovePlayer>().GetDefaultMoveMul() * 0.5f;
                //set player speed to new speed
                player.GetComponent<MovePlayer>().movemul = speedChange;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            Debug.Log("Released L Mouse btn in interactable controls");

            if (heldObject != null)
            {
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                    heldObject.GetPhotonView().RPC("DropItem", RpcTarget.AllViaServer);
                else
                    heldObject.GetComponent<InteractItem>().DropItem();

                heldObject.GetComponent<Collider>().enabled = true;
                //default speed
                player.GetComponent<MovePlayer>().movemul = player.GetComponent<MovePlayer>().GetDefaultMoveMul();
            }
        }
    }
}
