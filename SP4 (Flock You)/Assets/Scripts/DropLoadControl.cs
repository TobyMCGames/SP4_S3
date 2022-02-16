using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoadControl : MonoBehaviour
{
    // Projectile
    public GameObject PoopPrefab = null;
    public Transform ProjectileList = null;

    // Ammo variables
    public int MaxAmmo = 5;
    public int curAmmo;

    // Reload Variables
    public float MaxTillReload = 1.0f;
    private float tillReload;
    public float MaxRechargeRateTimer = 0.2f;
    private float RechargeRateTimer;

    // Start is called before the first frame update
    void Start()
    {
        curAmmo = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
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

        GameObject poop = Instantiate(PoopPrefab, ProjectileList);
        poop.transform.position = gameObject.transform.position;
        poop.GetComponent<Projectile_Poop>().AssignPooper(gameObject);
        poop.GetComponent<Rigidbody>().AddForce(gameObject.GetComponent<Rigidbody>().velocity);
        Debug.Log(gameObject.GetComponent<Rigidbody>().velocity);

        --curAmmo;
        UI_Manager.Instance.UpdateAmmo();
        tillReload = MaxTillReload;
    }
}
