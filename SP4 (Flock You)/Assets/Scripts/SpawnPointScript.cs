using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPointScript : MonoBehaviour
{
    public FYPlayer owner;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = gameObject.GetPhotonView();
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.CompareTag("Interactable"))
        {
            FetchEvent fetchScript = other.gameObject.GetComponent<InteractItem>().InteractableList.GetComponent<FetchEvent>();
            if (fetchScript != null)
            {
                fetchScript.SetCompleted(true);
                fetchScript.ToggleDestruction();
                fetchScript.FetchItem.SetActive(false);
            }
        }
        if (owner == null)
            return;
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
