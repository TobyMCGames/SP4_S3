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
    private Dictionary<int,Progressbar > playerProgresslist = new Dictionary<int,Progressbar>(); // keep track of who has the most points 

    private List<int> viewwithinBoundary = new List<int>();
    private List<float> timewithinBoundary = new List<float>();

    public float PointsTimer = 0.5f;
    public float ElapsedTime = 0f;
    public float rateofCapture = 0.5f;
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
            string playername = "";
            foreach (Transform player in FYGameManager.Instance.PlayerList)
            {
                if (playercount > 4)
                    return;
                if (playerview = player.gameObject.GetComponent<PhotonView>())
                {
                    progressbar = progressbarlist[playercount];
                    playername = "get playername here yes";
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
                    playerProgresslist[viewwithinBoundary[i]].addProgress(addedprogress);
                    ElapsedTime = 0;
                    if (playerProgresslist[viewwithinBoundary[i]].getProgress() >= 1)
                        Completed = true;
                    if (i == 0)
                    {
                        mostprogressViewID= viewwithinBoundary[i];
                    }
                    else
                    {
                        // compare who has the higher progress
                        if (playerProgresslist[viewwithinBoundary[i]].getProgress() > playerProgresslist[viewwithinBoundary[mostprogressViewID]].getProgress())
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
                    Leadingplayer.text = "Playername" + "is: " +
                        Mathf.Round((playerProgresslist[mostprogressViewID].getProgress() * 100f)).ToString() + "% to capturing the point";
                }
                else if (Completed)
                {
                    Leadingplayer.text = "Playername: " + "has captured the point";
                }
            }
            else
            {
                for (int i=0;i<viewwithinBoundary.Count;i++)
                {
                    timewithinBoundary[i] += Time.deltaTime;
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
        base.OnPhotonSerializeView(stream, info);
        
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(progressbarlist);
                stream.SendNext(Leadingplayer);

                int[] arr = null;
                
                Progressbar[] progressarr = null;
                int counter = 0;
                foreach (KeyValuePair<int,Progressbar> playerprogress in playerProgresslist)
                {
                    arr[counter] = playerprogress.Key;
                    progressarr[counter] = playerprogress.Value;
                    counter++;
                }
                stream.SendNext(arr);
                stream.SendNext(progressarr);

                int[] viewwithinarr = null;
                float[] timewithinarr = null;
                for (int i =0;i < viewwithinBoundary.Count;i++)
                {
                    viewwithinarr[i] = viewwithinBoundary[i];
                }
                
                for (int j =0;j < timewithinBoundary.Count; j++)
                {
                    timewithinarr[j] = timewithinBoundary[j];
                }

                stream.SendNext(viewwithinarr);
                stream.SendNext(timewithinarr);
            }
        }
        else if (stream.IsReading)
        {
            Array.Clear(progressbarlist, 0, progressbarlist.Length);
            progressbarlist = (GameObject[])stream.ReceiveNext();
            Leadingplayer = (Text)stream.ReceiveNext();

            playerProgresslist.Clear();
            int[] arr = (int[])stream.ReceiveNext();
            Progressbar[] progressarr = (Progressbar[])stream.ReceiveNext();
            if (arr != null && progressarr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    playerProgresslist.Add(arr[i], progressarr[i]);
                }
            }
           

            int[] viewwithinarr = (int[])stream.ReceiveNext();

            float[] timewithinarr = (float[])stream.ReceiveNext();

            
            if (viewwithinarr != null)
            {
                viewwithinBoundary.Clear();
                for (int i =0;i< viewwithinarr.Length;i++)
                {
                    viewwithinBoundary.Add(viewwithinarr[i]);
                }
            }

            if (timewithinarr != null)
            {
                timewithinBoundary.Clear();
                for (int i =0;i< timewithinarr.Length;i++)
                {
                    timewithinBoundary.Add(timewithinarr[i]);
                }
            }
        }
    }
}
