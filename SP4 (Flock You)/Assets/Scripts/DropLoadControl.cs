using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoadControl : MonoBehaviour
{
    // Ammo variables
    public int MaxAmmo = 5;
    private int curAmmo;

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
            Debug.Log("Drop load");
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
    }
}
