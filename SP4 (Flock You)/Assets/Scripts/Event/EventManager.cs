using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class EventManager : MonoBehaviourPun,IPunObservable
{
    public static EventManager Instance = null;
    public List<GameObject> ActiveEventList = new List<GameObject>();

    public GameObject EventfetchPrefab;
    public GameObject EventcapturePrefab;
    public GameObject EventText;

    public PhotonView EventmanagerView;

    private void Awake()
    {
        Instance = this;
        EventmanagerView = GetComponent<PhotonView>();
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
                ActiveEventList[i].GetComponent<BaseEvent>().returnEventobj();
                ActiveEventList.RemoveAt(i);
                Destroy(eventobj);
            }
        }
    }

    public void RemoveEvent(GameObject _object)
    {
        ActiveEventList.Remove(_object);
    }
    
    [PunRPC]
    public void CreateEvent(BaseEvent.GameEventType eventType,Vector3 position)
    {
        switch (eventType)
        {
            case BaseEvent.GameEventType.EVENT_CAPTURE:
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    GameObject CaptureEvent = PhotonNetwork.Instantiate("CaptureEventPrefab", gameObject.transform.position, Quaternion.identity);
                    CaptureEvent.transform.SetParent(gameObject.transform);

                    CaptureEvent.transform.localPosition = position;
                    CaptureEvent.GetComponent<CaptureEvent>().Init();
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                {
                    
                    //AddEvent(FetchEvent);
                }
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

    public void ObjectToDisplay(GameObject _object)
    {
        _object.transform.SetParent(EventText.transform);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                
                GameObject[] objarr = null;
                for (int i =0;i<ActiveEventList.Count;i++)
                {
                    objarr[i] = ActiveEventList[i];
                }
                stream.SendNext(objarr);
            }
        }
        else if (stream.IsReading)
        {
            ActiveEventList.Clear();
            GameObject[] receivedobj = (GameObject[])stream.ReceiveNext();
            if (receivedobj != null)
            {
                for (int i =0;i < receivedobj.Length;i++)
                {
                    ActiveEventList.Add(receivedobj[i]);
                }
            }

        }
    }
}
