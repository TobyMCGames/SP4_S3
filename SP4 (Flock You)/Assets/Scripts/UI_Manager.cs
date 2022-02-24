using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance = null;

    public PhotonView photonView;
    public FYPlayer player;

    public Text UI_Timer;
    public Transform scoreboard;
    //Temporary use text instead of image
    public Text UI_Health;
    public Text UI_Ammo;
    public Text UI_Lives;


    private void Awake()
    {
        Instance = this;
        photonView = gameObject.GetPhotonView();
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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.gameObject.SetActive(false);
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

    public void SetupScoreboard()
    {
        List<PlayerGameValues> playerValues = FYGameManager.Instance.playerGameList;

        if (scoreboard == null)
        {
            Debug.Log("Scoreboard has not been set");
            return;
        }

        for (int i = 0; i < playerValues.Count; i++)
        {
            scoreboard.GetChild(i + 1).gameObject.SetActive(true);  
            scoreboard.GetChild(i + 1).GetChild(0).GetComponent<Text>().text = playerValues[i].sUsername;
            scoreboard.GetChild(i + 1).GetChild(1).GetComponent<Text>().text = playerValues[i].ilevel.ToString();
            scoreboard.GetChild(i + 1).GetChild(2).GetComponent<Text>().text = playerValues[i].iPoints.ToString();
        }
    }

    [PunRPC]
    public void UpdatePlayerScore(string _name, int _points, int _objectViewID)
    {
        for (int i = 0; i < FYGameManager.Instance.playerGameList.Count; i++)
        {
            if (FYGameManager.Instance.playerGameList[i].sUsername != _name)
                continue;
            Debug.Log("Player: " + _name + " has now " + _points + " points");
            FYGameManager.Instance.playerGameList[i].iPoints = _points;
            scoreboard.GetChild(i + 1).GetChild(2).GetComponent<Text>().text = _points.ToString();
        }

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(PhotonNetwork.GetPhotonView(_objectViewID).gameObject);
    }
}
