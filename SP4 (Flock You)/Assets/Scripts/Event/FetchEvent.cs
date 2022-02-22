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
    public GameObject canvas;
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
                    TextCarriedby.text = "Playername is carrying the item";
                    TextDistanceCheck.text = "Nearest Nest: " + Distance.ToString() + "m";
                    FetchItem.GetComponent<FetchItemScript>().stoprender(true);
                }
            }
            else if (Completed)
            {
                TextCarriedby.text = "Playername delivered the item";
                TextDistanceCheck.text = "";
            }
        }
        else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            
        }
    }

    public void Init()
    {
        if (IsfetchGlobal && GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            foreach (Transform player in FYGameManager.Instance.PlayerList)
            {
                EventTarget.Add(player.GetComponent<PhotonView>());
            }
        }
        else // target the main player 
        {  
            // set the target to the main player in single player mode.
            //TargetTransform.Add(FYGameManager.Instance.player.gameObject.transform);
        }
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

    public override void returnText()
    {
        TextDistanceCheck.transform.parent = canvas.transform;
        TextCarriedby.transform.parent = canvas.transform;
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
                Distance = getNearestNest();
            }
        }
        else
        {
            foreach (Transform t in FYGameManager.Instance.PlayerList)
            {

            }
        }
    }
    
    public float getNearestNest()
    {
        // if not multiplayer dont need check whose nest isit 
        float nearestNest = -1;
        float currdistance;
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
        {
            currdistance = -1;
            foreach (Transform nest in FYGameManager.Instance.SpawnPoints)
            {
                currdistance = Vector3.Distance(nest.position, FYGameManager.Instance.PlayerList.GetChild(0).position);
                if (nearestNest == -1)
                    nearestNest = currdistance;
                else
                    if (currdistance < nearestNest)
                        nearestNest = currdistance;
            }
        }
        if (nearestNest == -1)
            Debug.Log("No nests foudn in the game manager");
        return Mathf.Round(nearestNest);
    }
}
