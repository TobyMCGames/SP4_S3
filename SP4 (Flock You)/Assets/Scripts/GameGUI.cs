using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class GameGUI : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public Text HealthText;
    public Text NumPlayerText;
    public GameObject HunterSelect;
    public GameObject PropSelect;
    public GameObject HunterBlockVision;
    public GameObject AbilityShadow;
    public GameObject SpawnAbilityIcon;
    public GameObject InvisibilityAbilityIcon;
    public GameObject RevealAbilityIcon;
    public GameObject StunAbilityIcon;
    public GameObject Crosshair;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInfo)
        {
            if (HealthText)
                HealthText.text = "Health: " + playerInfo.GetHealth();

            if (playerInfo.GetAbility() != PlayerInfo.ABILITY.NO_ABILITY)
            {
                if (playerInfo.IsAbilityAvailable())
                    AbilityShadow.SetActive(false);
                else
                    AbilityShadow.SetActive(true);
            }
        }

        if (NumPlayerText)
            NumPlayerText.text = "Number of players: " + PhotonNetwork.PlayerList.Length;
    }

    public void SelectAbilityScreen(bool state)
    {

        if (playerInfo.GetTeam() == PlayerInfo.TEAMS.HUNTER)
        {
            HunterSelect.SetActive(state);
            HunterBlockVision.SetActive(state);
            Debug.Log("Hunter screen set to: " + state);
        }
        else if (playerInfo.GetTeam() == PlayerInfo.TEAMS.PROP)
        {
            PropSelect.SetActive(state);
            Debug.Log("Prop screen set  to: " + state);
        }
    }

    public void SetAbilityIcon(PlayerInfo.ABILITY name)
    {
        if (name == PlayerInfo.ABILITY.SPAWN)
        {
            SpawnAbilityIcon.SetActive(true);
        }
        else if (name == PlayerInfo.ABILITY.INVISIBILITY)
        {
            InvisibilityAbilityIcon.SetActive(true);
        }
        else if (name == PlayerInfo.ABILITY.REVEAL)
        {
            RevealAbilityIcon.SetActive(true);
        }
        else if (name == PlayerInfo.ABILITY.STUN)
        {
            StunAbilityIcon.SetActive(true);
        }
    }

    public void SetAbilityAvailableIcon(bool state)
    {
        AbilityShadow.SetActive(state);
    }
}
