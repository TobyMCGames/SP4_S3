using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    protected float points;
    protected float health;
    protected float respawns;

    void Awake()
    {
        //Init values
        points = 0;
        health = 5;
        respawns = 3;
    }
}
