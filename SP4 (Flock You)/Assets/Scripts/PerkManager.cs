using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    public enum ActivePerk
    {
        None,
        Speedy_Legs,
        Jumpy,
        Hardy,
        Full_Nest
    }
    ActivePerk activePerk;

    public void ApplyPlayerPerk()
    {
        string strPerk = GlobalSettings.CurrentPerk;
        strPerk = strPerk.Replace(" ", "_");
        Debug.Log("Current perk: " + strPerk);
        activePerk = (ActivePerk)System.Enum.Parse(typeof(ActivePerk), strPerk);

        switch (activePerk)
        {
            case ActivePerk.None:
                break;
            case ActivePerk.Speedy_Legs:
                float playermovespeed = transform.GetComponent<MovePlayer>().movemul;
                transform.GetComponent<MovePlayer>().movemul = playermovespeed * 2;
                Debug.Log("Speedy legs active");
                break;
            case ActivePerk.Jumpy:
                float jumpForce = transform.GetComponent<MovePlayer>().jumpforce;
                transform.GetComponent<MovePlayer>().jumpforce = jumpForce * 2;
                Debug.Log("Jumpy active");
                break;
            case ActivePerk.Hardy:
                int PlayerMaxHealth = transform.GetComponent<FYPlayer>().MaxHealth;
                transform.GetComponent<FYPlayer>().MaxHealth = PlayerMaxHealth + 1;
                transform.GetComponent<FYPlayer>().curHealth = transform.GetComponent<FYPlayer>().MaxHealth;
                transform.GetComponent<FYPlayer>().MaxLives = transform.GetComponent<FYPlayer>().MaxLives - 1;
                Debug.Log("hardy active");
                break;
            case ActivePerk.Full_Nest:
                int PlayerMaxLives = transform.GetComponent<FYPlayer>().MaxLives;
                transform.GetComponent<FYPlayer>().MaxLives = PlayerMaxLives + 1;
                Debug.Log("Full Nest active");
                break;
        }
    }
}
