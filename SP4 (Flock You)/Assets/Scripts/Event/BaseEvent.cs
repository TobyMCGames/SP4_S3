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

    [SerializeField] protected EventType Eventtype;
    [SerializeField] protected Collider EventArea;
    [SerializeField] protected List<PhotonView> EventTarget;
    [SerializeField] protected PhotonView EventView;
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

    public virtual void Update()
    {
    }

    public virtual void Start()
    {
        EventArea = null;
    }

    public virtual void Awake()
    {

    }
    
    public EventType GetEventType()
    {
        return Eventtype;
    }

}
