using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FYPlayer : MonoBehaviour
{
    private PhotonView photonView;

    // Default SpawnPoint
    private Vector3 DefaultSpawnPoint = new Vector3(0.0f, 0.0f, 0.0f);

    // Health & Lives
    public int MaxHealth = 5;
    public int MaxLives = 3;
    public int curHealth;
    public int curLives;
    public float InvincibilityDuration;
    public float InvincibilityTimer;
    public float BlinkFrame;
    public float BlinkTimer;

    // Player scripts
    public DropLoadControl DropLoadControl_script;
    public PlayerGameValues playerGameInfo;

    //LayEggAbility layEggAbility = new LayEggAbility();
    LayEggAbility layEggAbility;

    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init()
    {
        curHealth = MaxHealth;
        curLives = MaxLives;
        layEggAbility = GetComponent<AbilityControl>().GetLayEggAbility();
        DropLoadControl_script.Init();
        photonView = gameObject.GetPhotonView();
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (isDead())
            return;

        if (!hasHealth())
        {
            --curLives;
            curHealth = MaxHealth;
            ResetSpawnPoint();
            UI_Manager.Instance.UpdateHealth();
            UI_Manager.Instance.UpdateLives();
        }

        if (InvincibilityTimer > 0)
        {
            InvincibilityTimer -= Time.deltaTime;
            if (BlinkTimer > 0)
            {
                BlinkTimer -= Time.deltaTime;
                if (BlinkTimer <= 0)
                {
                    GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
                    BlinkTimer = BlinkFrame;
                }
            }

            if (InvincibilityTimer <= 0)
                GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public bool hasHealth()
    {
        return curHealth > 0;
    }

    public bool isDead()
    {
        return curLives <= 0 && !hasHealth();
    }

    public void setDefaultSpawn(Vector3 _spawnpoint)
    {
        DefaultSpawnPoint = _spawnpoint;
    }

    public void ResetSpawnPoint()
    {
        GameObject NestPrefab = Resources.Load("NestSpawn") as GameObject;
        if (layEggAbility.GetNest() != null)
        {
            gameObject.transform.position = layEggAbility.GetNest().transform.position;
            Destroy(layEggAbility.GetNest());
        }
        else 
            gameObject.transform.position = DefaultSpawnPoint;
    }

    [PunRPC]
    public void Damaged(int _damage)
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;

        if (InvincibilityTimer > 0)
            return;

        curHealth -= _damage;
        UI_Manager.Instance.UpdateHealth();

        if (curLives <= 0)
            return;

        InvincibilityTimer = InvincibilityDuration;
        BlinkTimer = BlinkFrame;
    }
}