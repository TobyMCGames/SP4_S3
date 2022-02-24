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
        // To do: use this for points
        if (owner == null)
            return;

        if (!other.gameObject.CompareTag("FetchItem") && !other.gameObject.CompareTag("Interactable"))
            return;

        if (other.gameObject.CompareTag("FetchItem"))
        {
            FetchEvent fetchScript = other.gameObject.GetComponent<InteractItem>().InteractableList.GetComponent<FetchEvent>();
            if (fetchScript != null)
            {
                fetchScript.SetCompleted(true);
                fetchScript.ToggleDestruction();
                fetchScript.FetchItem.SetActive(false);
            }
        }

        if (other.gameObject.CompareTag("Interactable"))
        {
            owner.playerGameInfo.iPoints += (int)other.GetComponent<Rigidbody>().mass * 10;
            UI_Manager.Instance.UpdatePlayerScore(owner.playerGameInfo.sUsername, owner.playerGameInfo.iPoints);
            Destroy(other.gameObject);
        }
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
