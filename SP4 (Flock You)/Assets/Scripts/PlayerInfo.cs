using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class PlayerInfo : MonoBehaviour
{
    public enum TEAMS
    {
        NO_TEAM,
        HUNTER,
        PROP,
        NUM_TEAMS
    }

    public enum ABILITY
    {
        NO_ABILITY,
        //Hunter Abilities
        REVEAL,
        STUN,
        //Prop Abilities
        SPAWN,
        INVISIBILITY,

        NUM_ABILITIES
    }

    private PhotonView photonView;
    private int Health;
    private TEAMS team = TEAMS.NO_TEAM;
    private ABILITY ability = ABILITY.NO_ABILITY;
    private bool Ability_Available = false;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        Health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int hp)
    {
        Health = hp;
    }

    public int GetHealth()
    {
        return Health;
    }

    public void AffectHealth(int change)
    {
        Health += change;
    }

    public void SetTeam(TEAMS team)
    {
        this.team = team;
        if (team == TEAMS.HUNTER)
            GetComponent<Animator>().enabled = true;
    }

    public TEAMS GetTeam()
    {
        return team;
    }

    public ABILITY GetAbility()
    {
        return ability;
    }

    public void SetPlayerAbility(string ability)
    {
        if (ability == "Spawn")
            this.ability = ABILITY.SPAWN;
        else if (ability == "Invisibility")
            this.ability = ABILITY.INVISIBILITY;
        else if (ability == "Reveal")
            this.ability = PlayerInfo.ABILITY.REVEAL;
        else if (ability == "Stun")
            this.ability = ABILITY.STUN;
    }

    public void AutoAssignAbility()
    {
        if (ability != ABILITY.NO_ABILITY)
            return;

        if (team == TEAMS.HUNTER)
        {
            ability = ABILITY.REVEAL;
        }
        else if (team == TEAMS.PROP)
        {
            ability = ABILITY.SPAWN;
        }
    }

    public bool IsAbilityAvailable()
    {
        return Ability_Available;
    }

    public void SetAbilityAvailable(bool state)
    {
        Ability_Available = state;
    }
}
