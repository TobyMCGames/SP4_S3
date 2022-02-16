using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayEgg : Ability
{
    public GameObject NestPrefab;

    public LayEgg()
    {
        cooldown = 5f;
    }

}
