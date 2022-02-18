using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    private Camera thecam;

    void Start()
    {
        gameObject.SetActive(false);
        thecam = Camera.main;
    }


    void LateUpdate()
    {
        transform.LookAt(thecam.transform);
    }
}
