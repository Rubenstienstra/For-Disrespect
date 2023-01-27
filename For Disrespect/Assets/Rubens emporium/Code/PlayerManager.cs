using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Chat;
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
    public float syncedHP = 100;
    public float stamina = 100;
    public float staminaRegenRate = 4;
    public float staminaCostAttack = 20;
    public float staminaCostBlock = 10;

    #region PlayerSounds 
    public AudioSource soundOnHitEnemy; //OnLose en OnWin sounds worden automatisch afgespeeld.
    public AudioSource soundOnBlockEnemy;
    public AudioSource soundOnMissEnemy;

    public AudioSource soundLobbyEveryoneReady;
    #endregion

    public BoxCollider playerAttackCollider;
    public CharacterController characterCon;
    public GameObject playerInAttackRange;

    public Animator AllReadyUpAnimations;
    public Animator playerAnimations;

    public bool theGameEnded;
    public int waitTimeBeforeKick = 10;

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
        else
        {
            playerModels[1].SetActive(true);
            playerAnimations = playerModels[1].GetComponent<Animator>();
            playerModels[0].SetActive(false);
        }

        if (photonID.IsMine)
        {
            for (int i = 0; i < multiplayerDeletableMe.transform.childCount; i++)// Disables GameObjects for yourzelf
            {
                //print("Disabled: " + multiplayerDeletableMe.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
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
                //print("Disabled: " + multiplayerDeletableEnemy.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                if (multiplayerDeletableEnemy.transform.GetChild(i).gameObject.GetComponent<AudioListener>())
                {
                    multiplayerDeletableEnemy.transform.GetChild(i).gameObject.GetComponent<AudioListener>().enabled = !enabled;
                }
                multiplayerDeletableEnemy.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        print("Disabled all Gameobjects");
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
            print("Player has found: HPbarEnemy = " + playerUI.enemyWorldSpaceUI + ", EnemyStamina = " + playerUI.enemyStaminaBar + ", EnemyHPBehindFall = " + playerUI.enemyFallBehindHPBar + ", EnemyHP = " + playerUI.enemyHPBar);
        }
    }
    #region DoingAndBlockingDamage
    public void DealtBlockedDamage()
    {
        print("you've blocked: " + damage + " damage.");

        photonID.RPC("OnReceiveShieldedDamage", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public void SuccesfullyDealtDamage()//player 0 did damage to player 1
    {
        if (photonID.IsMine)
        {
            print("you've dealt: " + damage + " damage.");

            photonID.RPC("OnReceiveDamage", RpcTarget.All, PhotonNetwork.LocalPlayer); //Sends info on which player should not get attacked.
        }
    }
    [PunRPC]
    public void OnReceiveDamage(Player playerWhoSended)// Only player 1 gets this
    {
        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().soundOnHitEnemy.Play();
        if (playerWhoSended.UserId == PhotonNetwork.LocalPlayer.UserId)//If the info matches with the attacker don't get the damage.
        {
            return;
        }

        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().hp -= damage;
        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().playerAnimations.SetTrigger("Get Hit");
        crGameLobbyManager.allPlayers[0].GetComponent<UIPlayer>().bloodDamageEffect.SetFloat("Blood", (-crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().hp +100) /100 );

        print(PhotonNetwork.LocalPlayer.UserId + "Got hit by enemy! Player who sended the attack: " + playerWhoSended.UserId);

        if (crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().hp <= 0)
        {
            photonID.RPC("OnGameEnded", RpcTarget.All);
        }
        //playerUI.OnPlayerHealthChange();
    }

    [PunRPC]
    public void OnReceiveShieldedDamage(Player playerWhoSended)
    {
        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().soundOnBlockEnemy.Play();
        if (playerWhoSended.UserId == PhotonNetwork.LocalPlayer.UserId)//If the info matches with the attacker don't get the damage.
        {
            return;
        }

        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().playerAnimations.SetTrigger("Block");
        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().stamina -= staminaCostBlock;
        crGameLobbyManager.allPlayers[0].GetComponent<PlayerMovement>().isBlocking = false;

        print(PhotonNetwork.LocalPlayer.UserId + " Blocked the enemy attack! Player who sended the attack: " + playerWhoSended.UserId);
    }
    #endregion

    #region From Lobby To Game
    [PunRPC]
    public void LoadIntoGame()//Je kan niet makkelijk veriables veranderen, je moet crGameLobbyManager.allPlayers gebruiken en dan jezelf pakken. Geen idee waarom het zo werkt.
    {
        AllReadyUpAnimations.SetBool("GameStart", true);
        Destroy(GameObject.Find("MainMenuLobbyMusic"));

        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().soundLobbyEveryoneReady.Play();

        if (PhotonNetwork.IsMasterClient)
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
        isReadyToFight = true;
        playerUI.playerCanvas.SetActive(true);

        playerCamera.gameObject.SetActive(true);
        crGameLobbyManager.transform.GetChild(0).gameObject.SetActive(false);


        if (PhotonNetwork.IsMasterClient)//Setting player position up
        {
            crGameLobbyManager.allPlayers[0].transform.position = crGameLobbyManager.playerFightSpawnLocation[0]; crGameLobbyManager.allPlayers[0].transform.rotation = Quaternion.Euler(crGameLobbyManager.playerFightSpawnRotation[0]);
            crGameLobbyManager.allPlayers[1].transform.position = crGameLobbyManager.playerFightSpawnLocation[1]; crGameLobbyManager.allPlayers[1].transform.rotation = Quaternion.Euler(crGameLobbyManager.playerFightSpawnRotation[1]);
        }
        else
        {
            crGameLobbyManager.allPlayers[0].transform.position = crGameLobbyManager.playerFightSpawnLocation[1]; crGameLobbyManager.allPlayers[0].transform.rotation = Quaternion.Euler(crGameLobbyManager.playerFightSpawnRotation[1]);
            crGameLobbyManager.allPlayers[1].transform.position = crGameLobbyManager.playerFightSpawnLocation[0]; crGameLobbyManager.allPlayers[1].transform.rotation = Quaternion.Euler(crGameLobbyManager.playerFightSpawnRotation[0]);
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
        playerUI.roundCountdownStartAnimation.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);//2 seconds left

        yield return new WaitForSeconds(1);//1 seconds left

        yield return new WaitForSeconds(1);//0 seconds left
        playerUI.loadingScreen.SetActive(false);
        playerUI.playerRoundStartScreen.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        playerUI.playerRoundStartScreen.transform.parent.gameObject.SetActive(false);
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

    #region OnTriggerStay voids
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
    #endregion

    #region OnWinning/Losing

    [PunRPC]
    public void OnGameEnded()
    {
        ResetAnimations();
        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().theGameEnded = true;
        if(crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().hp <= 0)
        {
            OnLose();
            print("You Lost...");
            return;
        }
        OnWin();
        print("You've Won!");
    }
    public void ResetAnimations()
    {
        if (playerAnimations)
        {
            playerAnimations.SetFloat("Vertical", 0); playerAnimations.SetFloat("Horizontal", 0); playerAnimations.SetBool("Running", false);
        }
    }

    public void OnWin()
    {
        playerUI.winScreen.SetActive(true);

        crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().playerAnimations.SetTrigger("Dead");// Voor als de syncing niet goed werkt.

        StartCoroutine(CountDownEndGame());
    }

    public void OnLose()
    {
        playerUI.loseScreen.SetActive(true);

        crGameLobbyManager.allPlayers[0].GetComponent<PlayerManager>().playerAnimations.SetTrigger("Dead");

        StartCoroutine(CountDownEndGame());
    }

    public IEnumerator CountDownEndGame()
    {
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(waitTimeBeforeKick);

        crGameLobbyManager.LeaveRoom();

        yield return new WaitForSeconds(0);
    }

    #endregion

    #region UIButtonsVoids
    public void LeaveRoomButton()//Button Leave room
    {
        crGameLobbyManager.LeaveRoom();
    }
    #endregion
}
