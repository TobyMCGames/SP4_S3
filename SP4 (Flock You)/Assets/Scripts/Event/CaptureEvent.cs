using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CaptureEvent : BaseEvent
{
    // Start is called before the first frame update
    public float CaptureTimelimit;

    Dictionary<PhotonView, float> Pointlist = new Dictionary<PhotonView, float>(); // keep track of who has the most points 


    public override void Update()
    {
        
    }

    public override void Start()
    {
    }

    public override void Awake()
    {

    }
}
