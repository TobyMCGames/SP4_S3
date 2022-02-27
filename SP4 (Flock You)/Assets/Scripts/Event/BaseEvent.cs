using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BaseEvent :MonoBehaviourPun,IPunObservable
{
    public enum GameEventType
    {
        EVENT_FETCH,
        EVENT_CAPTURE,
        EVENT_TIMETRIAL,
        NUM_EVENTS,
    }
    [SerializeField] protected GameEventType Eventtype;
    [SerializeField] protected Collider EventArea;
    [SerializeField] protected GameObject canvas;

    protected PhotonView EventView;
    protected bool Completed;
    protected double Destructiontimer;

    protected int pointsAwarded=0;
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
        EventView = GetComponent<PhotonView>();
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

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    /*public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(Destructiontimer);
                stream.SendNext(Completed);
                stream.SendNext(canvas);
                //stream.SendNext(EventArea);
            }
        }
        else if (stream.IsReading)
        {
            Destructiontimer = (float)stream.ReceiveNext();
            Completed = (bool)stream.ReceiveNext();
            canvas = (GameObject)stream.ReceiveNext();
            
        }
    }*/
}
