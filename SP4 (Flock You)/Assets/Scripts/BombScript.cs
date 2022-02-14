using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class BombScript : MonoBehaviour
{

    public bool isMine = false;
    float time;

    public GameObject enemies;
    private Vector3 pos;
    private PhotonView photonView;

    // Start is called before the first frame update

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        time = 5.0f;
        enemies = GameObject.Find("Enemies");
    }

    [PunRPC]
    public void CallExplode()
    {
        // if(position == transform.position)
        // {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        time -= Time.deltaTime;
        if(time <= 0.0f)
        {
            string dataSent = ""+transform.position;

            photonView.RPC("CallExplode", RpcTarget.AllViaServer);
            pos = transform.position;
            BombEffects();
        }
    }

    public void BombEffects()
    {
        //Transform[] allChildren = enemies.GetComponentsInChildren<Transform>();
        foreach (Transform child in enemies.transform)
        {
            float distance = Vector3.Distance(child.position, pos);
            if (distance < 1)
            {
                child.gameObject.GetComponent<GhostRPC>().GhostBombed();
            }
        }
    }
}
