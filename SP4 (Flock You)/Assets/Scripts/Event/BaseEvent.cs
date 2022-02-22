using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BaseEvent :MonoBehaviour
{
    public enum GameEventType
    {
        EVENT_FETCH,
        EVENT_CAPTURE,
        NUM_EVENTS,
    }
    [SerializeField] protected GameEventType Eventtype;
    [SerializeField] protected Collider EventArea;
    [SerializeField] protected List<PhotonView> EventTarget;
    [SerializeField] protected List<Transform> TargetTransform;
    [SerializeField] protected GameObject canvas;
    protected bool Completed;
    protected double Destructiontimer; 
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

    public virtual void Init()
    {

    }

    public virtual void Update()
    {
        if (Completed)
        {
            Destructiontimer -= Time.deltaTime;
        }
    }

    public virtual void Start()
    {
        EventArea = null;
    }

    public virtual void Awake()
    {

    }
    
    public GameEventType GetEventType()
    {
        return Eventtype;
    }

    public bool IsCompleted()
    {
        return Completed;
    }

    public void SetCompleted(bool _Completed)
    {
        Completed = _Completed;
    }

    public virtual void returnEventobj()
    {
    }
    

    public virtual void ToggleDestruction()
    {
        Destructiontimer = 0;
    }
    
    public bool IsTobeDestroyed()
    {
        if (Completed && Destructiontimer <= 0)
            return true;
        else
            return false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        
    }

    public virtual void OnTriggerExit(Collider other)
    {
        
    }
}
