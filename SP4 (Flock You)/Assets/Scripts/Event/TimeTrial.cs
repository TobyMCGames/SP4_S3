using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimeTrial : BaseEvent
{
    public GameObject Path; // put in the path with the gameobjects which will set the path based on the object's transform
    [SerializeField] int pointsAccumulated = 0;
    [SerializeField] float completionBonus = 0f;
    [SerializeField] float TrialTime = 30f;
    [SerializeField] float CurrentTrialTime;
    GameObject TargetPlayer  = null;
    List<GameObject> pathways = new List<GameObject>();
    bool TimeTrialStarted = false;
    int currentPoint = 0;
    GameObject CurrentCheckpoint;

    public override void Start()
    {
        if (!CurrentCheckpoint)
            Debug.Log("There are no checkpoints in the time trial");
        CurrentTrialTime = TrialTime;
        foreach (Transform t in Path.transform)
        {
            pathways.Add(t.gameObject);
        }   
        for (int i =1; i < pathways.Count;i++)
        {
            pathways[i].SetActive(false);
        }
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
                TargetPlayer.GetComponent<FYPlayer>().playerGameInfo.iPoints += pointsAccumulated;
                gameObject.SetActive(false);
            }
        }
        if (TimeTrialStarted)
        {
            CurrentTrialTime -= Time.deltaTime;
        }
    } 
    public void initTargetPlayer(GameObject _targetObject)
    {
        TargetPlayer = _targetObject;
    }
}
