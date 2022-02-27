using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class FetchEvent : BaseEvent
{
    // Start is called before the first frame update

    public bool IsfetchGlobal = false;  
    public GameObject FetchItem = null;
    public PhotonView carriedby = null;
    public Transform SingleCarriedby = null;
    public Transform PreviousCarriedby = null;
    public float Distance;
    public float timebeforeDestruction =5f;
    // Event Information
    public Text TextDistanceCheck;
    public Text TextCarriedby;

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
        EventManager.Instance.ActiveEventList.Add(gameObject);

        EventManager.Instance.TextToDisplay(TextDistanceCheck);
        EventManager.Instance.TextToDisplay(TextCarriedby);
        pointsAwarded = 50;
    }

    public override void Awake()
    {
        Eventtype = GameEventType.EVENT_FETCH;
    }

    public override void Update()
    {
        base.Update();
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
        {
            if (!Completed)
            {

                if (SingleCarriedby == null)
                {
                    calculateDistance(true);
                    TextDistanceCheck.text = "Distance: " + Distance.ToString() + "m";
                    TextCarriedby.text = "Pick up the Item and Store it in your Nest to gain points!";
                    FetchItem.GetComponent<FetchItemScript>().stoprender(false);
                }
                else
                {
                    calculateDistance(false);
                    TextCarriedby.text = "";
                    TextDistanceCheck.text = "Nearest Nest: " + Distance.ToString() + "m";
                    FetchItem.GetComponent<FetchItemScript>().stoprender(true);
                }
            }
            else if (Completed)
            {
                TextCarriedby.text = "you have successfully delivered the item";
                TextDistanceCheck.text = "";
                SingleCarriedby.gameObject.GetComponent<FYPlayer>().playerGameInfo.iPoints += pointsAwarded;
                UI_Manager.Instance.UpdatePlayerScore(SingleCarriedby.transform.GetComponent<FYPlayer>().playerGameInfo.sUsername,
                    SingleCarriedby.transform.GetComponent<FYPlayer>().playerGameInfo.iPoints, 0);
            }
        }
        else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            if (!Completed)
            {
                if (carriedby == null)
                {
                    calculateDistance(true);
                    TextDistanceCheck.text = "Distance: " + Distance.ToString() + "m";
                    TextCarriedby.text = "Pick up the Item and Store it in your Nest to gain points!";
                    FetchItem.GetComponent<FetchItemScript>().stoprender(false);
                }
                else
                {
                    calculateDistance(false);
                    TextCarriedby.text = carriedby.gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername.ToString() + " is carrying the item";
                    
                    if (carriedby.IsMine)
                        TextDistanceCheck.text = "Nearest Nest: " + Distance.ToString() + "m";
                    else
                        TextDistanceCheck.text = carriedby.gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername.ToString() + "Is:" +Distance.ToString() + " awway catch him!";
                    FetchItem.GetComponent<FetchItemScript>().stoprender(true);
                }
            }
            else if (Completed)
            {
                TextCarriedby.text = carriedby.gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername.ToString() + " has delivered the item";
                TextDistanceCheck.text = "";
                UI_Manager.Instance.UpdatePlayerScore(carriedby.gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername,
                    carriedby.gameObject.GetComponent<FYPlayer>().playerGameInfo.iPoints, 0);
            }
        }
    }
    [PunRPC]
    public void CompleteEvent()
    {
        Completed = true;
        ToggleDestruction();
        FetchItem.SetActive(false);
    }
    
    public override void Init()
    {
        /*if (IsfetchGlobal && GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            foreach (Transform player in FYGameManager.Instance.PlayerList)
            {
                //EventTarget.Add(player.GetComponent<PhotonView>());
            }
        }
        else // target the main player 
        {  
            // set the target to the main player in single player mode.
            //TargetTransform.Add(FYGameManager.Instance.player.gameObject.transform);
        }*/
    }

    public void setCarriedby(PhotonView _view)
    {
        carriedby = _view;
    }

    public void setSingleCarriedby(Transform holder)
    {
        PreviousCarriedby = SingleCarriedby;
        SingleCarriedby = holder;
    }

    public override void returnEventobj()
    {
        if (canvas != null)
        {
            TextDistanceCheck.transform.SetParent(canvas.transform);
            TextCarriedby.transform.SetParent(canvas.transform);
        }
        else
            Debug.Log("Canvas is null in eventbase");
    }

    public override void ToggleDestruction()
    {
        Destructiontimer = timebeforeDestruction;
    }


    public void calculateDistance(bool Item)
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
        {
            if (Item)
            {
                Distance = Vector3.Distance(FetchItem.transform.position, FYGameManager.Instance.PlayerList.GetChild(0).position);
                Distance = Mathf.Round(Distance);
            }
            else // search for the nearest nest to the player 
            {
                Distance = getNearestNest(FYGameManager.Instance.PlayerList.GetChild(0).position);
            }
        }
        else
        {
            foreach (Transform t in FYGameManager.Instance.PlayerList.transform)
            {
                if (t.gameObject.GetComponent<PhotonView>().IsMine)
                {

                    if (Item)
                    {
                        Distance = Vector3.Distance(FetchItem.transform.position,t.position);
                        Distance = Mathf.Round(Distance);
                    }
                    else if (!Item) // search for the nearest nest to the player 
                    {
                        if (carriedby.IsMine)
                            Distance = getNearestNest(t.position);
                        else
                        {
                            Distance = Vector3.Distance(carriedby.gameObject.transform.position,t.gameObject.transform.position);
                            Distance = Mathf.Round(Distance);
                        }
                    }
                }
            }
        }
    }
    
    public float getNearestNest(Vector3 playerpos)
    {
        // if not multiplayer dont need check whose nest isit 
        float nearestNest = -1;
        float currdistance;
        currdistance = -1;
        foreach (Transform nest in FYGameManager.Instance.SpawnPoints)
        {
            currdistance = Vector3.Distance(nest.position,playerpos);
            if (nearestNest == -1)
                nearestNest = currdistance;
            else
                if (currdistance < nearestNest)
                    nearestNest = currdistance;
        }
        if (nearestNest == -1)
            Debug.Log("No nests found in the game manager");
        return Mathf.Round(nearestNest);
    }
    public PhotonView getView()
    {
        return photonView;
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (GlobalSettings.curMode != GlobalSettings.GAMEMODES.MULTIPLAYER)
            return;
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(Completed);
                stream.SendNext(Destructiontimer);
                if (carriedby != null)
                    stream.SendNext(carriedby.ViewID);
            }
        }
        else if (stream.IsReading)
        {
            Completed = (bool)stream.ReceiveNext();
            Destructiontimer = (double)stream.ReceiveNext();
            int carriedview = -1;
            carriedview = (int)stream.ReceiveNext();
            if (carriedview != -1)
                carriedby = PhotonNetwork.GetPhotonView(carriedview);
        }
    }
}
