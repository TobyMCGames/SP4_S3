using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviour
{
    protected PhotonView photonView;

    protected float firerate = 0f;
    //number of times trigger pulled
    protected int numShots = 0;
    protected int maxNumShot = 0;
    protected int MaxAmmo = 0;
    protected int MaxMag = 0;
    public bool canShoot = false;
    protected float FirerateTimer;
    protected float waitToShoot = 0f;
    protected float waitToShootTimer = 0f;
    private GameObject enemyPickUp = null;

    public GameObject ProjectilePrefab = null;
    public GameObject ProjectileList = null;

    protected virtual void Start()
    {
        photonView = gameObject.GetPhotonView();
        //init variables (can be overriden)
        numShots = 0;
        firerate = 1f;
        MaxAmmo = 100;
        MaxMag = 8;
        maxNumShot = 3;
        waitToShoot = 2f;
        FirerateTimer = 0f;
        waitToShootTimer = 0f;
    }

    public void SetEnemyPickUp(GameObject enemy)
    {
        if (enemyPickUp == null)
            enemyPickUp = enemy;
        else
            Debug.Log("Enemy to pickup weapon already set to this enemy: " + enemyPickUp);
    }

    public GameObject GetEnemyPickup()
    {
        return enemyPickUp;
    }

    protected virtual void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (canShoot)
        {
            Shoot();
        }
    }

    protected virtual void Shoot()
    {

        if(numShots <= maxNumShot)
        {
            //Debug.Log("Shooting");
            FirerateTimer += Time.deltaTime;
            if (FirerateTimer >= firerate)
            {
                SpawnProjectile();
                FirerateTimer = 0;
            }
        }
        else
        {
            //wait a few seconds before shooting
            //Debug.Log("Pause shooting");
            waitToShootTimer += Time.deltaTime;
            if(waitToShootTimer >= waitToShoot)
            {
                waitToShootTimer = 0;
                numShots = 0;
            }
        }
    }

    protected void SpawnProjectile()
    {
        if (ProjectilePrefab == null)
        {
            Debug.Log("PoopPrefab was not assigned");
            return;
        }
        if (ProjectileList == null)
        {
            Debug.Log("ProjectileList was not assigned");
            return;
        }

        //spawn projectile
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, ProjectileList.transform);
            projectile.transform.position = transform.GetChild(0).position;
            projectile.transform.rotation = transform.GetChild(0).rotation;
            projectile.GetComponent<Projectile_Bullet>().SetShootDir(projectile.transform.forward);
        }
        else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            GameObject projectile = PhotonNetwork.Instantiate(ProjectilePrefab.name, transform.GetChild(0).position, transform.GetChild(0).rotation);
            projectile.GetComponent<Projectile_Bullet>().SetShootDir(projectile.transform.forward);
            photonView.RPC("LoadBulletProperties", RpcTarget.AllViaServer, projectile.GetPhotonView().ViewID);
        }
            numShots += 1;
    }

    // For multiplayer only
    [PunRPC]
    public void LoadBulletProperties(int _bulletID)
    {
        PhotonView bullet = PhotonNetwork.GetPhotonView(_bulletID);
        if (bullet == null)
            return;
        bullet.transform.SetParent(ProjectileList.transform);
    }
}
