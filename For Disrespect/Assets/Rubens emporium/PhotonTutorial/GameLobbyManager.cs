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
    public PlayerMovement crInstantietedPlayerMovement;
    public Vector3[] spawnLocations;
    public bool doneMakingPlayer = true;

    public GameObject team0ParentForPlayer;
    public GameObject team1ParentForPlayer;
    
    public int minimumRequiredPlayers;
    public bool isHost;

    public GameObject hostUI;
    public GameObject guestUI;

    public Animator uiAnimation;
    public Animator camAnimation;

    // Game lobby manager is Wanneer je in de wacht ruimte zit. Elke speler heeft zijn eigen GameLobbyManager.
    public void Start()
    {
        gameLobbyInfo = this;

        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
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
            uiAnimation.SetBool("BeforeCombat", false); camAnimation.SetBool("BeforeCombat", false);
            Destroy(photonView);
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
    public void RecalculatePlayers()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            //SpawnPlayer();
            print("Needs extra player? (used to spawn player)");
        }
    }
    public void EnoughPlayers()
    {
        
    }
    public void NotEnoughPlayers()
    {

    }
    public void SpawnPlayer()
    {
        if (!doneMakingPlayer)
        {
            new WaitForSeconds(0.1f);
            SpawnPlayer();
            return;
        }

        print("Spawned a player in: " + SceneManager.GetActiveScene());
        crInstantiatedPlayerPrefab = PhotonNetwork.Instantiate(playerSpawnPrefab.name, spawnLocations[PhotonNetwork.CurrentRoom.PlayerCount -1], Quaternion.identity);
        crInstantietedPlayerMovement = crInstantiatedPlayerPrefab.GetComponent<PlayerMovement>();
        crInstantietedPlayerMovement.crGameLobbyManager = this;
        crInstantietedPlayerMovement.allowMoving = false;
        crInstantietedPlayerMovement.UIPrefab.SetActive(false);
        crInstantietedPlayerMovement.cameraPlayer.SetActive(false);
        crInstantietedPlayerMovement.playerID = PhotonNetwork.CurrentRoom.PlayerCount; // -1 so player 1 has PlayerID 0.

        //als even is, wordt het 0 en als het getal oneven is is het 1.
        if (crInstantietedPlayerMovement.playerID %2 == 0)//Team0
        {
            crInstantiatedPlayerPrefab.transform.parent = team0ParentForPlayer.transform;
            crInstantiatedPlayerPrefab.transform.rotation = Quaternion.Euler(0, 180, 0);

            print("Player Number: " + crInstantietedPlayerMovement.playerID.ToString() + "Has Joined team: " + crInstantietedPlayerMovement.playerID % 2);
        }
        else if(crInstantietedPlayerMovement.playerID % 2 == 1)//Team0
        {
            crInstantiatedPlayerPrefab.transform.parent = team1ParentForPlayer.transform;

            print("Player Number: " + crInstantietedPlayerMovement.playerID.ToString() + ".Has Joined team: " + crInstantietedPlayerMovement.playerID % 2);
        }

        crInstantiatedPlayerPrefab = null;
        doneMakingPlayer = true;
    }
}
