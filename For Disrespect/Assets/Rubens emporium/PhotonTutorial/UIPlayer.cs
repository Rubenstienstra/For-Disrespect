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

    public void Start()
    {
        if (!playerMovement.photonID.IsMine)
        {
            Destroy(gameObject);
        }

        if (playerName != null && playerMovement != null)
        {
            playerName.text = playerMovement.photonID.Owner.NickName.ToString();
        }

        //parentComponent.SetParent(GameObject.Find("MainCanvas").GetComponent<Transform>());
        //if (playerMovement != null)
        //{
        //    transformPlayer = playerMovement.GetComponent<Transform>();
        //    rendererPlayer = playerMovement.GetComponent<Renderer>();
        //    characterControlPlayer = playerMovement.GetComponent<CharacterController>();
        //}
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
