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

    public GameObject playerSpawnPrefab;
    public Vector3 spawnLocation;

    // Game lobby manager is Wanneer je in de game zit
    public void Start()
    {
        gameLobbyInfo = this;

        if (PhotonNetwork.IsConnected)
        {
            if (PlayerMovement.thisPlayerPrefab == null)
            {
                print("Spawned in a player " + Application.loadedLevelName);

                PhotonNetwork.Instantiate(playerSpawnPrefab.name, spawnLocation, Quaternion.identity, 0);
            }
            
        }
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
            return;
        }
        SceneManager.LoadScene("Launcher");
    }

    public void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            print("Player is not MasterClient");
            return;
        }
        print("Loading World, PlayerName: " + PhotonNetwork.NickName);
        PhotonNetwork.LoadLevel("GameRoom");
    }
}
