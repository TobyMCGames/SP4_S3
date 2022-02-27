using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimeTrial : BaseEvent
{
    public GameObject Path; // put in the path with the gameobjects which will set the path based on the object's transform
    [SerializeField] int pointsAccumulated = 0;
    [SerializeField] int completionBonus = 20;
    [SerializeField] float TrialTime = 2f;
    [SerializeField] float CurrentTrialTime;
    GameObject TargetPlayer  = null;
    List<GameObject> pathways = new List<GameObject>();
    bool TimeTrialStarted = false;
    int currentPoint = 0;

    public override void Start()
    {
        CurrentTrialTime = TrialTime;
        Initcheckpoints();
    }

    public override void Update()
    {
        if (FYGameManager.Instance.isOver())
            return;
        if (pathways[currentPoint].GetComponent<TimeCheckpoint>().isCollided())
        {
            if (currentPoint > 0 && currentPoint < Path.transform.childCount-1)
            {
                CurrentTrialTime += pathways[currentPoint].GetComponent<TimeCheckpoint>().GetTimeExtension();
                pointsAccumulated += pathways[currentPoint].GetComponent<TimeCheckpoint>().GetPoints();
                pathways[currentPoint].SetActive(false);
                currentPoint++;
                pathways[currentPoint].SetActive(true);
            }
            else if (currentPoint == 0) // startpoint 
            {
                TimeTrialStarted = true;
                pathways[currentPoint].SetActive(false);
                currentPoint++;
                pathways[currentPoint].SetActive(true);
            }
            else if (currentPoint == Path.transform.childCount-1) // handle point giving
            {
                pathways[currentPoint].SetActive(false);
                TargetPlayer.GetComponent<FYPlayer>().playerGameInfo.iPoints += (pointsAccumulated + completionBonus);
                if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
                {
                    UI_Manager.Instance.photonView.RPC("UpdatePlayerScore", RpcTarget.AllViaServer,
                    TargetPlayer.GetComponent<FYPlayer>().playerGameInfo.sUsername, TargetPlayer.GetComponent<FYPlayer>().playerGameInfo.iPoints,
                    0);
                }
                else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                {
                    UI_Manager.Instance.UpdatePlayerScore(TargetPlayer.GetComponent<FYPlayer>().playerGameInfo.sUsername, TargetPlayer.GetComponent<FYPlayer>().playerGameInfo.iPoints,0);
                }
                
                gameObject.SetActive(false);
            }
        }
        if (TimeTrialStarted)
        {
            CurrentTrialTime -= Time.deltaTime;
            if (CurrentTrialTime  <=0)
            {
                CurrentTrialTime = TrialTime;
                TimeTrialStarted = false;
                Initcheckpoints();
                currentPoint = 0;

            }
        }
    } 
    
    void Initcheckpoints()
    {
        pathways.Clear();
        foreach (Transform t in Path.transform)
        {
            pathways.Add(t.gameObject);
        }
        pathways[0].SetActive(true);
        for (int i = 1; i < pathways.Count; i++)
        {
            pathways[i].SetActive(false);
        }
    }

    public void initTargetPlayer(GameObject _targetObject)
    {
        TargetPlayer = _targetObject;
    }
}
