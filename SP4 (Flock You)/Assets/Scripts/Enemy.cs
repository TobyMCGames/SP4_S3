using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject POV_Agro;
    public GameObject POV_DeAgro;
    public enum FSM { IDLE, PATROL, AGRO, STUN }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}