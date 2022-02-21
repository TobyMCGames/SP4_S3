using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    List<BaseEvent> ActiveEventList = new List<BaseEvent>();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void RemoveEvent(BaseEvent InvalidEvent)
    {
        ActiveEventList.Remove(InvalidEvent);
    }

}
