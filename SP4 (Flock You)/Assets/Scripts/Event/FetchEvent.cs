using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FetchEvent : BaseEvent
{
    // Start is called before the first frame update

    public bool IsfetchGlobal = false;  
    public GameObject FetchItem = null;
    public PhotonView carriedby = null;


    public override void Start()
    {
        // randomize the FetchItem's location
        Init();
        Vector3 randpos = Vector3.zero;
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            Debug.Log("Fetch Event does not have a box collider");
            return;
        }
        // doesnt care about fetch item's size
        Vector3 collidersize = collider.size;
        collidersize *= 0.5f;
        randpos.x = Random.Range(-collidersize.x,collidersize.x);
        randpos.y = Random.Range(-collidersize.y,collidersize.y);
        randpos.z = Random.Range(-collidersize.z,collidersize.z);

        FetchItem.transform.localPosition = randpos;
        EventManager.Instance.AddEvent(EventView,gameObject);
    }

    public override void Awake()
    {
        Eventtype = EventType.EVENT_FETCH;
    }

    public override void Update()
    {
        
    }

    public void Init()
    {
        if (IsfetchGlobal)
        {
            foreach (Transform player in FYGameManager.Instance.PlayerList)
            {
                EventTarget.Add(player.GetComponent<PhotonView>());
            }
        }
    }
    public void setCarriedby(PhotonView _view)
    {
        carriedby = _view;
    }
}
