using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalSettings : MonoBehaviour
{
    //EC2 (online aws)
    public static string BaseServer = "http://ec2-3-210-222-73.compute-1.amazonaws.com/FlockYou_server/";

    //LocalHost (for testing)
    //public static string BaseServer = "http://localhost/FlockYou_server/";

    public static string Username = "Default Username";
    public static int Level;
    public static int XP;
    public static int Cash;
    public static string CurrentSkin;
    public static string itemname;
    public static bool NotConnected = true;
    
    // Game Modes
    public enum GAMEMODES
    {
        SINGLE_PLAYER,
        MULTIPLAYER,
        NUM_GAME_MODES
    }

    public static GAMEMODES curMode;

    private void OnApplicationQuit()
    {
        curMode = GAMEMODES.SINGLE_PLAYER;
        NotConnected = true;
    }
}

[Serializable]
public class PlayerGameValues
{
    public string sUsername;
    public int ilevel;
    public int iPoints;
}