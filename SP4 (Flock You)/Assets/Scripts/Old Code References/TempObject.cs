using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class TempObject : MonoBehaviour
{
    private bool Activated = false;
    private bool Infinite = false;
    private float CurrentTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Activated)
        {
            if (CurrentTimer > 0)
            {
                if (!Infinite)
                    CurrentTimer -= Time.deltaTime;
            }
            else
                Destroy(this);
        }
    }

    public void SetObjectTimer(float timer, bool infinite)
    {
        Activated = true;
        CurrentTimer = timer;
        Infinite = infinite;
    }
}
