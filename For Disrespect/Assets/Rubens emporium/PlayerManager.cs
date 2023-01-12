using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject multiplayerDeletableMe;
    public GameObject multiplayerDeletableEnemy;

    public string crPlayerName;
    public string crEnemyName;

    public bool isReadyLobby;
    public float waitTimeAnimation = 2;
    public bool isWaitAnimationDone;
    public bool isReadyToFight;

    public Animator AllReadyUpAnimations;

    public GameLobbyManager crGameLobbyManager;
    public PhotonView photonID;
    public PlayerMovement playerMoving;


    public void Awake()
    {
        crPlayerName = PhotonNetwork.NickName;
        if (crGameLobbyManager == null)
        {
            crGameLobbyManager = GameObject.Find("GameManager").GetComponent<GameLobbyManager>();
        }
        AllReadyUpAnimations = GameObject.Find("Main Camera Lobby").GetComponent<Animator>();
    }
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //isHost = true;
            crGameLobbyManager.uiAnimation = crGameLobbyManager.hostUI.GetComponent<Animator>();
            if (photonID.IsMine)
            {
                crGameLobbyManager.hostUI.SetActive(true);
            }
        }
        else
        {
            //isGuest = true;
            crGameLobbyManager.uiAnimation = crGameLobbyManager.guestUI.GetComponent<Animator>();
            crGameLobbyManager.CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount - 1, true);

            if (photonID.IsMine)
            {
                crGameLobbyManager.guestUI.SetActive(true);
            }
        }
        crGameLobbyManager.uiAnimation.SetBool("BeforeCombat", true);
        crGameLobbyManager.camAnimation.SetBool("BeforeCombat", true);

        SendMessageUpwards("RecalculatePlacementReadyUpRoom", crGameLobbyManager, SendMessageOptions.DontRequireReceiver);

        if (photonID.IsMine)// Disables GameObjects for yourzelf
        {
            for (int i = 0; i < multiplayerDeletableMe.transform.childCount; i++)
            {
                print("Disabled: " + multiplayerDeletableMe.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                multiplayerDeletableMe.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else//Disables GameObjects for your enemy
        {
            for (int i = 0; i < multiplayerDeletableEnemy.transform.childCount; i++)
            {
                print("Disabled: " + multiplayerDeletableEnemy.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                multiplayerDeletableEnemy.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveEnemyNames()// Soms krijgt de speler de vijand zijn naam niet als hij terug joined.
    {
        GameObject crWorldSpaceNameLobbyEnemy = GameObject.Find("WORLDSPACECANVAS NameLobbyEnemy");
        GameObject crWorldSpaceNameEnemy = GameObject.Find("WORLDSPACECANVAS NameEnemy");

        if (photonID.IsMine && crGameLobbyManager.allPlayers.Count >= 2)
        {
            crEnemyName = crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().crPlayerName;
            crWorldSpaceNameLobbyEnemy.transform.GetChild(0).GetComponent<TMP_Text>().text = crEnemyName;
            crWorldSpaceNameEnemy.transform.GetChild(0).GetComponent<TMP_Text>().text = crEnemyName;
            print("giving enemy names");
        }
    }
    #region From Lobby To Game
    [PunRPC]
    public void LoadIntoGame()
    {
        AllReadyUpAnimations.SetBool("GameStart", true);

        //DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(crGameLobbyManager);
        DontDestroyOnLoad(crGameLobbyManager.allPlayers[0]);
        DontDestroyOnLoad(crGameLobbyManager.allPlayers[1]);

        StartCoroutine(WaitingReadyUpAnimation());
    }

    public IEnumerator WaitingReadyUpAnimation()
    {
        if(!isWaitAnimationDone)
        {
            isWaitAnimationDone = true;
            yield return new WaitForSeconds(waitTimeAnimation);
            print("Animation time is: " + isWaitAnimationDone);
        }

        if (SceneManager.GetActiveScene().name != "GameRoom" && PhotonNetwork.IsMasterClient)// Iedereen volgt de masterclient wanneer hij van scene veranderd. //PhotonNetwork.AutomaticallySyncScene = true;
        {
            PhotonNetwork.LoadLevel("GameRoom");
        }

        if(PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1)
        {
            print("Waiting Again. Progress: " + PhotonNetwork.LevelLoadingProgress);
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(WaitingReadyUpAnimation());
        }
        else
        {
            ArrivedAtGame();
        }
        yield return new WaitForSeconds(0);
    }
    public void ArrivedAtGame()
    {
        print("ArrivedAtGame Activated");
        crGameLobbyManager.transform.GetChild(0).gameObject.SetActive(false);
        
        
        if (PhotonNetwork.IsMasterClient)//Setting player position up
        {
            this.transform.position = crGameLobbyManager.playerFightSpawnLocation[0];
            crGameLobbyManager.allPlayers[1].transform.position = crGameLobbyManager.playerFightSpawnLocation[1];
        }
        else
        {
            this.transform.position = crGameLobbyManager.playerFightSpawnLocation[1];
            crGameLobbyManager.allPlayers[1].transform.position = crGameLobbyManager.playerFightSpawnLocation[0];
        }

        if (crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().isReadyToFight && isReadyToFight)
        {
            photonID.RPC("GameStarted", RpcTarget.All);
        }
    }
    [PunRPC]
    public void GameStarted()
    {
        playerMoving.allowMoving = true;
    }
    #endregion
    //spawnPoints = GameObject.Find("SpawnPoints");//Can't find spawnpoints if still in destroyOnLoad, Misschien destroyOnLoad when Instantiate.
    //this.gameObject.transform.SetParent(spawnPoints.transform.GetChild(0));
    //SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName("GameRoom"));

    //public void LeaveRoom()
    //{
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        PhotonNetwork.LeaveRoom();
    //    }
    //}
}
