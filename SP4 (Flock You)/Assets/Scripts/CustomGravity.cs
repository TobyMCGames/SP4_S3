using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.AddForce(new Vector3(0, GlobalSettings.gravity, 0) * Time.deltaTime, ForceMode.VelocityChange);
    }
}
