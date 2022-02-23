using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalSettings : MonoBehaviour
{
    //EC2 (online aws)
    //public static string BaseServer = "http://ec2-3-210-222-73.compute-1.amazonaws.com/FlockYou_server/";

    //LocalHost (for testing)
    public static string BaseServer = "http://localhost/FlockYou_server/";

    public static string Username;
    public static int Level;
    public static int XP;
    public static int Cash;
    public static string ShopFirstPage = "skin";
    
    // Game Modes
    public enum GAMEMODES
    {
        SINGLE_PLAYER,
        MULTIPLAYER,
        NUM_GAME_MODES
    }

    public static GAMEMODES curMode = GAMEMODES.SINGLE_PLAYER;

    public void Awake()
    {
        curMode = GAMEMODES.SINGLE_PLAYER;
    }
}

[Serializable]
public class PlayerGameValues
{
    public string sUsername;
    public int ilevel;
    public int iPoints;
}