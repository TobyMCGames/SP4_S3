using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUnequipButton : MonoBehaviour
{
    public void SetButton(Transform itemSelected)
    {
        Text itemStatus = itemSelected.GetChild(3).GetComponentInChildren<Text>();
        if (itemStatus.text == "Owned")
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<Button>().enabled = true;
            GetComponentInChildren<Text>().text = "Equip";
        }
        else if (itemStatus.text == "Equipped")
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<Button>().enabled = false;
            GetComponentInChildren<Text>().text = "Equipped";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        
    }
}
