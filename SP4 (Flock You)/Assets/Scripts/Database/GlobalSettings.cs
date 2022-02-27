using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalSettings : MonoBehaviour
{

    [Header("Server Settings")]
    //EC2 (online aws)
    public static string BaseServer = "http://ec2-3-210-222-73.compute-1.amazonaws.com/FlockYou_server/";

    //LocalHost (for testing)
    //public static string BaseServer = "http://localhost/FlockYou_server/";

    public static bool NotConnected = true;

    [Header("Player Values")]
    public static string Username = "Default Username";
    public static int Level;
    public static int XP;
    public static int BaseXPToLevelUp = 100;
    public static int XPToLevelUp;
    public static int Cash;
    public static string CurrentSkin = "Main_Player";
    public static string CurrentPerk = "None";
    public static string DefaultSkin = "Main_Player";
    public static string Defaultperk = "None";
    public static string itemname;

    [Header("Game Settings")]
    public static float gravity = -29.4f;
    public static float Volume = 1.0f;

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