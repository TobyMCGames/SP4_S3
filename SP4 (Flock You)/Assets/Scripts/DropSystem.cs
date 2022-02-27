using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DropSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float ForcerequiredtoBreak;
    [SerializeField] GameObject interactlist;
    private Rigidbody m_rigidbody;
    bool isbroken = false;
    float destructiontime = 5f;
    PhotonView photonview;
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        photonview = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InteractItem interactscript;
        foreach (Transform t in transform)
        {
            if (interactscript = t.gameObject.GetComponent<InteractItem>())
            {
                interactscript.InteractableList = interactlist.transform;
                t.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isbroken == true)
        {
            if (destructiontime <= 0)
                Destroy(gameObject);
            else
                destructiontime -= Time.deltaTime;
            return;
        }
        if (m_rigidbody.velocity.sqrMagnitude >= ForcerequiredtoBreak && !GetComponent<InteractItem>().IsHeldonto())
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                photonview.RPC("ReleaseInventory", RpcTarget.AllViaServer);
            }
            else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                ReleaseInventory();
        }
    }

    [PunRPC]
    public void ReleaseInventory()
    { 
        foreach (Transform t in transform)
        {
             t.SetParent(interactlist.transform);
            t.gameObject.SetActive(true);
        }
        isbroken = true;
        gameObject.SetActive(false);
    }
}
