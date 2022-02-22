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
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (transform.parent == InteractableList)
            return;

        transform.localPosition = Vector3.zero;
    }

    public void SetHolder(Transform _holder)
    {
        transform.SetParent(_holder);
        FetchEvent fetchscript = InteractableList.GetComponent<FetchEvent>();
        if (fetchscript != null) // check if this object is a fetch item
        {
            fetchscript.setSingleCarriedby(_holder.parent);
        }
    }

    [PunRPC]
    public void DropItem()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        transform.SetParent(InteractableList);
        FetchEvent fetchscript = InteractableList.GetComponent<FetchEvent>();
        if (fetchscript != null)
        {
            fetchscript.setSingleCarriedby(null);
        }
    }

    [PunRPC]
    public void SetHolder(int ViewID, int child)
    {
        GameObject _holder = PhotonNetwork.GetPhotonView(ViewID).gameObject;
        transform.SetParent(_holder.transform.GetChild(child));
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
    }
}
