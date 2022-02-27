using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimeTrial : BaseEvent
{
    [SerializeField] GameObject Path; // put in the path with the gameobjects which will set the path based on the object's transform
    [SerializeField] List<GameObject> PointList;
    [SerializeField] float pointsAccumulated = 0f;
    [SerializeField] float completionBonus = 0f;
    [SerializeField] float TrialTime = 30f;
    [SerializeField] float CurrentTrialTime;

    bool TimeTrialStarted = false;
    int currentPoint = 0;
    TimeCheckpoint CurrentCheckpoint;

    public override void Awake()
    {   

    }

    public override void Start()
    {   
        foreach (Transform T in Path.transform)
        {
            PointList.Add(T.gameObject);
        }
        CurrentCheckpoint = PointList[0].GetComponent<TimeCheckpoint>();
        if (!CurrentCheckpoint)
            Debug.Log("There are no checkpoints in the time trial");
    }

    public override void Update()
    {
        if (CurrentCheckpoint.IsCollided())
        {
            if (currentPoint > 0 && currentPoint < PointList.Count-1)
            {
                CurrentTrialTime += CurrentCheckpoint.GetTimeExtension();
                pointsAccumulated += CurrentCheckpoint.GetPoints();
                currentPoint++;
                CurrentCheckpoint = PointList[currentPoint].GetComponent<TimeCheckpoint>();
            }
            else if (currentPoint == 0) // startpoint 
            {
                TimeTrialStarted = true;
            }
            else if (currentPoint == PointList.Count-1) // handle point giving
            {
                
            }

            currentPoint++;
        }
        if (TimeTrialStarted)
        {
            CurrentTrialTime -= Time.deltaTime;
        }
    }
    
    // check if the player collided with the point 
    public void CheckPoinHit()
    {

    }
    
}
