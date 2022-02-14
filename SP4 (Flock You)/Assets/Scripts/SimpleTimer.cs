using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTimer : MonoBehaviour
{
    public Text Infotext;

    private float CurrentTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentTime > 0)
        {
            Infotext.text = "Time left: " + CurrentTime;
            CurrentTime -= Time.deltaTime;
        }
        else
            Infotext.text = "";
    }

    public void StartTimer(float countdowntime)
    {
        CurrentTime = countdowntime;
    }
}
