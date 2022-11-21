using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameLobbyManager : MonoBehaviourPunCallbacks
{
    public static GameLobbyManager gameLobbyInfo;

    public Text playerNameText;
    // Game lobby manager is Wanneer je in de game zit
    public void Start()
    {
        gameLobbyInfo = this;
        playerNameText.text = PhotonNetwork.NickName;
    }

    #region Automatic voids
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
        base.OnLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " has joined");

        if (PhotonNetwork.IsMasterClient)
        {
            print("OnPlayerEnteredRoom IsMasterClient:" + PhotonNetwork.IsMasterClient);
            LoadArena();
        }

        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " has leaved");
        if (PhotonNetwork.IsMasterClient)
        {
            print("OnPlayerEnteredRoom IsMasterClient:" + PhotonNetwork.IsMasterClient);
            LoadArena();
        }

        base.OnPlayerLeftRoom(otherPlayer);
    }
    #endregion

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            print("Player is not MasterClient");
            return;
        }
        print("Loading Level: " + PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
