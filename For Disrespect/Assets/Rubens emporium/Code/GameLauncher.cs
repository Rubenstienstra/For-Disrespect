using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameLauncher : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    //GameLauncher is voor wanneer je in de game wil.

    public string gameVersion = "1";
    public byte maxPlayersInRoom = 2;
    public bool isConnectedToMaster;
    public bool isConnectedToLobby;

    public string joinRoomName;
    public string createRoomName;
    public TMP_InputField joinRoomNameInput;
    public TMP_InputField createRoomNameInput;
    public string sceneName = "Lobby";

    public byte createMaxTotalPlayers = 2;

    public RoomOptions createRoomSettings;

    public List<string> stringOfAllRooms;
    public RoomInfo listOfRoomInfo;
    public GameObject buttonPrefab;
    public GameObject crInstantiatedButtonPrefab;

    public GameObject loadingText;
    public GameObject mainMenuWindow;
    public GameObject choosingLobbyOrCreate;
    public GameObject creatingLobby; // is in different scene

    public string crServerAdress;
    public int crPort;
    public string crAppID;

    public GameObject mainMenuLobbyMusic;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            mainMenuWindow.SetActive(false);
        }
        DontDestroyOnLoad(mainMenuLobbyMusic);
    }

    #region Public Methods

    public void Connect()
    {
        PhotonNetwork.GameVersion = gameVersion;

        crServerAdress = PhotonNetwork.ServerAddress;
        crPort = PhotonNetwork.ServerPortOverrides.MasterServerPort;
        crAppID = PhotonNetwork.AppVersion;

        if(PhotonNetwork.IsConnected)
        {
            mainMenuWindow.SetActive(false);
            choosingLobbyOrCreate.SetActive(true);
            print("Player connection is: " + PhotonNetwork.IsConnected);

            return;
        }
        else if (!PhotonNetwork.IsConnected)
        {
            loadingText.SetActive(true);
            mainMenuWindow.SetActive(false);
            PhotonNetwork.ConnectUsingSettings();
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.ConnectToMaster(crServerAdress, crPort, crAppID);
        }
    }
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    public void JoinRoomButton()
    {
        print("JoinButton is pressed");
        if (isConnectedToMaster && isConnectedToLobby && joinRoomName != "")
        {
            PhotonNetwork.JoinRoom(joinRoomName);
        }
        else
        {
            Connect();
        }
    }
    public void SetJoinRoomName()
    {
        joinRoomName = joinRoomNameInput.text;
        print("Current RoomName: " + joinRoomName);
    }
    public void SetCreateRoomName()
    {
       createRoomName = createRoomNameInput.text;
       print("Current RoomName: " + createRoomName);
    }
    public void CreateRoomButton()
    {
        if(createRoomName != "")
        {
            PhotonNetwork.CreateRoom(createRoomName, new RoomOptions { IsVisible = true, MaxPlayers = createMaxTotalPlayers, IsOpen = true, PublishUserId = true }, TypedLobby.Default);
        }
        else
        {
            print("The Room Has No Name!");
            choosingLobbyOrCreate.SetActive(true);
        }
    }
    public void LeaveCreatingRoomButton()
    {
        creatingLobby.SetActive(false);
        choosingLobbyOrCreate.SetActive(true);
    }
    public void LeaveApplicationButton()
    {
        Application.Quit();
    }

    #endregion

    #region Callbacks Photon

    public override void OnConnectedToMaster()
    {
        isConnectedToMaster = true;
        print("OnConnectedToMaster was activated");

        PhotonNetwork.JoinLobby();

        base.OnConnectedToMaster();
    }
    public override void OnJoinedLobby()
    {
        isConnectedToLobby = true;
        
        if(isConnectedToLobby && isConnectedToMaster)
        {
            loadingText.SetActive(false);
            choosingLobbyOrCreate.SetActive(true);
        }
        else
        {
            Connect();
        }
        base.OnJoinedLobby();
    }
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom was activated: ");
        loadingText.SetActive(false);
        mainMenuWindow.SetActive(true);

        print("Loading "+ sceneName);
     
        PhotonNetwork.LoadLevel(sceneName);
        
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("OnDisconnected was activated: " + cause);
        if("MainMenu" == SceneManager.GetActiveScene().name)
        {
            if(loadingText != null && mainMenuWindow != null)
            {
                loadingText.SetActive(false);
                mainMenuWindow.SetActive(true);
            }
        }
        base.OnDisconnected(cause);
    }
    public override void OnCreatedRoom()
    {
        if(createRoomName != "")
        {
            print("Created: " + createRoomName + ". In: " + sceneName);
        }
        else
        {
            print("Created: MISSING ROOM NAME" + ". In: " + sceneName);
        }
        
        base.OnCreatedRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print(returnCode + message);

        choosingLobbyOrCreate.SetActive(true);

        base.OnCreateRoomFailed(returnCode, message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(returnCode + message);
        base.OnJoinRoomFailed(returnCode, message);
    }

    #endregion

}