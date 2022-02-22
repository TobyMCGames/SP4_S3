using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DropLoadControl : MonoBehaviour
{
    PhotonView photonView;

    // Projectile
    [Header("Projectile Essentials")]
    public GameObject PoopPrefab = null;
    public Transform ProjectileList = null;

    // Ammo variables
    [Header("Ammo Variables")]
    public int MaxAmmo = 5;
    public int curAmmo;

    // Reload Variables
    [Header("Reloading Variables")]
    public float MaxTillReload = 1.0f;
    private float tillReload;
    public float MaxRechargeRateTimer = 0.2f;
    private float RechargeRateTimer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.Log("PhotonView is not set in DropLoadControl.cs");
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        curAmmo = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;
        if (curAmmo < MaxAmmo)
        {
            UpdateReload();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CanDropLoad())
            {
                Debug.Log("Drop load");
                DropLoad();
            }
        }
    }

    void UpdateReload()
    {
        if (tillReload > 0) {
            tillReload -= Time.deltaTime;
            return;
        }

        if (RechargeRateTimer > 0) {
            RechargeRateTimer -= Time.deltaTime;
            return;
        }

        curAmmo++;
        RechargeRateTimer = MaxRechargeRateTimer;
        UI_Manager.Instance.UpdateAmmo();
    }

    bool CanDropLoad()
    {
        return curAmmo > 0;
    }

    void DropLoad()
    {
        if (PoopPrefab == null)
        {
            Debug.Log("PoopPrefab was not assigned");
            return;
        }
        if (ProjectileList == null)
        {
            Debug.Log("ProjectileList was not assigned");
            return;
        }

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
        {
            GameObject poop = Instantiate(PoopPrefab, ProjectileList);
            poop.transform.position = gameObject.transform.position;
            poop.GetComponent<Projectile_Poop>().AssignPooper(transform);
            poop.GetComponent<Rigidbody>().AddForce(gameObject.GetComponent<Rigidbody>().velocity);
        }
        else if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
        {
            GameObject poop = PhotonNetwork.Instantiate("Bird_Poop", gameObject.transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
            poop.GetComponent<Projectile_Poop>().AssignPooper(transform);
            photonView.RPC("LoadPoopProperties", RpcTarget.AllViaServer,
                            poop.GetPhotonView().ViewID,
                            gameObject.GetComponent<Rigidbody>().velocity);
        }

        Debug.Log(gameObject.GetComponent<Rigidbody>().velocity);

        --curAmmo;
        UI_Manager.Instance.UpdateAmmo();
        tillReload = MaxTillReload;
    }

    // For multiplayer only
    [PunRPC]
    public void LoadPoopProperties(int _poopID, Vector3 _velocity)
    {
        GameObject poop = PhotonNetwork.GetPhotonView(_poopID).gameObject;
        poop.GetComponent<Rigidbody>().AddForce(_velocity);
        poop.transform.SetParent(ProjectileList);
    }
}
