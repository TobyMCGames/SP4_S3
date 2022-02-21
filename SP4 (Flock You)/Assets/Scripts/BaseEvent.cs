using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BaseEvent :MonoBehaviour
{
    public enum EventType
    {
        EVENT_FETCH,
        EVENT_CAPTURE,
        NUM_EVENTS,
    }

    public EventType Eventtype;
    private Collider EventArea;
    private PhotonView EventTarget;
    
    

    
    public virtual bool IsReqMet()
    {
        return false;
    }
    public virtual bool IsEventValid()
    {
        return false;
    }

    public virtual void RunEvent()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        EventArea = null;
    }
    PhotonView GetEventTarget()
    {
        return EventTarget;
    }
    
    void SetEventTarget(PhotonView photonview)
    {
        EventTarget = photonview;
    }
}
