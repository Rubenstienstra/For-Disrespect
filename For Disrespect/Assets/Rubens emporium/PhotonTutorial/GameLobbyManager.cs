using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameLobbyManager : MonoBehaviourPunCallbacks
{
    public static GameLobbyManager gameLobbyInfo;

    public GameObject playerSpawnPrefab;
    public GameObject crInstantiatedPlayerPrefab;
    public TMP_Text playerNameText;
    public Vector3 spawnLocation;

    public int minimumRequiredPlayers;
    public bool isHost;

    public GameObject hostUI;
    public GameObject guestUI;

    // Game lobby manager is Wanneer je in de game zit
    public void Start()
    {
        gameLobbyInfo = this;

        if (PhotonNetwork.IsConnected)
        {
            if (PlayerMovement.thisPlayerPrefab == null)
            {
                //print("Spawned a player in: " + Application.loadedLevelName);
                //PhotonNetwork.Instantiate(playerSpawnPrefab.name, spawnLocation, Quaternion.identity, 0);
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                isHost = true;
                hostUI.SetActive(true);
            }
            else
            {
                guestUI.SetActive(true);
            } 
        }
    }
    

    #region Automatic voids
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
        base.OnLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " has joined. Total players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        SpawnPlayer();

        if (PhotonNetwork.CurrentRoom.PlayerCount >= minimumRequiredPlayers)
        {
            EnoughPlayers();
        }

        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " has leaved. Total players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        
        if (PhotonNetwork.CurrentRoom.PlayerCount < minimumRequiredPlayers)
        {
            NotEnoughPlayers();
        }

        base.OnPlayerLeftRoom(otherPlayer);
    }
    #endregion

    public void LeaveRoom()//Button Leave room
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }
        SceneManager.LoadScene("MainMenu");
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
    public void EnoughPlayers()
    {
        
    }
    public void NotEnoughPlayers()
    {

    }
    public void SpawnPlayer()
    {
        print("Spawned a player in: " + Application.loadedLevelName);
        crInstantiatedPlayerPrefab = PhotonNetwork.Instantiate(playerSpawnPrefab.name, spawnLocation, Quaternion.identity);
        
    }
}
