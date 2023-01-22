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

    public Image playerStaminaBar;
    public Image playerFallBehindHPBar;
    public Image playerHPBar;

    public Image enemyStaminaBar;
    public Image enemyFallBehindHPBar;
    public Image enemyHPBar;

    public float timeToWaitHPBar = 1;
    public bool isAlreadyWaiting;
    public bool isSecondInQueue;

    public GameObject playerGameObject;
    public Transform parentComponent;

    public void Start()
    {
        if (!playerMovement.photonID.IsMine)
        {
            //gameObject.SetActive(false);
        }
    }
    public void FixedUpdate()
    {
        if (PhotonNetwork.CountOfPlayers >= 2 && playerManager.isReadyToFight)
        {
            if(enemyStaminaBar)//playerStaminaBar heb je altijd.
            {
                playerStaminaBar.fillAmount = playerManager.stamina / 100;
                enemyStaminaBar.fillAmount = playerManager.crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().stamina / 100;
            }
            else
            {
               enemyStaminaBar = GameObject.Find("EnemyStamina").GetComponent<Image>();
            }
        }
    }
    public void OnHealthChange(float hp)
    {
        if(hp <= 0)
        {
            playerManager.playerAnimations.SetTrigger("Dead");
            print("Player Has Died");
        }
        if(playerHPBar != null)
        {
            playerHPBar.fillAmount = hp / 100;
            print("Player Total HP: " + hp);
            StartCoroutine(FallBehindHPWaiting(hp));
        }
    }
    public IEnumerator FallBehindHPWaiting(float hp)
    {
        if (isAlreadyWaiting)
        {
            isSecondInQueue = true;
        }
        isAlreadyWaiting = true;

        yield return new WaitForSeconds(timeToWaitHPBar);

        if (!isSecondInQueue)
        {
            playerFallBehindHPBar.fillAmount = hp / 100;
            isAlreadyWaiting = false;
            isSecondInQueue = false;
        }
        print("Is the IEnumarator secondInQueue: " + isSecondInQueue + "and waiting: " + isAlreadyWaiting);

        StopCoroutine(FallBehindHPWaiting(hp));

        yield return new WaitForSeconds(0);
    }
    public void OnEnemyHealthChange()
    {

    }
}
