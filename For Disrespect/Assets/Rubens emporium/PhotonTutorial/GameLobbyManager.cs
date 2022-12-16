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
    public Vector3[] spawnLocations;
    public TMP_Text playerNameText;

    public bool toggleEnemyOrFriendly;
    public GameObject enemyParentForPlayer;
    public GameObject friendlyParentForPlayer;
    
    public int minimumRequiredPlayers;
    public bool isHost;

    public GameObject hostUI;
    public GameObject guestUI;

    private Animator uiAnimation;

    // Game lobby manager is Wanneer je in de wacht ruimte zit. Elke speler heeft zijn eigen GameLobbyManager.
    public void Start()
    {
        gameLobbyInfo = this;

        if (PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == 0)
            {
                isHost = true;
                hostUI.SetActive(true);
                uiAnimation = hostUI.GetComponent<Animator>();
            }
            else
            {
                guestUI.SetActive(true);
                uiAnimation = guestUI.GetComponent<Animator>();
            }
            SpawnPlayer();
            uiAnimation.SetBool("BeforeCombat", true);
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
            uiAnimation.SetBool("BeforeCombat", false);
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
            SpawnPlayer();
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
        print("Spawned a player in: " + Application.loadedLevelName);
        crInstantiatedPlayerPrefab = PhotonNetwork.Instantiate(playerSpawnPrefab.name, spawnLocations[PhotonNetwork.CurrentRoom.PlayerCount], Quaternion.identity); print(spawnLocations[PhotonNetwork.CurrentRoom.PlayerCount]);
        crInstantiatedPlayerPrefab.transform.GetChild(0).gameObject.SetActive(false);

        if(toggleEnemyOrFriendly)
        {
            crInstantiatedPlayerPrefab.transform.parent = enemyParentForPlayer.transform;
            toggleEnemyOrFriendly = false;
        }
        else
        {
            crInstantiatedPlayerPrefab.transform.parent = friendlyParentForPlayer.transform;
            toggleEnemyOrFriendly = true;
        }
    }
}
