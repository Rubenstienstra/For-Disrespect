using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameLobbyManager : MonoBehaviourPunCallbacks
{
    #region Automatic voids
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
        base.OnLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " has joined");
        if (PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }

        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " has leaved");

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
        if (PhotonNetwork.IsMasterClient)
        {
            print("Loading Level");
            PhotonNetwork.LoadLevel(2);
        }
        else
        {
            print("Player is not MasterClient");
        }
    }
}
