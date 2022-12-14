using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeGameCam : MonoBehaviour
{

    public Animator beforeGameCam;

    public Animator enemyText;
    public Animator hostBars;
    public Animator joinBars;

    public GameObject gameSettingsHost;
    public GameObject gameSettingsJoin;

    public void camMove()
    {
        beforeGameCam.SetBool("BeforeComabt", true);
    }

    public void camBack()
    {
        beforeGameCam.SetBool("BeforeComabt", false);
    }

    public void gameStart()
    {
        enemyText.SetBool("GameStart", true);
        beforeGameCam.SetBool("GameStart", true);
        hostBars.SetBool("GameStart", true);
        joinBars.SetBool("GameStart", true);

        gameSettingsHost.SetActive(false);
        gameSettingsJoin.SetActive(false);


    }

}
