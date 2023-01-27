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
    public Animator bloodDamageEffect;

    public GameObject loadingScreen;
    public GameObject winScreen;
    public GameObject loseScreen;

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
        if (GameObject.Find("HPbarEnemy"))
        {
            enemyWorldSpaceUI = GameObject.Find("HPbarEnemy");
        }
        if (GameObject.Find("EnemyStamina"))
        {
            enemyStaminaBar = GameObject.Find("EnemyStamina").GetComponent<Image>();
        }
        if (GameObject.Find("EnemyHPBehindFall"))
        {
            enemyFallBehindHPBar = GameObject.Find("EnemyHPBehindFall").GetComponent<Image>();
        }
        if (GameObject.Find("EnemyHP"))
        {
            enemyHPBar = GameObject.Find("EnemyHP").GetComponent<Image>();
        }

    }
    public void FixedUpdate()
    {
        if (PhotonNetwork.CountOfPlayers >= 2 && playerManager.hasStartedGame)
        {
            playerStaminaBar.fillAmount = playerManager.stamina / 100;
            if (enemyStaminaBar && !playerManager.theGameEnded)//playerStaminaBar heb je altijd.
            {
                enemyStaminaBar.fillAmount = playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().stamina / 100;
            }
            else
            {
                print("Player has no enemyStaminaBar");
            }

            playerHPBar.fillAmount = playerManager.hp / 100;
            if (enemyHPBar && !playerManager.theGameEnded)
            {
                enemyHPBar.fillAmount = playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().hp / 100;
            }
            else
            {
                print("Player has no enemyHPBar");
            }
        }
    }
    public void OnPlayerHealthChange()
    {
        print("OnPlayerHealthChange Activated");
        playerHPBar.fillAmount = playerManager.hp / 100;

        if (playerManager.hp <= 0)
        {
            playerManager.playerAnimations.SetTrigger("Dead");
            print("Player Has Died");
        }
        StartCoroutine(FallBehindHPWaiting(playerFallBehindHPBar, true));
    }
    public void OnEnemyHealthChange(Image hpBar, Image fallBehindhpBar)// works for enemy RPC and yourzelf player. 
    {
        print("OnEnemyHealthChange Activated");

        hpBar.fillAmount = playerManager.hp / 100;
        hpBar.fillAmount = playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().hp / 100;
        
        if (playerManager.hp <= 0)
        {
            playerManager.playerAnimations.SetTrigger("Dead");
            print("Player Has Died");
        }
        
        print("Current Enemy VS Your health: " + playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().hp + " " + playerManager.hp);
        StartCoroutine(FallBehindHPWaiting(fallBehindhpBar, false));

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
