using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class EventManager : MonoBehaviourPun
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
                photonView.RPC("RemoveFromList", RpcTarget.Others, i);
                ActiveEventList.RemoveAt(i);
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.Destroy(eventobj);
            }
        }
    }

    [PunRPC]
    public void RemoveFromList(int index)
    {
        ActiveEventList[index].GetComponent<BaseEvent>().returnEventobj();
        ActiveEventList.RemoveAt(index);
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
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER && PhotonNetwork.IsMasterClient)
                {
                    GameObject CaptureEvent = PhotonNetwork.Instantiate("CaptureEventPrefab", gameObject.transform.position, Quaternion.identity);
                    CaptureEvent.transform.localPosition = position;
                    photonView.RPC("SyncEventPosition", RpcTarget.AllViaServer,CaptureEvent.GetComponent<PhotonView>().ViewID, position);
                    photonView.RPC("InitEvent", RpcTarget.AllViaServer, CaptureEvent.GetComponent<PhotonView>().ViewID, eventType);
                }
                break;
            case BaseEvent.GameEventType.EVENT_FETCH:
                GameObject FetchEvent;
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    FetchEvent = PhotonNetwork.Instantiate("FetchEvent", position, Quaternion.identity);
                    photonView.RPC("SyncEventPosition", RpcTarget.AllViaServer, FetchEvent.GetComponent<PhotonView>().ViewID, position);
                    photonView.RPC("InitEvent", RpcTarget.AllViaServer, FetchEvent.GetComponent<PhotonView>().ViewID, eventType);
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                {
                    FetchEvent = Instantiate(EventfetchPrefab, gameObject.transform);
                    FetchEvent.transform.localPosition = position;
                    ActiveEventList.Add(FetchEvent);
                    //AddEvent(FetchEvent);
                }
                break;
        }
    }
    [PunRPC]
    public void InitEvent(int viewID,BaseEvent.GameEventType eventType)
    {
        GameObject obj = PhotonNetwork.GetPhotonView(viewID).gameObject;
        switch(eventType)
        {
            case BaseEvent.GameEventType.EVENT_CAPTURE:
                obj.transform.SetParent(gameObject.transform);
                obj.GetComponent<CaptureEvent>().Init();
                break;
            case BaseEvent.GameEventType.EVENT_FETCH:
                obj.transform.SetParent(gameObject.transform);
                //obj.GetComponent<FetchEvent>().Init(); // currently has nothing to init
                break;
        }
    }

    [PunRPC]
    public void SyncEventPosition(int viewID,Vector3 position)
    {
        PhotonNetwork.GetPhotonView(viewID).gameObject.transform.localPosition = position;
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
}
