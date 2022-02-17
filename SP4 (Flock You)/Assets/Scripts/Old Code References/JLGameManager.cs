using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Cinemachine;

public class JLGameManager : MonoBehaviourPunCallbacks
{
    public static JLGameManager Instance = null;
    public Text InfoText;
    public TempText AlertText;
    public GameObject MainCamera;
    public CinemachineVirtualCamera ThirdPersonCam;
    public CinemachineVirtualCamera FirstPersonCam;
    public GameGUI gameGui;
    public GameObject[] SphereSpawnPoints;
    public float HidingTime;
    public float GameTime;

    private PhotonView photonView;
    private List<int> PlayerList = new List<int>();
    private PlayerInfo playerInfo;
    private List<int> HunterList = new List<int>();
    private List<int> PropList = new List<int>();
    private bool Initialized = false;
    private bool GameStarted = false;
    private bool HidingPhase = false;
    private bool HuntingPhase = false;
    private float SphereSpawnTime = 0;

    public void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

    }

    // Start is called before the first frame update
    public void Start()
    {
        Hashtable props = new Hashtable
            {
                {JLGame.PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
    }

    #region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
           // StartCoroutine(SpawnAsteroid());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckEndOfGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
        int startTimestamp;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

        if (changedProps.ContainsKey(JLGame.PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                if (!startTimeIsSet)
                {
                    photonView.RPC("Initialize", RpcTarget.AllViaServer);
                }
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ", this.InfoText);
                InfoText.text = "Waiting for other players...";
            }
        }

    }

    #endregion

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(FYGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

    //[PunRPC]
    private void Initialize()
    {
        Debug.Log("Initializing");

        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        
        GameObject player = PhotonNetwork.Instantiate("JohnLemon", position, rotation, 0);      // avoid this call on rejoin (ship was network instantiated before)
        GameObject CameraPoint = player.transform.Find("CameraPoint").gameObject;

        player.GetComponent<PlayerMovement>().enabled = false;

        if(player.GetComponent<PhotonView>().IsMine)
        {
            ThirdPersonCam.Follow = player.transform;
            FirstPersonCam.Follow = player.transform;
            if (CameraPoint != null)
            {
                ThirdPersonCam.LookAt = CameraPoint.transform;
            }
            
            playerInfo = player.GetComponent<PlayerInfo>();

            photonView.RPC("RegisterPlayerID", RpcTarget.AllViaServer, player.GetComponent<PhotonView>().ViewID);

            
            gameGui.playerInfo = playerInfo;

        }

        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("AssignTeam", RpcTarget.AllViaServer);
    }
    
    private void StartGame()
    {
        Debug.Log("Start Game");
        GameStarted = true;

        // Set Respective Camera
        if (playerInfo.GetTeam() == PlayerInfo.TEAMS.PROP)
            playerInfo.GetComponent<PlayerMovement>().SetCamera(ThirdPersonCam.gameObject, PlayerMovement.CAMERA_TYPE.THIRD_PERSON);
        else if (playerInfo.GetTeam() == PlayerInfo.TEAMS.HUNTER)
            playerInfo.GetComponent<PlayerMovement>().SetCamera(FirstPersonCam.gameObject, PlayerMovement.CAMERA_TYPE.FIRST_PERSON);

        CountdownTimer.SetStartTime();
        gameGui.SelectAbilityScreen(true);
    }

    private void CheckEndOfGame()
    {
        if (HunterList.Count == 0)
        {
            InfoText.text = "Props Win";
            HuntingPhase = false;
        }
        else if (PropList.Count == 0)
        {
            InfoText.text = "Hunters Win";
            HuntingPhase = false;
        }
    }

    private void OnCountdownTimerIsExpired()
    {
        HidingPhase = true;
    }
    // Update is called once per frame
    void Update()
    {

        if (!Initialized)
        {
            if (playerInfo == null)
                return;
            if (PhotonNetwork.GetPhotonView(PlayerList[PlayerList.Count - 1]).GetComponent<PlayerInfo>().GetTeam() != PlayerInfo.TEAMS.NO_TEAM)
            {
                Initialized = true;
            }
        }
        if (Initialized && !GameStarted)
        {
            StartGame();
        }

        if (playerInfo != null)
        {
            if (playerInfo.GetTeam() == PlayerInfo.TEAMS.PROP &&
                playerInfo.GetAbility() != PlayerInfo.ABILITY.NO_ABILITY &&
                !playerInfo.GetComponent<PlayerMovement>().GetCamera().activeInHierarchy)
            {
                playerInfo.GetComponent<PlayerMovement>().GetCamera().SetActive(true);
                playerInfo.GetComponent<PlayerMovement>().enabled = true;
                // Set Ability Icon
                gameGui.SetAbilityIcon(playerInfo.GetAbility());
                gameGui.SetAbilityAvailableIcon(false);
            }
        }

        if (GameStarted)
        {
            if (HidingPhase)
            {
                HidingTime -= Time.deltaTime;
                InfoText.text = "Releasing Hunters in: " + (int)HidingTime;
                if (HidingTime < 0)
                {
                    HidingPhase = false;
                    HuntingPhase = true;
                    gameGui.SelectAbilityScreen(false);
                    playerInfo.AutoAssignAbility();

                    if (playerInfo.GetTeam() == PlayerInfo.TEAMS.HUNTER)
                    {
                        playerInfo.GetComponent<PlayerMovement>().GetCamera().SetActive(true);
                        playerInfo.GetComponent<PlayerMovement>().enabled = true;
                        playerInfo.transform.Find("PointOfView").GetChild(0).gameObject.SetActive(true);
                        gameGui.Crosshair.SetActive(true);
                    }

                    // Set Ability Icon
                    gameGui.SetAbilityIcon(playerInfo.GetAbility());
                    gameGui.SetAbilityAvailableIcon(false);
                }
            }
            if (HuntingPhase)
            {
                GameTime -= Time.deltaTime;
                InfoText.text = "Time Left: " + (int)GameTime;

                if (PhotonNetwork.IsMasterClient)
                {
                    SphereSpawnTime += Time.deltaTime;
                    if (SphereSpawnTime >= 10f)
                    {
                        SphereSpawnTime = 0;
                        photonView.RPC("SpawnAbilitySphere", RpcTarget.AllViaServer);
                        Debug.Log("Spawned Orb");
                    }
                }

                if (playerInfo != null)
                {
                    if (playerInfo.GetHealth() <= 0)
                    {
                        photonView.RPC("RemoveFromList", RpcTarget.AllViaServer, playerInfo.GetComponent<PhotonView>().ViewID);
                        AlertText.SetAlertText("You died", 2f);
                        PhotonNetwork.Destroy(playerInfo.gameObject);
                        playerInfo = null;
                    }
                }

                CheckEndOfGame();

                if (GameTime < 0)
                {
                    HuntingPhase = false;
                }
            }

        }
    }

    public void SetAbility(string ability)
    {
        playerInfo.SetPlayerAbility(ability);
    }

    public List<int> GetHunterList()
    {
        return HunterList;
    }

    public List<int> GetPropList()
    {
        return PropList;
    }

    [PunRPC]
    public void AssignTeam()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int MaxHunterNum = 1;
        if (PlayerList.Count <= 4)
            MaxHunterNum = 1;
        else if (PlayerList.Count <= 8)
            MaxHunterNum = 2;

        int NumHunter = 0;

        for (int i = 0; i < PlayerList.Count; i++)
        {
            int ID = PlayerList[i];
            Debug.Log("Current loop: " + i);
            Debug.Log("Current ID: " + ID);
            if (NumHunter == MaxHunterNum)
            {
                photonView.RPC("SetTeamByID", RpcTarget.AllViaServer, ID, "Prop");
            }
            else if (MaxHunterNum - NumHunter == PlayerList.Count - i)
            {
                photonView.RPC("SetTeamByID", RpcTarget.AllViaServer, ID, "Hunter");
                //photonView.RPC("SetTeamByID", RpcTarget.AllViaServer, ID, "Prop");
                NumHunter++;
            }
            else
            {
                //if (Random.value > 0.6f)
                if (Random.value > 1.0f)
                {
                    photonView.RPC("SetTeamByID", RpcTarget.AllViaServer, ID, "Hunter");
                    NumHunter++;
                }
                else
                {
                    photonView.RPC("SetTeamByID", RpcTarget.AllViaServer, ID, "Prop");
                }

            }
        }
    }

    [PunRPC]
    public void RegisterPlayerID(int id)
    {
        PlayerList.Add(id);
    }

    [PunRPC]
    public void SetTeamByID(int id, string team)
    {
        Debug.Log("Setting team");
        if (team == "Prop")
        {
            PhotonNetwork.GetPhotonView(id).GetComponent<PlayerInfo>().SetTeam(PlayerInfo.TEAMS.PROP);
            PropList.Add(id);
            Debug.Log("Setting " + id + " into prop");
        }
        else if (team == "Hunter")
        {
            PhotonNetwork.GetPhotonView(id).GetComponent<PlayerInfo>().SetTeam(PlayerInfo.TEAMS.HUNTER);
            HunterList.Add(id);
            Debug.Log("Setting " + id + " into hunter");
        }
    }

    [PunRPC]
    public void SpawnAbilitySphere()
    {
        AlertText.SetAlertText("Ability Orb has Spawned!", 2.0f);

        if (!PhotonNetwork.IsMasterClient)
            return;

        int range = Random.Range(0, SphereSpawnPoints.Length - 1);
        Vector3 pos = SphereSpawnPoints[range].transform.position;
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        GameObject orb = PhotonNetwork.Instantiate("AbilitySphere", pos, rotation, 0);
        GameObject pillar = PhotonNetwork.Instantiate("LightBeam", pos, rotation);
        photonView.RPC("SetLightBeamToObject", RpcTarget.AllViaServer, pillar.GetComponent<PhotonView>().ViewID, orb.GetComponent<PhotonView>().ViewID, -1.0f, true);
    }

    [PunRPC]
    public void SetLightBeamToObject(int pillarID, int objectID, float timer, bool infinite)
    {
        GameObject gameObject = PhotonNetwork.GetPhotonView(objectID).gameObject;
        GameObject pillar = PhotonNetwork.GetPhotonView(pillarID).gameObject;
        pillar.transform.parent = gameObject.transform;
        pillar.GetComponent<TempObject>().SetObjectTimer(timer, infinite);
    }

    [PunRPC]
    public void RemoveFromList(int ID)
    {
        for (int i = 0; i < HunterList.Count; i++)
        {
            if (HunterList[i] == ID)
            {
                HunterList.RemoveAt(i);
                return;
            }
        }
        for (int i = 0; i < PropList.Count; i++)
        {
            if (PropList[i] == ID)
            {
                PropList.RemoveAt(i);
                return;
            }
        }
    }
}
