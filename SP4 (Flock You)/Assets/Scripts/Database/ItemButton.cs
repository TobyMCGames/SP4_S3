using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public void SetThisItem()
    {
        //get the LocalShopManager script from Shop Panel
        transform.parent.parent.parent.GetComponent<LocalShopManager>().ChangeStats(transform);
    }
}
