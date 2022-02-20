using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using Cinemachine;

public class FYGameManager : MonoBehaviourPunCallbacks
{
    // Singleton Instance
    public static FYGameManager Instance = null;
    private PhotonView photonView;

    // Game Modes
    public enum GAMEMODES
    {
        SINGLE_PLAYER,
        MULTIPLAYER,
        NUM_GAME_MODES
    }

    public GAMEMODES curMode;

    // Game Timer
    public float curTimer;

    [Header("Loading Screen")]
    public GameObject UI_Screen;
    public GameObject Loading_Screen;
    private Text Loading_InfoText;
    //Temp
    public Text Direction_Test;


    [Header("Main Player")]
    // Main client player
    public GameObject PlayerPrefab;
    public FYPlayer player;
    public CinemachineFreeLook MainCamera;
    public Transform PlayerList;
    public Transform ProjectileList;

    [Header("Server Settings")]
    bool AllPlayersAssigned = false;
    // Player List
    //FYPlayer[] playerList;

    // Spawn Points
    public Transform[] SpawnPoints;

    public void Awake()
    {
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (ProjectileList == null)
            Debug.Log("Projectile List not set in FYGameManager");
        if (UI_Screen == null)
            Debug.Log("UI_Screen not set in FYGameManager");
        if (Loading_Screen == null)
            Debug.Log("Loading_Screen not set in FYGameManager");
        else
            Loading_InfoText = Loading_Screen.transform.GetChild(0).GetComponent<Text>();

        photonView = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

    }
    private void OnCountdownTimerIsExpired()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.SINGLE_PLAYER)
        {
            Initialize();
            UI_Screen.SetActive(true);
            AssignSpawnPoints();
            player.ResetSpawnPoint();
        }
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
        {
            Loading_Screen.SetActive(true);
            Hashtable props = new Hashtable
            {
                {JLGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
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
        //CheckEndOfGame();
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

        if (changedProps.ContainsKey(FYGame.PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                if (!startTimeIsSet)
                {
                    photonView.RPC("Initialize", RpcTarget.AllViaServer);
                    AssignSpawnPoints();
                }
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ", this.Loading_InfoText);
                if (UI_Screen == null)
                    Loading_InfoText.text = "Waiting for other players...";
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

    // Update is called once per frame
    void Update()
    {
        if (curMode == GAMEMODES.MULTIPLAYER)
        {
            if (!AllPlayersAssigned)
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == PlayerList.childCount)
                    AssignSpawnPoints();
        }

        if (!isOver())
        {
            curTimer -= Time.deltaTime;
        }
    }

    public bool isOver()
    {
        return curTimer <= 0;
    }

    void AssignSpawnPoints()
    {
        if (SpawnPoints.Length == 0)
        {
            Debug.Log("No SpawnPoints are set");
            return;
        }
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.SINGLE_PLAYER)
        {
            if (player == null)
            {
                Debug.Log("No Player is set");
                return;
            }

            // Do if single player vvv
            player.setDefaultSpawn(SpawnPoints[0].position);
            AllPlayersAssigned = true;
        }
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (PlayerList.childCount > SpawnPoints.Length)
                Debug.Log("There are more players than spawnpoints");

            for (int i = 0; i < PlayerList.childCount; i++)
            {
                int ViewID = PlayerList.GetChild(i).gameObject.GetPhotonView().ViewID;
                photonView.RPC("AssignPlayerSpawn", RpcTarget.AllViaServer, ViewID, i);
                Debug.Log(ViewID + " spawn point is at " + SpawnPoints[i].position);
            }
        }
    }

    void SetUpUI()
    {
        // DeActiviate Loading_Screen for player after instantiating
        if (Loading_Screen != null)
            Loading_Screen.SetActive(false);
        // ReActiviate UI for player after instantiating
        if (UI_Screen != null)
        {
            UI_Screen.SetActive(true);
            UI_Manager.Instance.Init();
        }
    }

    [PunRPC]
    private void Initialize()
    {
        Debug.Log("Initializing");

        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        GameObject ClientPlayer = null;
        if (curMode == GAMEMODES.SINGLE_PLAYER && player == null)
        {
            ClientPlayer = Instantiate(PlayerPrefab, PlayerList);
            ClientPlayer.GetComponent<PhotonTransformView>().enabled = false;
        }
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
            ClientPlayer = PhotonNetwork.Instantiate("player_online", position, rotation, 0);      // avoid this call on rejoin (ship was network instantiated before)

        if (player != null)
        {
            SetUpUI();
            return;
        }

        player = ClientPlayer.GetComponent<FYPlayer>();

        MainCamera.Follow = player.transform;
        MainCamera.LookAt = player.transform;

        player.GetComponent<MovePlayer>().camera = MainCamera;
        //player.GetComponent<FYPlayer>().Init();
        player.GetComponent<DropLoadControl>().ProjectileList = ProjectileList;
        UI_Manager.Instance.player = player.GetComponent<FYPlayer>();
        // Temp
        player.GetComponent<MovePlayer>().directiontext = Direction_Test;

        // Set up UI now
        SetUpUI();
        //photonView.RPC("RegisterPlayerID", RpcTarget.AllViaServer, player.GetComponent<PhotonView>().ViewID);

        photonView.RPC("SetToPlayerList", RpcTarget.AllViaServer, player.gameObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    private void SetToPlayerList(int playerID)
    {
        if (player.gameObject.GetPhotonView().ViewID == playerID)
        {
            player.transform.SetParent(PlayerList);
            return;
        }

        PhotonNetwork.GetPhotonView(playerID).transform.SetParent(PlayerList);
    }

    [PunRPC]
    private void AssignPlayerSpawn(int playerID, int nestID)
    {
        if (player.GetComponent<PhotonView>().ViewID != playerID)
            return;

        AllPlayersAssigned = true;

        player.setDefaultSpawn(SpawnPoints[nestID].position);
        SpawnPoints[nestID].GetChild(0).GetComponent<SpawnPointScript>().owner = player;
        player.ResetSpawnPoint();
    }
}
