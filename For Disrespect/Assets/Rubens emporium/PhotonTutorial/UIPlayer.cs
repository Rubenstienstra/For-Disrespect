using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public Text playerName;
    public Slider playerHPBar;
    public PlayerMovement playerMovement;

    public void Start()
    {
        if(playerName != null)
        {
            playerName.text = playerMovement.photonID.Owner.NickName;
        }
    }
    public void OnHealthChange()
    {

    }
}
