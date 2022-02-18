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
    private int currAbility = 0;
    DistractAbility distactAbility = new DistractAbility();
    LayEggAbility layEggAbility = new LayEggAbility();

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
        AbilitiesList.Add(layEggAbility);
        AbilitiesList.Add(distactAbility);
        AbilityCount = AbilitiesList.Count;
    }

    private void Start()
    {
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
        if (photonView != null)
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

            Debug.Log(currAbility);
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

}
