using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerManager playerManager;

    public Text playerName;

    public Image staminaBar;
    public Image playerHPBar;
    public Image fallBehindHPBar;

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
            playerName.text = playerManager.crPlayerName;
        }
    }
    public void OnHealthChange(float hp)
    {
        if(playerHPBar != null)
        {
            playerHPBar.fillAmount = hp/100;
            print("Player Total HP: " + hp);
        }
    }
    public void OnStaminaChange(float stamina)
    {

    }
}
