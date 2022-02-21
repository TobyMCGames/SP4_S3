using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;
    //List<BaseEvent> ActiveEventList = new List<BaseEvent>();
    Dictionary<PhotonView,GameObject> ActiveEventList = new Dictionary<PhotonView,GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
           
    }

    public void RemoveEvent(PhotonView Eventview)
    {
        ActiveEventList.Remove(Eventview);
    }

    public void AddEvent(PhotonView EventView, GameObject baseEventobj)
    {
        ActiveEventList.Add(EventView,baseEventobj);
    }

    public GameObject getEventobj(PhotonView EventView)
    {
        GameObject returnevent;
        ActiveEventList.TryGetValue(EventView,out returnevent);
        return returnevent;

    }

    public void UpdateEventobj(PhotonView EventView ,GameObject _Event)
    {
        ActiveEventList[EventView] = _Event;
    }

}
