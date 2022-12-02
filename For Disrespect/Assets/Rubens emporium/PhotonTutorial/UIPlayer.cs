using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public PlayerMovement playerMovement;//(TARGET)

    public Text playerName;
    public Slider playerHPBar;
    public GameObject playerGameObject;
    public Transform parentComponent;

    public Vector3 offsetUI;
    public float crCharacterHeight;
    public Transform transformPlayer;
    public Renderer rendererPlayer;
    public CanvasGroup canvasGroupPlayer;
    public CharacterController characterControlPlayer;
    public Vector3 positionPlayer;

    public void Awake()
    {
        canvasGroupPlayer = GetComponentInChildren<CanvasGroup>();
    }
    public void Start()
    {
        parentComponent.SetParent(GameObject.Find("MainCanvas").GetComponent<Transform>());
        if (playerMovement != null)
        {
            transformPlayer = playerMovement.GetComponent<Transform>();
            rendererPlayer = playerMovement.GetComponent<Renderer>();
            characterControlPlayer = playerMovement.GetComponent<CharacterController>();
        }
    }
    public void Update()
    {
        //if(playerHPBar == null && playerMovement != null)
        //{
        //    playerHPBar.value = playerMovement.hp;
        //}
    }
    public void SetTarget(PlayerMovement target)
    {
        if(target == null)
        {
            print("There is no PlayerMovement");
            return;
        }

        playerMovement = target;

        if (playerName != null && playerMovement != null)
        {
            playerName.text = playerMovement.photonID.Owner.NickName.ToString();
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
