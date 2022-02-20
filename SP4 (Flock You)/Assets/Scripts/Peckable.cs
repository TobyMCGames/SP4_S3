using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peckable : MonoBehaviour
{
    protected bool canBeDamaged = false;

    public bool CanBeDamaged()
    {
        return canBeDamaged;
    }

    public void SetCanBeDamaged(bool status)
    {
        canBeDamaged = status;
    }

    protected virtual void Start(){}
    protected virtual void Update(){}
}
