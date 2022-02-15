using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerIndicatorColor : MonoBehaviour
{
    private PhotonView photonView;
    // Start is called before the first frame update
    void Awake()
    {
        photonView = transform.parent.GetComponent<PhotonView>();
    }

    private void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = JLGame.GetColor(photonView.Owner.GetPlayerNumber());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
