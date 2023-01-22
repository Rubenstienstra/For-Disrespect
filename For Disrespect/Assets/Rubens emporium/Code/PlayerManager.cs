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

    public Vector2 crScreenSize;

    public GameObject multiplayerDeletableMe;
    public GameObject multiplayerDeletableEnemy;

    public string crPlayerName;
    public string crEnemyName;

    public bool isHost;
    public bool isGuest;

    public GameObject worldSpaceEnemyUIBar;

    public bool isReadyLobby;
    public float waitTimeAnimation = 2;
    public bool isWaitAnimationDone;
    public bool isReadyToFight;

    public int damage = 5;
    public int hp = 100;
    public float stamina = 100;
    public float staminaRegenRate = 1;
    public float staminaCostAttack = 20;
    public float staminaCostBlock = 30;

    public List<GameObject> playersInAttackRange;

    public Animator AllReadyUpAnimations;
    public Animator playerAnimations;

    public GameObject UIPrefab;
    public GameObject playerESCMenu;

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
            playerModels[1].SetActive(false);
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
                multiplayerDeletableMe.transform.GetChild(i).gameObject.SetActive(false);
            }
            crScreenSize.x = Display.main.systemWidth;
            crScreenSize.y = Display.main.systemHeight;

        }
        else
        {
            for (int i = 0; i < multiplayerDeletableEnemy.transform.childCount; i++)//Disables GameObjects for your enemy
            {
                print("Disabled: " + multiplayerDeletableEnemy.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                multiplayerDeletableEnemy.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void GiveEnemyNamesAndModels()// Soms krijgt de speler de vijand zijn naam niet als hij terug joined.
    {
        if (photonID.IsMine && crGameLobbyManager.allPlayers.Count >= 2)
        {
            if (GameObject.Find("HPbarEnemy") && GameObject.Find("EnemyStamina").GetComponent<Image>() && GameObject.Find("EnemyHPBehindFall").GetComponent<Image>() && GameObject.Find("EnemyHP").GetComponent<Image>())
            {
                worldSpaceEnemyUIBar = GameObject.Find("HPbarEnemy");
                playerUI.enemyStaminaBar = GameObject.Find("EnemyStamina").GetComponent<Image>();
                playerUI.enemyFallBehindHPBar = GameObject.Find("EnemyHPBehindFall").GetComponent<Image>();
                playerUI.enemyHPBar = GameObject.Find("EnemyHP").GetComponent<Image>();

                crEnemyName = crGameLobbyManager.allPlayers[1].GetComponent<PlayerManager>().crPlayerName;
                worldSpaceEnemyUIBar.transform.GetChild(0).GetComponent<TMP_Text>().text = crEnemyName;
                print("giving enemy names");
            }
            if (GameObject.Find("WORLDSPACECANVAS NameLobbyEnemy"))
            {
                GameObject crWorldSpaceNameLobbyEnemy = GameObject.Find("WORLDSPACECANVAS NameLobbyEnemy");

                crWorldSpaceNameLobbyEnemy.transform.GetChild(0).GetComponent<TMP_Text>().text = crEnemyName;
            }
        }
    }
    public void DealtBlockedDamage(GameObject enemyPlayer)
    {
        print("you've blocked: " + damage + " damage.");

        photonID.RPC("OnReceiveShieldedDamage", RpcTarget.Others);
    }

    public void SuccesfullyDealtDamage(GameObject enemyPlayer)//player 0 did damage to player 1
    {
        print("you've dealt: " + damage + " damage.");
        enemyPlayer.GetComponent<PlayerManager>().hp -= damage;

        photonID.RPC("OnReceiveDamage", RpcTarget.Others);
    }
    [PunRPC]
    public void OnReceiveDamage()// Only player 1 gets this
    {
        playerUI.OnHealthChange(hp);
        playerAnimations.SetTrigger("Get Hit");
    }

    [PunRPC]
    public void OnReceiveShieldedDamage()
    {
        playerAnimations.SetTrigger("Block");
    }

    #region From Lobby To Game
    [PunRPC]
    public void LoadIntoGame()
    {
        print("STEP 0" + PhotonNetwork.NickName);
        AllReadyUpAnimations.SetBool("GameStart", true);

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
        UIPrefab.transform.GetChild(2).gameObject.SetActive(true);
        print("STEP 1" + PhotonNetwork.NickName);

        if (SceneManager.GetActiveScene().name != "BattlefieldCom" && PhotonNetwork.IsMasterClient)// Iedereen volgt de masterclient wanneer hij van scene veranderd. //PhotonNetwork.AutomaticallySyncScene = true;
        {
            PhotonNetwork.LoadLevel("BattlefieldCom");
        }
        print("STEP 2" + PhotonNetwork.NickName);

        if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1)// Als je nog niet klaar bent met laden.
        {
            print("Waiting Again. Progress: " + PhotonNetwork.LevelLoadingProgress);
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(WaitingReadyUpAnimation());
        }
        else //als je klaar bent met laden.
        {
            print("Waiting Done! Progress: " + PhotonNetwork.LevelLoadingProgress);
            ArrivedAtGame();
            print("STEP 3" + PhotonNetwork.NickName);
        }
        yield return new WaitForSeconds(0);
    }
    public void ArrivedAtGame()
    {
        print("ArrivedAtGame Activated");

        if(SceneManager.GetActiveScene().name != "BattlefieldCom")
        {
            print("Player is still in scene: " + SceneManager.GetActiveScene().name);
            PhotonNetwork.LoadLevel("BattlefieldCom");
        }
        UIPrefab.transform.GetChild(2).gameObject.SetActive(false);
        isReadyToFight = true;
        crGameLobbyManager.transform.GetChild(0).gameObject.SetActive(false);
        print("STEP 4" + PhotonNetwork.NickName);


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
        }
        print("STEP 5" + PhotonNetwork.NickName);
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
        print("STEP 6" + PhotonNetwork.NickName);
        print("GameStarted activated");
        if (GameObject.Find("Main Camera GameRoom"))
        {
            GameObject.Find("Main Camera GameRoom").SetActive(false);
        }
        else
        {
            print("Couldn't find camera");
        }
        playerCamera.gameObject.SetActive(true);
        playerMoving.allowMoving = true;

        if (!crGameLobbyManager.allPlayers[1].GetComponent<PlayerMovement>().allowMoving)
        {
            print("Other player didn't get: allowMoving");
            crGameLobbyManager.allPlayers[1].GetComponent<PlayerMovement>().allowMoving = true;
        }
        print("STEP 7" + PhotonNetwork.NickName);
    }
    #endregion

    #region OnTriggerEnter/Exit voids
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playersInAttackRange.Add(other.gameObject);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playersInAttackRange.RemoveAt(playersInAttackRange.Count);
        }
    }
    #endregion 
}