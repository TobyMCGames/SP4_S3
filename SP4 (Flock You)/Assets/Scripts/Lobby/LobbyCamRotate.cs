using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamRotate : MonoBehaviour
{
    public float rotationSpeed = 10;
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
