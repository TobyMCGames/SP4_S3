using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CaptureEvent : BaseEvent
{
    // Start is called before the first frame update

    // UI
    public GameObject progressbarPrefab = null;
    public GameObject progressbarlist = null;
    public Text Leadingplayer;

    public float CaptureTimelimit;
    // viewid: points
    private Dictionary<int,Progressbar > playerProgresslist = new Dictionary<int,Progressbar>(); // keep track of who has the most points 

    private Dictionary<int,float> withinBoundary = new Dictionary<int, float>();

    public float PointsTimer = 0.5f;
    public float ElapsedTime = 0f;
    public float rateofCapture = 20f;
    public float maxaddedprogress = 2f;
    public float progressbaryOffset = 50;


    public override void Init()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            PhotonView playerview;
            GameObject progressbar;
            Vector3 progressbarposition = Vector3.zero;
            int playercount = 0;
            foreach (Transform player in FYGameManager.Instance.PlayerList)
            {
                if (playerview = player.gameObject.GetComponent<PhotonView>())
                {
                    progressbar = Instantiate(progressbarPrefab,progressbarlist.transform);
                    progressbarposition.x = canvas.GetComponent<RectTransform>().rect.width * (0.9f);
                    progressbarposition.y = canvas.GetComponent<RectTransform>().rect.height * (0.1f) + (playercount * progressbaryOffset);
                    progressbar.transform.localPosition = progressbarposition;
                    playerProgresslist.Add(playerview.ViewID,progressbar.GetComponent<Progressbar>());
                    EventManager.Instance.ObjectToDisplay(progressbarlist);
                }
                else
                {
                    Debug.Log("CaptureEvent: Player does not have a photon view");
                }
            }
        }
    }

    public override void Update()
    {
        if (withinBoundary.Count > 0) // add points to the player 
        {
            if (ElapsedTime >= PointsTimer)
            {
                float addedprogress = 0;
                foreach (KeyValuePair<int,float> within  in withinBoundary)
                {
                    addedprogress = (rateofCapture * within.Key) * Time.deltaTime;
                   
                    if (addedprogress > maxaddedprogress)
                        addedprogress = maxaddedprogress;
                    playerProgresslist[within.Key].addProgress(within.Value + addedprogress);
                    ElapsedTime = 0;
                }
            }
            else
            {
                ElapsedTime += Time.deltaTime;
                
            }
        }
        else
        {
            ElapsedTime = 0f;
        }
    }

    public override void Start()
    {

    }

    public override void Awake()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                PhotonView playerview = other.gameObject.GetComponent<PhotonView>();
                if (playerview == null)
                    Debug.Log("multiplayer CaptureEvent: Colliding Player does not have a photonview");
                else if (!withinBoundary.ContainsKey(playerview.ViewID))
                {
                    withinBoundary.Add(playerview.ViewID, 0f);
                }
            }
        }
    }

    
    public override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                PhotonView playerview = other.gameObject.GetComponent<PhotonView>();
                if (playerview == null)
                    Debug.Log("multiplayer CaptureEvent: Colliding Player does not have a photonview");
                else if (withinBoundary.ContainsKey(playerview.ViewID))
                {
                    withinBoundary.Remove(playerview.ViewID);
                }
            }
        }    
    }

    public override void returnEventobj()
    {
        if (canvas.transform != null)
        {
            progressbarlist.transform.SetParent(canvas.transform);
        }
    }
}
