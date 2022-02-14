using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempText : MonoBehaviour
{
    private Text infoText;
    private float activeTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        infoText = GetComponent<Text>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (activeTime > 0)
            {
                activeTime -= Time.deltaTime;
            }
            else
                gameObject.SetActive(false);
        }
    }

    public void SetAlertText(string text, float time)
    {
        infoText.text = text;
        activeTime = time;
        gameObject.SetActive(true);
    }
}
