using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYGameManager : MonoBehaviour
{
    // Singleton Instance
    public static FYGameManager Instance = null;

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

    // Spawn Points
    public Transform[] SpawnPoints;

    // Main client player
    public FYPlayer player;

    // Player List
    //FYPlayer[] playerList;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        AssignSpawnPoints();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (player == null)
        {
            Debug.Log("No Player is set");
            return;
        }

        if (SpawnPoints.Length == 0)
        {
            Debug.Log("No SpawnPoints are set");
            return;
        }

        //int random_iterator = Random.Range(0, SpawnPoints.Length - 1);
        //
        //player.setDefaultSpawn(SpawnPoints[random_iterator]);
        player.setDefaultSpawn(SpawnPoints[0]);
    }
}
