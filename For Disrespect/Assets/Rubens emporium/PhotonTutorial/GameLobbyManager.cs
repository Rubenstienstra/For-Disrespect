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
    public List<GameObject> allPlayers;
    public PlayerMovement crInstantietedPlayerMovement;
    public Vector3[] PlayerSpawnLocations;

    public GameObject team0ParentForPlayer;
    public GameObject team1ParentForPlayer;
    public GameObject[] playerDummyGameObjects;
    
    public int minimumRequiredPlayers = 2;

    public GameObject hostUI;
    public TMP_Text hostUIRoomName;
    public GameObject guestUI;
    public TMP_Text guestUIRoomName;
    public TMP_Text worldSpaceNameEnemy;

    public Animator uiAnimation;
    public Animator camAnimation;

    // Game lobby manager is Wanneer je in de wacht ruimte zit. Elke speler heeft zijn eigen GameLobbyManager.
    public void Start()
    {
        gameLobbyInfo = this;

        if (camAnimation == null)
        {
            GameObject crCameraGameobject = GameObject.Find("Main Camera");
            camAnimation = crCameraGameobject.GetComponent<Animator>();
        }
        hostUIRoomName.text = PhotonNetwork.CurrentRoom.Name;
        guestUIRoomName.text = PhotonNetwork.CurrentRoom.Name;

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

        //if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount, true);
        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " has leaved. Total players: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (allPlayers.Count >= 1)
        {
            allPlayers.RemoveAt(allPlayers.Count - 1);
        }


        //if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount, false);
        
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
    
    public void CheckingPlayersInRoom(int totalPlayers, bool enableOrDisable)
    {
        playerDummyGameObjects[1].SetActive(enableOrDisable);
    }

    public void SpawnPlayer()
    {
        print("Spawned a player in: " + SceneManager.GetActiveScene().name);
        crInstantiatedPlayerPrefab = null;
        crInstantiatedPlayerPrefab = PhotonNetwork.Instantiate(playerSpawnPrefab.name, new Vector3(10,0,10), Quaternion.identity);
        crInstantietedPlayerMovement = crInstantiatedPlayerPrefab.GetComponent<PlayerMovement>();
        crInstantietedPlayerMovement.crGameLobbyManager = this;
        crInstantietedPlayerMovement.allowMoving = false;
        crInstantietedPlayerMovement.UIPrefab.SetActive(false);
        crInstantietedPlayerMovement.cameraPlayer.SetActive(false);
        crInstantietedPlayerMovement.playerID = PhotonNetwork.CurrentRoom.PlayerCount -1; // -1 so player 1 has PlayerID 0.

        //CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount, true);
       
    }
    public void RecalculatePlacementReadyUpRoom()// heb ik niet nodig als de max 2 spelers zijn.
    {

        for (int i = 0; i < allPlayers.Count; i++)
        {
            print("Is Recalculating for Player: " + crInstantietedPlayerMovement.playerID);
            //als even is, wordt het 0 en als het getal oneven is is het 1.
            if (crInstantietedPlayerMovement.playerID % 2 == 0)//Team0
            {
                crInstantiatedPlayerPrefab.transform.parent = team0ParentForPlayer.transform;
                crInstantiatedPlayerPrefab.transform.rotation = Quaternion.Euler(0, 180, 0);

                print("Player Number: " + crInstantietedPlayerMovement.playerID.ToString() + "Has Joined team: " + crInstantietedPlayerMovement.playerID % 2);
            }
            else if (crInstantietedPlayerMovement.playerID % 2 == 1)//Team1
            {
                crInstantiatedPlayerPrefab.transform.parent = team1ParentForPlayer.transform;

                print("Player Number: " + crInstantietedPlayerMovement.playerID.ToString() + ".Has Joined team: " + crInstantietedPlayerMovement.playerID % 2);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.tag == "Player")
        {
            allPlayers.Add(other.gameObject);
            allPlayers[0].GetComponent<PlayerMovement>().GiveEnemyNames();
            if(allPlayers.Count >= 2)
            {
                allPlayers[1].GetComponent<PlayerMovement>().GiveEnemyNames();
            }
            print(other.gameObject.name + "Has entered the triggerZone");
            
        }
        
    }
}
