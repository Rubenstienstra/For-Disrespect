using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//[RequireComponent(typeof(InputField))]
public class PlayerNameField : MonoBehaviour
{
    public string crPlayerName;
    public string defaultSavedName;
    public InputField playerNameInput;

    public GameLobbyManager gameLobbyManager;

    public GameLauncher gameLauncher;

    //Zorgt er voor dat de default name gedisplayed word
    public void Start()
    {
        if (playerNameInput != null)
        {
            if (PlayerPrefs.HasKey(crPlayerName))
            {
                defaultSavedName = PlayerPrefs.GetString(crPlayerName);
                playerNameInput.text = defaultSavedName;
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
    }
    public void SetMaxPlayers()
    {
        if (gameLauncher != null)
        {
            gameLauncher.createMaxTotalPlayers = 2;
            print(2);
        }
    }
    public void SetServerName()
    {
        if(gameLauncher != null)
        {
            gameLauncher.createRoomName = gameLauncher.roomName.text;
            print(gameLauncher.roomName.text);
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
    public void SearchServerName()
    {
        gameLauncher.crSelectedRoomName = gameLauncher.displayingRoomNameInput.text;
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
