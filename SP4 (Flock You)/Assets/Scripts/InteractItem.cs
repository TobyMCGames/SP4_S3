using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractItem : MonoBehaviour
{
    public PhotonView photonView;
    public Transform InteractableList;

    private void Awake()
    {
        photonView = gameObject.GetPhotonView();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (transform.parent == InteractableList)
            return;

        transform.localPosition = Vector3.zero;
    }

    public void SetHolder(Transform _holder)
    {
        transform.SetParent(_holder);
    }

    [PunRPC]
    public void DropItem()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        transform.SetParent(InteractableList);
    }

    [PunRPC]
    public void SetHolder(int ViewID, int child)
    {
        GameObject _holder = PhotonNetwork.GetPhotonView(ViewID).gameObject;
        transform.SetParent(_holder.transform.GetChild(child));
        GameObject baseEventobj = EventManager.Instance.getEventobj(PhotonNetwork.GetPhotonView(ViewID));
        if (baseEventobj == null)
            return;
        else
        {
            switch (baseEventobj.GetComponent<BaseEvent>().GetEventType())
            {
                case BaseEvent.EventType.EVENT_FETCH:
                    baseEventobj.GetComponent<FetchEvent>().setCarriedby(PhotonNetwork.GetPhotonView(ViewID));
                    EventManager.Instance.UpdateEventobj(PhotonNetwork.GetPhotonView(ViewID), baseEventobj);
                    break;
            }

        }
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
    }
}
