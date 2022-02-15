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

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
