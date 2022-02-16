using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance = null;

    public FYPlayer player;

    public Text UI_Timer;
    //Temporary use text instead of image
    public Text UI_Health;
    public Text UI_Ammo;
    public Text UI_Lives;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
    }

    public void Init()
    {
        UpdateHealth();
        UpdateAmmo();
        UpdateLives();
    }

    // Update is called once per frame
    void Update()
    {
        if (UI_Timer != null)
        {
            int min = (int)FYGameManager.Instance.curTimer / 60;
            int sec = (int)FYGameManager.Instance.curTimer % 60;
            if (sec > 10)
                UI_Timer.text = min + ":" + sec;
            else
                UI_Timer.text = min + ":0" + sec; 
        }
    }

    public void UpdateHealth()
    {
        if (UI_Health == null)
        {
            Debug.Log("Text_numHealth not set");
            return;
        }

        UI_Health.text = "x " + player.curHealth;
    }

    public void UpdateAmmo()
    {
        if (UI_Ammo == null)
        {
            Debug.Log("Text_numAmmo not set");
            return;
        }

        UI_Ammo.text = "x " + player.DropLoadControl_script.curAmmo;
    }

    public void UpdateLives()
    {
        if (UI_Lives == null)
        {
            Debug.Log("Text_numLives not set");
            return;
        }

        UI_Lives.text = "x " + player.curLives;
    }
}
