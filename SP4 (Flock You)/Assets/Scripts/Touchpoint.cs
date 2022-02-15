using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touchpoint : MonoBehaviour
{
    private GameObject parentGO;
    private float deltaX;
    private float deltaY;
    private float deltaZ;

    void Start()
    {
        parentGO = this.transform.parent.gameObject;

        deltaX = parentGO.transform.position.x - transform.localPosition.x;
        deltaY = parentGO.transform.position.y - transform.localPosition.y;
        deltaZ = parentGO.transform.position.z - transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    { 
        parentGO.transform.localPosition = new Vector3(deltaX, deltaY, deltaZ);
    }
}
