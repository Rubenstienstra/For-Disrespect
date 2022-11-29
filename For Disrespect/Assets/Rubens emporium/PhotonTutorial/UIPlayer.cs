using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public Text playerName;
    public Slider playerHPBar;
    public PlayerMovement playerMovement;

    public Transform parentComponent;

    public void Start()
    {
        parentComponent.SetParent(GameObject.Find("MainCanvas").GetComponent<Transform>());
        if(playerName != null && playerMovement != null)
        {
            playerName.text = playerMovement.photonID.Owner.NickName.ToString();
        }
    }
    public void OnHealthChange(float hp)
    {
        playerHPBar.value = hp;
        print("Player Total HP: " + hp);
    }
}
