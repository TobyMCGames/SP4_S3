using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile_Bullet : MonoBehaviour
{
    PhotonView photonView;
    private Vector3 ShootDir;
    float moveSpeed = 50f;
    float tillDelete = 3f;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Projectile") || other.CompareTag("Non-collidable"))
            return;

        if (other.CompareTag("Player"))
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                if (other.GetComponent<PhotonView>().IsMine)
                    other.GetComponent<FYPlayer>().Damaged(1);
            }
            else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                other.GetComponent<FYPlayer>().Damaged(1);

            Debug.Log("Bullet Shot a player");
        }
        else if (other.CompareTag("Enemy"))
        {
            // Reduce health of Dog/Cat
            return;
        }
        else
            Debug.Log("Bullet Shot something");

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject);
    }

    public void SetShootDir(Vector3 shootDir)
    {
        ShootDir = shootDir;
    }

    private void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (tillDelete > 0)
            tillDelete -= Time.deltaTime;
        else
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                Destroy(gameObject);
            else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
                PhotonNetwork.Destroy(gameObject);
        }    
        transform.position += ShootDir * moveSpeed * Time.deltaTime;
    }
}
