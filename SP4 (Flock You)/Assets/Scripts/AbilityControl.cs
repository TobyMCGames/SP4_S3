using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class AbilityControl : MonoBehaviour
{
    PhotonView photonView;

    //private Ability CurAbility = null;
    //public Ability[] Abilities;
    private List<Ability> AbilitiesList = new List<Ability>();
    private float ScrollLimit = 2f;
    private float ScrollValue = 0f;
    private int AbilityCount = 0;
    public int currAbility = 0;
    DistractAbility distactAbility = new DistractAbility();
    LayEggAbility layEggAbility = new LayEggAbility();

    public AudioManager PlayerAudio;

    public enum ABILITIES { 
        DISTRACT,
        LAYEGG,
        NUM_ABBILITIES
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.Log("photonView not set in AbilityControl.cs");

        //add abilities to list
        AbilitiesList.Add(distactAbility);
        AbilitiesList.Add(layEggAbility);
        AbilityCount = AbilitiesList.Count;

        for (int i = 0; i < AbilityCount; i++)
        {
            AbilitiesList[i].SetPlayer(transform);
            AbilitiesList[i].SetPhotonView(photonView);
            AbilitiesList[i].SetPlayerAudio(PlayerAudio);
        }
    }

    private void Start()
    {
        UI_Manager.Instance.UpdateAbility();
    }

    public LayEggAbility GetLayEggAbility()
    {
        return layEggAbility;
    }

    public DistractAbility GetDistractAbility()
    {
        return distactAbility;
    }

    private void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
            if (FYGameManager.Instance.isPaused)
                return;

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;
        //mouse scroll check which ability to use
        ScrollValue += Input.mouseScrollDelta.y;
        if(ScrollValue >= ScrollLimit)
        {
            currAbility += 1;
            ScrollValue = 0;

            //if more than AbilityCount, then go back to first ability
            if(currAbility >= AbilityCount)
            {
                currAbility = 0;
            }
            UI_Manager.Instance.UpdateAbility();
            Debug.Log(currAbility);
        }
        else if (ScrollValue <= -ScrollLimit)
        {
            currAbility -= 1;
            ScrollValue = 0;

            //if less than 0, then go back to last ability
            if (currAbility < 0)
            {
                currAbility = AbilityCount - 1;
            }
            UI_Manager.Instance.UpdateAbility();
            //Debug.Log(currAbility);
        }

        //get key to activate ability
        if(Input.GetKeyDown(KeyCode.Q) && AbilitiesList[currAbility].CanUse())
        {
            AbilitiesList[currAbility].UseAbility();
        }

        //update for abilities
        layEggAbility.Update();
        distactAbility.Update();
    }

    [PunRPC]
    private void DistractEnemyList(int[] IDList, Vector3 _pos)
    {
        for (int i = 0; i < IDList.Length; i++)
        {
            PhotonView enemyView = PhotonNetwork.GetPhotonView(IDList[i]);
            if (!enemyView.IsMine)
                continue;
            Enemy enemy = enemyView.GetComponent<Enemy>();
            enemy.Distracted(_pos);
        }
    }

    [PunRPC]
    public void PlayAudio(string key)
    {
        if (PlayerAudio != null)
            PlayerAudio.PlayAudio(key);
    }
}
