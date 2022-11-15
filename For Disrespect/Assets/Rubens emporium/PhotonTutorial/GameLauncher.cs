using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    public string gameVersion = "1";
    public byte maxPlayersInRoom = 4;

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Start()
    {
        
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #endregion

    #region Callbacks Photon

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster was activated");
        base.OnConnectedToMaster();
    }
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom was activated: ");
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("OnDisconnected was activated: " + cause);
        base.OnDisconnected(cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("OnJoinRandomFailed was activated: " + returnCode + message);

        //creates new room after he couldn't join one.
        PhotonNetwork.CreateRoom("New Room", new RoomOptions { MaxPlayers = maxPlayersInRoom });
        base.OnJoinRandomFailed(returnCode, message);
    }

    #endregion

}
