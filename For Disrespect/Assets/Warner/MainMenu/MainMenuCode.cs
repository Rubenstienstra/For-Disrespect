using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCode : MonoBehaviour
{

    public Animator main;
    public Animator settings;
    public Animator multiplayerSelect;
    public Animator create;
    public Animator join;
    public Animator beforeCombatCam;
    public Animator beforeCombatUi;
    // Start is called before the first frame update

    public void mainMenu()
    {
        main.SetBool("Main Menu", true);
    }

    public void gameSettings()
    {
        main.SetBool("Main Menu", false);
        settings.SetBool("Settings", true);

    }

    public void multie()
    {
        main.SetBool("Main Menu", false);
        multiplayerSelect.SetBool("Multiplayer", true);
        join.SetBool("CreateJoin", false);
        create.SetBool("CreateJoin", false);
        beforeCombatCam.SetBool("BeforeComabt", false);
        beforeCombatUi.SetBool("BeforeComabt", false);
    }

    public void gameCreate()
    {
        multiplayerSelect.SetBool("Multiplayer", false);
        create.SetBool("CreateJoin", true);
    }
    public void gameJoin()
    {
        multiplayerSelect.SetBool("Multiplayer", false);
        join.SetBool("CreateJoin", true);
    }
    public void beforeGame()
    {
        join.SetBool("CreateJoin", false);
        create.SetBool("CreateJoin", false);
        beforeCombatCam.SetBool("BeforeComabt", true);
        beforeCombatUi.SetBool("BeforeComabt", true);
    }

}
