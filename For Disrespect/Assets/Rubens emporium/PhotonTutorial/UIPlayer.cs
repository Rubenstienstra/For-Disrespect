using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public Text playerName;
    public Slider playerHPBar;
    public GameObject playerGameObject;
    public Transform parentComponent;

    public void Start()
    {
        if (!playerMovement.photonID.IsMine)
        {
            //gameObject.SetActive(false);
        }

        if (playerName != null && playerMovement != null)//Canvas 
        {
            if (playerMovement.photonID.IsMine)
            {
                playerName.text = playerMovement.photonID.Owner.NickName.ToString();
                return;
            }
            playerName.text = playerMovement.crPlayerName;
        }



        
    }
    public void OnHealthChange(float hp)
    {
        if(playerHPBar != null)
        {
            playerHPBar.value = hp;
            print("Player Total HP: " + hp);
        }
    }
}
