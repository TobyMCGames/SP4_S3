using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

public class PropObserver : MonoBehaviour
{

    private PhotonView photonView;
    private GameObject prop;

    // Start is called before the first frame update
    void Start()
    {
        photonView = transform.parent.GetComponent<PhotonView>();
        prop = null;
    }

    void OnTriggerEnter(Collider other)
    {
        prop = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        prop = null;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(prop.name);
    }

    public GameObject GetProp()
    {
        return prop;
    }
}
