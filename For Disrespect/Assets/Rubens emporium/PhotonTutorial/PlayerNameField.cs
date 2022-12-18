using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

//[RequireComponent(typeof(InputField))]
public class PlayerNameField : MonoBehaviour
{
    public string crPlayerName;
    public string defaultSavedName;
    public InputField playerNameInput;
    public TMP_InputField playerTextProNameInput;

    public TMP_Text playerTextProNameMainMenu;

    //public GameLobbyManager gameLobbyManager;
    public GameLauncher gameLauncher;

    //Zorgt er voor dat de default name gedisplayed word
    public void Start()
    {
        if (PlayerPrefs.HasKey(crPlayerName)) // hiermee zie je dierect je gebruikers naam wanneer je inlogt.
        {
            if (playerNameInput != null)
            {
                defaultSavedName = PlayerPrefs.GetString(crPlayerName);
                playerNameInput.text = defaultSavedName;
            }
            else if (playerTextProNameInput != null)
            {
                defaultSavedName = PlayerPrefs.GetString(crPlayerName);
                playerTextProNameInput.text = defaultSavedName;
            }
            if(playerTextProNameMainMenu != null && defaultSavedName != null)
            {
                playerTextProNameMainMenu.text = defaultSavedName;
            }
        }
        PhotonNetwork.NickName = defaultSavedName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            print("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(crPlayerName, value);
        
        if(playerTextProNameMainMenu != null)
        {
            playerTextProNameMainMenu.text = value;
        }
    }
    public void SetMaxPlayers()
    {
        if (gameLauncher != null)
        {
            gameLauncher.createMaxTotalPlayers = 2;
            print(2);
        }
    }
    public void SetPrivateSettings()
    {
        if (gameLauncher != null)
        {
            gameLauncher.createPrivacySettings = gameLauncher.privacySettings.isOn;
            print(gameLauncher.privacySettings.isOn);
        }
    }
    public void CreatingRoomUI()
    {
       if(gameLauncher != null)
       {
            gameLauncher.choosingLobbyOrCreate.SetActive(false);
            gameLauncher.creatingLobby.SetActive(true);
       }
    }
}
