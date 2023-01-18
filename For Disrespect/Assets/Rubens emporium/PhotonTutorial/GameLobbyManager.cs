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
    public Vector3[] playerFightSpawnLocation;

    public GameObject team0ParentForPlayer;
    public GameObject team1ParentForPlayer;
    public GameObject[] playerDummyGameObjects;

    public int totalRounds;
    public int MaxTotalRounds;
    public Button lessRoundsButton;
    public Button moreRoundsButton;
    public TMP_Text HostTotalRoundsUI;
    public TMP_Text GuestTotalRoundsUI;

    public GameObject hostUI;
    public GameObject hostUISettings;
    public TMP_Text hostUIRoomName;



    public GameObject guestUI;
    public GameObject guestUISettings;
    public TMP_Text guestUIRoomName;

    public TMP_Text worldSpaceNameEnemy;

    public Animator uiAnimation;
    public Animator camAnimation;

    public PlayerMovement crInstantietedPlayerMovement;
    public PlayerManager crInstantietedPlayerManager;

    // Game lobby manager is Wanneer je in de wacht ruimte zit. Elke speler heeft zijn eigen GameLobbyManager.
    public void Start()
    {
        gameLobbyInfo = this;

        if (camAnimation == null)
        {
            camAnimation = GameObject.Find("Main Camera").GetComponent<Animator>();
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

        CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount, true);
        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " has leaved. Total players: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (guestUI.activeInHierarchy)
        {
            guestUI.SetActive(false);
            hostUI.SetActive(true);
        }
        else if(guestUISettings.activeInHierarchy)
        {
            guestUISettings.SetActive(false);
            hostUISettings.SetActive(true);
        }

        worldSpaceNameEnemy.text = "";
        if (allPlayers.Count >= 1)
        {
            allPlayers.RemoveAt(allPlayers.Count - 1);
        }

        CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount, false);
        
        base.OnPlayerLeftRoom(otherPlayer);
    }
    #endregion

    public void LeaveRoom()//Button Leave room
    {
        if (PhotonNetwork.IsConnected)
        {
            uiAnimation.SetBool("BeforeCombat", false); camAnimation.SetBool("BeforeCombat", false);
            worldSpaceNameEnemy.text = "";
            Destroy(photonView);
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
        print("Loading World, PlayerName: " + PhotonNetwork.NickName);
        PhotonNetwork.LoadLevel("GameRoom");
    }
    
    public void CheckingPlayersInRoom(int totalPlayers, bool enableOrDisable)
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            playerDummyGameObjects[1].SetActive(enableOrDisable);
        }
    }

    public void SpawnPlayer()
    {
        print("Spawned a player in: " + SceneManager.GetActiveScene().name);
        crInstantiatedPlayerPrefab = null;
        crInstantiatedPlayerPrefab = PhotonNetwork.Instantiate(playerSpawnPrefab.name, new Vector3(10,0,10), Quaternion.identity);
        crInstantietedPlayerMovement = crInstantiatedPlayerPrefab.GetComponent<PlayerMovement>();
        crInstantietedPlayerManager = crInstantiatedPlayerPrefab.GetComponent<PlayerManager>();
        
        crInstantietedPlayerMovement.allowMoving = false;
        crInstantietedPlayerMovement.UIPrefab.SetActive(false);
        crInstantietedPlayerMovement.playerID = PhotonNetwork.CurrentRoom.PlayerCount -1; // -1 so player 1 has PlayerID 0.
    }
    #region Unused Code
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
    #endregion  
    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.tag == "Player")
        {
            allPlayers.Add(other.gameObject);
            print(other.gameObject.name + "Has entered the triggerZone");
            StartCoroutine(WaitBeforeGivingNames());
        }
    }
    public IEnumerator WaitBeforeGivingNames()
    {
        yield return new WaitForSeconds(0.5f);//Het heeft processing tijd nodig.
        allPlayers[0].GetComponent<PlayerManager>().GiveEnemyNamesAndModels();//heeft genoeg aan de eerste spelers[0], speler 0 heeft speler 1 || speler 1 heeft speler 0.
    }
    #region ReadyUpCode
    public void ReadyUpHostUI(bool readyOrUnready)
    {
        allPlayers[0].GetComponent<PlayerManager>().isReadyLobby = readyOrUnready;

        if(allPlayers.Count <= 1)
        {
            return;
        }
        if(allPlayers[0].GetComponent<PlayerManager>().isReadyLobby && allPlayers[1].GetComponent<PlayerManager>().isReadyLobby)
        {
            EveryoneIsReady();
        }
    }
    public void ReadyUpGuestUI(bool readyOrUnready)
    {
        allPlayers[0].GetComponent<PlayerManager>().isReadyLobby = readyOrUnready;
        print("Player is: " + readyOrUnready);

        if(allPlayers.Count <= 1)
        {
            return;
        }
        if(allPlayers[0].GetComponent<PlayerManager>().isReadyLobby && allPlayers[1].GetComponent<PlayerManager>().isReadyLobby)
        {
            EveryoneIsReady();
        }
    }
    public void EveryoneIsReady()
    {
        print("STEP 00");
        allPlayers[1].GetComponent<PhotonView>().RPC("LoadIntoGame", RpcTarget.All); //Dit moet op het einde gebeuren. De RPC kan niet voor andere worden geladen, omdat je naar een andere scene toe gaat en daardoor verlies je connections met de andere spelers.
        allPlayers[0].GetComponent<PhotonView>().RPC("LoadIntoGame", RpcTarget.All); //Het resultaat hiervan is dat alle bools, floats, ints niet meer worden ingeladen voor de speler zelf en ook niet voor de andere.
    }
    #endregion

    #region voidsForUIButtons
    public void ChangeTotalRounds(int addNumber)
    {
        if(totalRounds > 1 && totalRounds < MaxTotalRounds)
        {
            totalRounds += addNumber;

            lessRoundsButton.interactable = true;
            moreRoundsButton.interactable = true;
        }

        else if (totalRounds >= MaxTotalRounds)
        {
            moreRoundsButton.interactable = false;
        }
        else if (totalRounds <= 1)
        {
            lessRoundsButton.interactable = false;
        }

        HostTotalRoundsUI.text = totalRounds.ToString();
        GuestTotalRoundsUI.text = totalRounds.ToString();

        print("Changed Number to: " + totalRounds);
    }

    #endregion
}
