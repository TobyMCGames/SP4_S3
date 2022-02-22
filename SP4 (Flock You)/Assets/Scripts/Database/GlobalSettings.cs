using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    // Game Modes
    public enum GAMEMODES
    {
        SINGLE_PLAYER,
        MULTIPLAYER,
        NUM_GAME_MODES
    }

    public static GAMEMODES curMode = GAMEMODES.SINGLE_PLAYER;

    public static string BaseServer = "http://ec2-3-210-222-73.compute-1.amazonaws.com/FlockYou_server/";
    public static string Username;

    public void Awake()
    {
        curMode = GAMEMODES.SINGLE_PLAYER;
    }
}
