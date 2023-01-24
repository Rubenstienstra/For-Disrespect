using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;// Can not be moved
    public Transform playerMovableCamera;
    public GameObject[] playerModels;

    public GameObject multiplayerDeletableMe;
    public GameObject multiplayerDeletableEnemy;

    public string crPlayerName;
    public string crEnemyName;

    public bool isHost;
    public bool isGuest;

    public bool isReadyLobby;
    public float waitTimeAnimation = 2;
    public bool alreadyLoadedLevel;
    public bool isReadyToFight;
    public bool hasStartedGame;

    public int damage = 15;
    public float hp = 100;
    public float stamina = 100;
    public float staminaRegenRate = 2;
    public float staminaCostAttack = 20;
    public float staminaCostBlock = 10;

    public BoxCollider playerAttackCollider;
    public CharacterController characterCon;
    public GameObject playerInAttackRange;

    public Animator AllReadyUpAnimations;
    public Animator playerAnimations;

    public string sceneNameToLoad = "BattlefieldCom";

    public GameLobbyManager crGameLobbyManager;
    public PhotonView photonID;
    public PlayerMovement playerMoving;
    public UIPlayer playerUI;


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
            crGameLobbyManager.uiAnimation = crGameLobbyManager.hostUI.GetComponent<Animator>();

            if (photonID.IsMine)
            {
                crGameLobbyManager.hostUI.SetActive(true);
                isHost = true;
            }
        }
        else
        {
            crGameLobbyManager.uiAnimation = crGameLobbyManager.guestUI.GetComponent<Animator>();
            crGameLobbyManager.CheckingPlayersInRoom(PhotonNetwork.CurrentRoom.PlayerCount - 1, true);

            if (photonID.IsMine)
            {
                crGameLobbyManager.guestUI.SetActive(true);
                isGuest = true;
            }
        }
        crGameLobbyManager.uiAnimation.SetBool("BeforeCombat", true);
        crGameLobbyManager.camAnimation.SetBool("BeforeCombat", true);

        if (isHost)
        {
            playerModels[0].SetActive(true);
            playerAnimations = playerModels[0].GetComponent<Animator>();
            playerModels[1].SetActive(false)                                                                                                                                                                                                                                                    ;
        }
        else if (isGuest)
        {
            playerModels[1].SetActive(true);
            playerAnimations = playerModels[1].GetComponent<Animator>();
            playerModels[0].SetActive(false);
        }

        if (photonID.IsMine)
        {
            for (int i = 0; i < multiplayerDeletableMe.transform.childCount; i++)// Disables GameObjects for yourzelf
            {
                print("Disabled: " + multiplayerDeletableMe.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                if (multiplayerDeletableMe.transform.GetChild(i).gameObject.GetComponent<AudioListener>())
                {
                    multiplayerDeletableMe.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = !enabled;
                }
                multiplayerDeletableMe.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < multiplayerDeletableEnemy.transform.childCount; i++)//Disables GameObjects for your enemy
            {
                print("Disabled: " + multiplayerDeletableEnemy.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                if (multiplayerDeletableEnemy.transform.GetChild(i).gameObject.GetComponent<AudioListener>())
                {
                    multiplayerDeletableEnemy.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = !enabled;
                }
                multiplayerDeletableEnemy.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void GiveEnemyNamesAndUI()// Soms krijgt de speler de vijand zijn naam niet als hij terug joined.
    {
        if (photonID.IsMine && crGameLobbyManager.allPlayers.Count >= 2)
        {
            if (GameObject.Find("HPbarEnemy") && GameObject.Find("EnemyStamina").GetComponent<Image>() && GameObject.Find("EnemyHPBehindFall").GetComponent<Image>() && GameObject.Find("EnemyHP").GetComponent<Image>())
            {
                playerUI.enemyWorldSpaceUI = GameObject.Find("HPbarEnemy");
                playerUI.enemyStaminaBar = GameObject.Find("EnemyStamina").GetComponent<Image>();
                playerUI.enemyFallBehindHPBar = GameObject.Find("EnemyHPBehindFall").GetComponent<Image>();
                playerUI.enemyHPBar = GameObject.Find("EnemyHP").GetComponent<Image>();

                crEnemyName = crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().crPlayerName;
                playerUI.enemyWorldSpaceUI.transform.GetChild(0).GetComponent<TMP_Text>().text = crEnemyName;
                print("giving enemy names");
            }
            if (GameObject.Find("WORLDSPACECANVAS NameLobbyEnemy"))
            {
                GameObject crWorldSpaceNameLobbyEnemy = GameObject.Find("WORLDSPACECANVAS NameLobbyEnemy");

                crWorldSpaceNameLobbyEnemy.transform.GetChild(0).GetComponent<TMP_Text>().text = crEnemyName;
            }
        }
        
    }
    #region DoingAndBlockingDamage
    public void DealtBlockedDamage(GameObject enemyPlayer)
    {
        print("you've blocked: " + damage + " damage.");

        photonID.RPC("OnReceiveShieldedDamage", RpcTarget.Others);
    }

    public void SuccesfullyDealtDamage()//player 0 did damage to player 1
    {
        print("you've dealt: " + damage + " damage.");
        crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().hp -= damage;
        
        playerUI.OnHealthChange(playerUI.enemyHPBar, playerUI.enemyFallBehindHPBar, false);
        photonID.RPC("OnReceiveDamage", RpcTarget.Others);
    }
    [PunRPC]
    public void OnReceiveDamage()// Only player 1 gets this
    {
        playerUI.OnHealthChange(playerUI.playerHPBar, playerUI.playerFallBehindHPBar, true);
        playerAnimations.SetTrigger("Get Hit");
    }

    [PunRPC]
    public void OnReceiveShieldedDamage()
    {
        playerAnimations.SetTrigger("Block");
        hp -= damage;
    }
    #endregion

    #region From Lobby To Game
    [PunRPC]
    public void LoadIntoGame()
    {
        print("STEP 0" + PhotonNetwork.NickName);
        AllReadyUpAnimations.SetBool("GameStart", true);
        if (isHost)
        {
            crGameLobbyManager.hostUI.GetComponent<Animator>().SetBool("GameStart", true);
        }
        else
        {
            crGameLobbyManager.guestUI.GetComponent<Animator>().SetBool("GameStart", true);
        }

        DontDestroyOnLoad(crGameLobbyManager);
        DontDestroyOnLoad(crGameLobbyManager.allPlayers[0]);
        DontDestroyOnLoad(crGameLobbyManager.allPlayers[1]);

        StartCoroutine(WaitingReadyUpAnimation());
    }

    public IEnumerator WaitingReadyUpAnimation()
    {
        yield return new WaitForSeconds(waitTimeAnimation);

        playerUI.playerRoundStartScreen.gameObject.SetActive(true);
        print("STEP 1" + PhotonNetwork.NickName);

        if (PhotonNetwork.IsMasterClient)// Iedereen volgt de masterclient wanneer hij van scene veranderd. //PhotonNetwork.AutomaticallySyncScene = true;
        {
            if (!alreadyLoadedLevel)
            {
                alreadyLoadedLevel = true;
                PhotonNetwork.LoadLevel(sceneNameToLoad);
            }
        }

        yield return new WaitForSeconds(0);
    }
    public void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == sceneNameToLoad)
        {
            ArrivedAtGame();
            print("Level Loaded!");
        }
    }
    public void ArrivedAtGame()
    {
        Destroy(GameObject.Find("MainMenuLobbyMusic"));
        isReadyToFight = true;
        playerUI.playerCanvas.SetActive(true);

        playerUI.playerRoundStartScreen.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        crGameLobbyManager.transform.GetChild(0).gameObject.SetActive(false);


        if (PhotonNetwork.IsMasterClient)//Setting player position up
        {
            crGameLobbyManager.allPlayers[0].transform.position = crGameLobbyManager.playerFightSpawnLocation[0];
            crGameLobbyManager.allPlayers[1].transform.position = crGameLobbyManager.playerFightSpawnLocation[1];
        }
        else
        {
            crGameLobbyManager.allPlayers[0].transform.position = crGameLobbyManager.playerFightSpawnLocation[1];
            crGameLobbyManager.allPlayers[1].transform.position = crGameLobbyManager.playerFightSpawnLocation[0];
        }

        if (crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().isReadyToFight && isReadyToFight)
        {
            photonID.RPC("CountDownGame", RpcTarget.All);
            print("Activated CountDownGame");
            return;
        }
    }
    [PunRPC]
    public IEnumerator CountDownGame()
    {
        playerUI.roundCountdownStartAnimation.SetTrigger("RoundCounter");
        yield return new WaitForSeconds(1);//2 seconds left

        yield return new WaitForSeconds(1);//1 seconds left

        yield return new WaitForSeconds(1);//0 seconds left
        GameStarted();
    }
    public void GameStarted()
    {
        if (photonID.IsMine)
        {
            print("GameStarted activated");

            hasStartedGame = true;
            playerMoving.allowMoving = true;
            Cursor.lockState = CursorLockMode.Locked;

            if (!crGameLobbyManager.allPlayers[1].GetComponent<PlayerMovement>().allowMoving)
            {
                print("Other player didn't get: allowMoving");
                crGameLobbyManager.allPlayers[1].GetComponent<PlayerMovement>().allowMoving = true;
            }
        }
    }
    #endregion

    #region OnTriggerEnter/Exit voids
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && isReadyToFight)
        {
            if (!playerInAttackRange)
            {
                print(other.gameObject.name + "is in attack range");
            }
            playerInAttackRange = (other.gameObject);
        }
    }
    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" && isReadyToFight)
    //    {
    //        playerInAttackRange =(other.gameObject);
    //        print(other.gameObject.name + "is in attack range");
    //    }
    //}
    //public void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.tag == "Player" && isReadyToFight)
    //    {
    //        if (playerInAttackRange)
    //        {
    //            playerInAttackRange = null;
    //        }
    //        print(other.gameObject.name + "is out of attack range");
    //    }
    //}
    #endregion

    #region UIButtonsVoids
    public void LeaveRoomButton()//Button Leave room
    {
        crGameLobbyManager.LeaveRoom();
    }
    #endregion
}
