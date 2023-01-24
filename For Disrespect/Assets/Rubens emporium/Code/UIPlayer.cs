using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class UIPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerManager playerManager;

    public Animator roundCountdownStartAnimation;

    public GameObject playerCanvas;
    public Image playerBloodDamageEffect;
    public Image playerRoundStartScreen;
    public GameObject playerESCMenu;

    public GameObject playerWorldSpaceUI;
    public Image playerStaminaBar;
    public Image playerFallBehindHPBar;
    public Image playerHPBar;

    public GameObject enemyWorldSpaceUI;
    public Image enemyStaminaBar;
    public Image enemyFallBehindHPBar;
    public Image enemyHPBar;

    public float timeToWaitHPBar = 1;
    public bool isAlreadyWaiting;
    public bool isSecondInQueue;

    public void Start()
    {
        
    }
    public void FixedUpdate()
    {
        if (PhotonNetwork.CountOfPlayers >= 2 && playerManager.hasStartedGame)
        {
            if(enemyStaminaBar)//playerStaminaBar heb je altijd.
            {
                playerStaminaBar.fillAmount = playerManager.stamina / 100;
                enemyStaminaBar.fillAmount = playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().stamina / 100;
            }
            else if(GameObject.Find("EnemyStamina"))
            {
               enemyStaminaBar = GameObject.Find("EnemyStamina").GetComponent<Image>();
            }
        }
    }
    public void OnHealthChange(Image hpBar, Image fallBehindhpBar, bool isFriendly)// works for enemy RPC and yourzelf player. 
    {
        if (isFriendly)
        {
            hpBar.fillAmount = playerManager.hp / 100;
        }
        else
        {
            hpBar.fillAmount = playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().hp / 100;
        }
        if (playerManager.hp <= 0)
        {
            playerManager.playerAnimations.SetTrigger("Dead");
            print("Player Has Died");
        }
        
        print("Current Enemy VS Your health: " + playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().hp + " " + playerManager.hp);
        StartCoroutine(FallBehindHPWaiting(fallBehindhpBar, isFriendly));

    }
    public IEnumerator FallBehindHPWaiting(Image fallBehindhpBar, bool isFriendly)// works for enemy RPC and yourzelf player.
    {
        if (isAlreadyWaiting)
        {
            isSecondInQueue = true;
        }
        isAlreadyWaiting = true;

        yield return new WaitForSeconds(timeToWaitHPBar);

        if (!isSecondInQueue)
        {
            fallBehindhpBar.fillAmount = playerManager.hp / 100;
            isAlreadyWaiting = false;
            isSecondInQueue = false;
        }
        print("Is the IEnumarator secondInQueue: " + isSecondInQueue + "and waiting: " + isAlreadyWaiting);

        StopCoroutine(FallBehindHPWaiting(fallBehindhpBar, isFriendly));

        yield return new WaitForSeconds(0);
    }
}
