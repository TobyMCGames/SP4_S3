using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile_Poop : MonoBehaviour
{
    private PhotonView photonView;

    private Vector3 direction;
    private float Velocity;
    // Use Players PhotonView Instead in Multiplayer (maybe) or use GO instead
    Transform pooper = null;

    private void Awake()
    {
        photonView = gameObject.GetPhotonView();
    }

    void OnTriggerEnter(Collider other)
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (other.CompareTag("Projectile") || other.CompareTag("Non-collidable"))
            return;

        if (pooper != null)
        {
            if (other.transform == pooper || other.transform.parent == pooper)
                return;
        }
        else
            Debug.Log("No pooper assigned");

        if (other.CompareTag("Enemy"))
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                PhotonView _enemy = other.gameObject.GetPhotonView();
                photonView.RPC("SetStunRPC", RpcTarget.MasterClient, _enemy.ViewID, 5.0f);
            }
            else
                other.transform.GetComponent<Enemy>().SetStun(5.0f);
        }
        Debug.Log("Poop drop on non-enemy");

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignPooper(Transform _pooper)
    {
        pooper = _pooper;
    }

    [PunRPC]
    private void SetStunRPC(int viewID, float stunTime)
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!PhotonNetwork.IsMasterClient)
                return;

        GameObject _enemy = PhotonNetwork.GetPhotonView(viewID).gameObject;
        _enemy.GetComponent<Enemy>().SetStun(stunTime);
    }
}
