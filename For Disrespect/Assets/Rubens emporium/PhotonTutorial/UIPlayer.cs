using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerManager playerManager;

    public Image playerStaminaBar;
    public Image playerFallBehindHPBar;
    public Image playerHPBar;

    public GameObject EnemyBar;
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
    public void OnStaminaChange(float stamina)
    {

    }
    public void OnEnemyHealthChange()
    {

    }
}
