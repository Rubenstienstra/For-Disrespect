using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public GameObject multiplayerDeletableMe;
    public GameObject multiplayerDeletableEnemy;

    public string crPlayerName;
    public string crEnemyName;

    public bool isReady;
    public float waitTimeAnimation = 2;

    public Animator AllReadyUpAnimations;

    public GameObject spawnPoints;

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
                print("Destroyed: " + multiplayerDeletableMe.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
                multiplayerDeletableMe.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else//Disables GameObjects for your enemy
        {
            for (int i = 0; i < multiplayerDeletableEnemy.transform.childCount; i++)
            {
                print("Destroyed: " + multiplayerDeletableEnemy.transform.GetChild(i).gameObject + "current for loop: " + i.ToString());
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
            print("tried giving enemy names");
        }
    }

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
        bool a = false;
        if(!a)
        {
            a = true;
            yield return new WaitForSeconds(waitTimeAnimation);
        }

        if (SceneManager.GetActiveScene().name != "GameRoom" && PhotonNetwork.IsMasterClient)// Iedereen volgt de masterclient wanneer hij van scene veranderd. //PhotonNetwork.AutomaticallySyncScene = true;
        {
            PhotonNetwork.LoadLevel("GameRoom");
        }
        if(PhotonNetwork.LevelLoadingProgress != 1)
        {
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(WaitingReadyUpAnimation());
        }
        ArrivedAtGame();
        
        yield return new WaitForSeconds(0);
    }
    public void ArrivedAtGame()
    {
        crGameLobbyManager.transform.GetChild(0).gameObject.SetActive(false);

        if (!spawnPoints)
        {
            spawnPoints = GameObject.Find("SpawnPoints");
        }
        if (PhotonNetwork.IsMasterClient)
        {
            this.gameObject.transform.SetParent(spawnPoints.transform.GetChild(0));
            SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName("GameRoom"));
            
        }
        else
        {
            this.gameObject.transform.SetParent(spawnPoints.transform.GetChild(1));
        }
    }

    //public void LeaveRoom()
    //{
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        PhotonNetwork.LeaveRoom();
    //    }
    //}
}
