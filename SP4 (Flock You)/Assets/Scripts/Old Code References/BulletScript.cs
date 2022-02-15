using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class BulletScript : MonoBehaviour
{
    private PhotonView photonView;
    private Vector3 Velocity = Vector3.zero;
    private float Speed = 0f;
    private GameObject prop;
    private PhotonView source;
    private bool Activated = false;

    private float lifespan = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        prop = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        prop = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Activated || !photonView.IsMine)
            return;
        gameObject.transform.position += Velocity * Time.deltaTime;
        lifespan -= Time.deltaTime;

        if (prop != null)
        {
            if (prop.CompareTag("Player"))
            {
                if (prop.GetComponent<PhotonView>().ViewID != source.ViewID)
                {
                    Activated = false;
                    Debug.Log("Deleting by hitting a player");
                    photonView.RPC("DamageEntity", RpcTarget.AllViaServer, prop.GetComponent<PhotonView>().ViewID);
                }
            }
            else if (prop.CompareTag("Prop"))
            {
                Activated = false;
                Debug.Log("Deleting by hitting a prop");
                if (prop.GetComponentInParent<PlayerInfo>() != null)
                {
                    Debug.Log("Damage entity");
                    photonView.RPC("DamageEntity", RpcTarget.AllViaServer, prop.GetComponent<PhotonView>().ViewID);
                }
                else
                {
                    Debug.Log("Damage Self");
                    photonView.RPC("DamageEntity", RpcTarget.AllViaServer, source.ViewID);
                    Debug.Log("What is going on?");
                }
            }
        }

        if (lifespan < 0)
        {
            Activated = false;
            Debug.Log("Deleting bullet by time");
            photonView.RPC("deleteBullet", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void SetCreatedBullet(float directionX, float directionY, float speed, int sourceID)
    {
        Vector3 direction = Vector3.forward;
        direction = Quaternion.AngleAxis(directionX, Vector3.right) * direction;
        direction = Quaternion.AngleAxis(directionY, Vector3.up) * direction;
        Velocity = direction * speed;
        source = PhotonNetwork.GetPhotonView(sourceID);
        Activated = true;
    }

    [PunRPC]
    public void DamageEntity(int ID)
    {
        PhotonView entity = PhotonNetwork.GetPhotonView(ID);
        if (entity.GetComponent<PlayerInfo>())
        {
            PlayerInfo playerInfo = entity.GetComponent<PlayerInfo>();
            playerInfo.AffectHealth(-5);
        }
        else if (entity.GetComponentInParent<PlayerInfo>())
        {
            PlayerInfo playerInfo = entity.GetComponentInParent<PlayerInfo>();
            playerInfo.AffectHealth(-5);
        }
        Destroy(this.gameObject);
    }

    [PunRPC]
    public void deleteBullet()
    {
        Destroy(this);
    }
}
