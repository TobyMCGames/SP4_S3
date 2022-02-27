using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class CaptureEvent : BaseEvent
{
    // Start is called before the first frame update

    // UI
    public GameObject progressbarPrefab = null;
    public GameObject[] progressbarlist;
    public Text Leadingplayer;

    public float CaptureTimelimit;
    // viewid: points
    
    [SerializeField]private Dictionary<int,Progressbar > playerProgresslist = new Dictionary<int,Progressbar>(); // keep track of who has the most points 
    [SerializeField]private List<int> viewwithinBoundary = new List<int>();
    [SerializeField]private List<float> timewithinBoundary = new List<float>();

    public float PointsTimer = 0.5f;
    public float ElapsedTime = 0f;
    public float rateofCapture = 0.5f;
    public float maxaddedprogress = 2f;
    public float progressbaryOffset = 50;

    public void SyncPoints(int Playerview,float currentprogress)
    {
        playerProgresslist[Playerview].setProgress(currentprogress);
    }

    public void SyncPlayerTime(int index,float time)
    {
        timewithinBoundary[index] = time;
    }

    public void Syncviewboundary(int index,int playerview)
    {
        viewwithinBoundary[index] = playerview;
    }

    public void AddviewBoundary(int playerview)
    {
        viewwithinBoundary.Add(playerview);
    }

    public void RemoveviewBoundary(int playerview)
    {
        if (viewwithinBoundary.Contains(playerview))
            viewwithinBoundary.Remove(playerview);
    }

    public void addTimeBoundary(float timeboundary)
    {
        timewithinBoundary.Add(timeboundary);
    }

    public void removeTimeBoundary(int index)
    {
        timewithinBoundary.RemoveAt(index);
    }

    // sync text
    public void SyncLedaingPlayer(string leadingtext)
    {
        Leadingplayer.text = leadingtext;
    }


    public override void Init()
    {
        pointsAwarded = 70;
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            PhotonView playerview;
            GameObject progressbar;
            Vector3 progressbarposition = Vector3.zero;
            int playercount = 0;
            string playername = "";
            foreach (Transform player in FYGameManager.Instance.PlayerList)
            {
                if (playercount > 4)
                    return;
                if (playerview = player.gameObject.GetComponent<PhotonView>())
                {
                    progressbar = progressbarlist[playercount];
                    playername = playerview.gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername;
                    if (playerview.IsMine)
                    {
                        playername += "(You)";
                    }
                    progressbar.GetComponent<Progressbar>().SetPlayerName(playername);
                    playerProgresslist.Add(playerview.ViewID,progressbar.GetComponent<Progressbar>());
                    progressbar.SetActive(true);
                    EventManager.Instance.ObjectToDisplay(progressbar);
                    EventManager.Instance.ActiveEventList.Add(gameObject);
                    EventManager.Instance.TextToDisplay(Leadingplayer);
                }
                else
                {
                    Debug.Log("CaptureEvent: Player does not have a photon view");
                }
                playercount++;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (viewwithinBoundary.Count >0) // add points to the player 
        {
            if (ElapsedTime >= PointsTimer)
            {
                float addedprogress = 0;
                int mostprogressViewID = -1;
                for (int i=0;i< viewwithinBoundary.Count;i++)
                {
                    addedprogress = (rateofCapture * timewithinBoundary[i]) *Time.deltaTime * 0.01f;
                    if (addedprogress > maxaddedprogress)
                        addedprogress = maxaddedprogress;
                    if (playerProgresslist.ContainsKey(viewwithinBoundary[i]))
                    {
                        playerProgresslist[viewwithinBoundary[i]].addProgress(addedprogress);
                        ElapsedTime = 0;
                        // sync with other clients
                        //photonView.RPC("SyncPoints", RpcTarget.OthersBuffered, viewwithinBoundary[i], playerProgresslist[viewwithinBoundary[i]].getProgress());

                        if (playerProgresslist[viewwithinBoundary[i]].getProgress() >= 1)
                            Completed = true;
                    }
                    if (i == 0)
                    {
                        mostprogressViewID= viewwithinBoundary[i];
                    }
                    else
                    {
                        // compare who has the higher progress
                        if (playerProgresslist[viewwithinBoundary[i]].getProgress() > playerProgresslist[mostprogressViewID].getProgress())
                        {
                            mostprogressViewID = viewwithinBoundary[i];
                        }
                    }
                }
                if (mostprogressViewID == -1) // no player has entered
                {
                    Leadingplayer.text = "Capture the point!";
                }
                else if (!Completed)
                {
                        Leadingplayer.text = PhotonNetwork.GetPhotonView(mostprogressViewID).gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername + " is: " +
                        Mathf.Round((playerProgresslist[mostprogressViewID].getProgress() * 100f)).ToString() + " % to capturing the point";
                }
                else if (Completed)
                {
                    Leadingplayer.text = PhotonNetwork.GetPhotonView(mostprogressViewID).gameObject.GetComponent<FYPlayer>().playerGameInfo.sUsername + " has captured the point";
                }
                //photonView.RPC("SyncLeadingPlayer", RpcTarget.OthersBuffered, Leadingplayer);
            }
            else
            {
                for (int i=0;i<viewwithinBoundary.Count;i++)
                {
                    timewithinBoundary[i] += Time.deltaTime;
                   // photonView.RPC("SyncPlayerTime", RpcTarget.Others,i, timewithinBoundary[i]);
                }
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
        Destructiontimer += 2f;
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                PhotonView playerview = other.gameObject.GetComponent<PhotonView>();
                if (playerview == null)
                    Debug.Log("multiplayer CaptureEvent: Colliding Player does not have a photonview");
                else if (!viewwithinBoundary.Contains(playerview.ViewID))
                {
                    viewwithinBoundary.Add(playerview.ViewID);
                    timewithinBoundary.Add(0f);
                }   
            }
        }
    }

    
    public override void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                PhotonView playerview = other.gameObject.GetComponent<PhotonView>();
                if (playerview == null)
                    Debug.Log("multiplayer CaptureEvent: Colliding Player does not have a photonview");
                else if (viewwithinBoundary.Contains(playerview.ViewID))
                {
                    int index = viewwithinBoundary.IndexOf(playerview.ViewID);
                    viewwithinBoundary.Remove(playerview.ViewID);
                    timewithinBoundary.RemoveAt(index);
                    //photonView.RPC("RemoveviewBoundary", RpcTarget.AllViaServer, playerview.ViewID);
                    //photonView.RPC("removeTimeBoundary", RpcTarget.AllViaServer, index);
                }
            }
        }    
    }

    public override void returnEventobj()
    {
        for (int i=0;i<progressbarlist.Length;i++)
        {
            progressbarlist[i].transform.SetParent(canvas.transform);
        }
        Leadingplayer.transform.SetParent(canvas.transform);
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                float[] prog = new float[playerProgresslist.Count];
                int[] view = new int[playerProgresslist.Count];
                
                int counter = 0;
                foreach (KeyValuePair<int, Progressbar> pair in playerProgresslist)
                {
                    prog[counter] = pair.Value.getProgress();
                    view[counter] = pair.Key;
                    counter++;
                }
                stream.SendNext(prog);
                stream.SendNext(view);
            }
            string LeadText = Leadingplayer.text;
            stream.SendNext(LeadText);
            stream.SendNext(Completed);
            stream.SendNext(Destructiontimer);
        }
        else if (stream.IsReading)
        {
            float[] prog = (float[])stream.ReceiveNext();
            int[] view = (int[])stream.ReceiveNext();
            for (int i=0;i< view.Length;i++)
            {
                playerProgresslist[view[i]].setProgress(prog[i]);
            }
            // receive the leading player text 
            Leadingplayer.text = (string)stream.ReceiveNext();
            Completed = (bool)stream.ReceiveNext();
            Destructiontimer = (double)stream.ReceiveNext();
        }
    }
}
