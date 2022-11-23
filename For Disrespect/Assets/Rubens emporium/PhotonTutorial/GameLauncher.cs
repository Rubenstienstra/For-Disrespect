using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    //GameLauncher is voor wanneer je in de game wil.

    public string gameVersion = "1";
    public byte maxPlayersInRoom = 4;
    public bool isConnected;

    public GameObject loadingText;
    public GameObject controlWindow;

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        loadingText.SetActive(true);
        controlWindow.SetActive(false);
        if (!PhotonNetwork.IsConnected)
        {
            isConnected = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
    }

    #endregion

    #region Callbacks Photon

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster was activated");
        loadingText.SetActive(false);
        controlWindow.SetActive(true);
        base.OnConnectedToMaster();

        if (isConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnected = false;
        }
    }
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom was activated: ");
        loadingText.SetActive(false);
        controlWindow.SetActive(true);

        print("Loading Room For " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.CurrentRoom.PlayerCount <= maxPlayersInRoom)
        {
            PhotonNetwork.LoadLevel("Room For " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("OnDisconnected was activated: " + cause);
        isConnected = false;
        if("Launcher" == SceneManager.GetActiveScene().name)
        {
            loadingText.SetActive(false);
            controlWindow.SetActive(true);
        }
        base.OnDisconnected(cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("OnJoinRandomFailed was activated: " + returnCode + message);

        //creates new room after he couldn't join one.
        PhotonNetwork.CreateRoom("GameRoom", new RoomOptions { MaxPlayers = maxPlayersInRoom });
        base.OnJoinRandomFailed(returnCode, message);
    }

    #endregion

}
