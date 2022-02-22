using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance = null;
    public List<GameObject> ActiveEventList = new List<GameObject>();

    public GameObject EventfetchPrefab;
    public GameObject EventcapturePrefab;
    public GameObject EventText;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        GameObject eventobj;
        for (int i = ActiveEventList.Count-1; i >= 0;i--)
        {
            eventobj = ActiveEventList[i];
            if (ActiveEventList[i].GetComponent<BaseEvent>().IsTobeDestroyed())
            {
                ActiveEventList[i].GetComponent<BaseEvent>().returnText();
                ActiveEventList.RemoveAt(i);
                Destroy(eventobj);
            }
        }
    }

    public void RemoveEvent(GameObject _object)
    {
        ActiveEventList.Remove(_object);
    }

    public void CreateEvent(BaseEvent.GameEventType eventType,Vector3 position)
    {
        switch (eventType)
        {
            case BaseEvent.GameEventType.EVENT_CAPTURE:
                break;
            case BaseEvent.GameEventType.EVENT_FETCH:
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                {
                    GameObject FetchEvent = Instantiate(EventfetchPrefab, gameObject.transform);
                    FetchEvent.transform.localPosition = position;
                    //AddEvent(FetchEvent);
                }
                break;
        }
    }

    public void ToggleEventText(bool Toggle)
    {
        EventText.SetActive(Toggle);
    }

    public void TextToDisplay(Text _Text)
    {
        _Text.transform.SetParent(EventText.transform);
    }

    public GameObject GetEvent(BaseEvent.GameEventType _type)
    {
        for (int i =0;i < ActiveEventList.Count;i++)
        {
            if (ActiveEventList[i].GetComponent<BaseEvent>().GetEventType() == _type)
                return ActiveEventList[i];
        }
        return null;
    }
}
