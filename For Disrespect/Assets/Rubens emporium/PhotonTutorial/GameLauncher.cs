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
    public byte maxPlayersInRoom = 4;
    public bool isConnectedToMaster;
    public bool isConnectedToLobby;

    public string crSelectedRoomName;
    public string createRoomName;
    public InputField roomName;
    public InputField displayingRoomNameInput;

    public byte createMaxTotalPlayers;
    public InputField maxTotalPlayers;


    public bool createPrivacySettings;
    public Toggle privacySettings;

    public RoomOptions createRoomSettings;

    public List<string> stringOfAllRooms;
    public RoomInfo listOfRoomInfo;
    public GameObject buttonPrefab;
    public GameObject crInstantiatedButtonPrefab;


    public GameObject loadingText;
    public GameObject mainMenuWindow;
    public GameObject choosingLobbyOrCreate;
    public GameObject creatingLobby;

    public Transform contentToParent;
    public RoomListing roomListing;

    public string crServerAdress;
    public int crPort;
    public string crAppID;


    #region MonoBehaviour CallBacks

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
    }

    #endregion

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
        if(crSelectedRoomName != "")
        {
            PhotonNetwork.JoinRoom(crSelectedRoomName);
        }
    }
    public void CreateRoomButton()
    {
        PhotonNetwork.CreateRoom(createRoomName,new RoomOptions {IsVisible = createPrivacySettings, MaxPlayers = createMaxTotalPlayers}, TypedLobby.Default);
    }
    public void LeaveCreatingRoomButton()
    {
        creatingLobby.SetActive(false);
        choosingLobbyOrCreate.SetActive(true);
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

        print("Loading GameRoom");
     
        if (PhotonNetwork.CurrentRoom.PlayerCount <= maxPlayersInRoom)
        {
            PhotonNetwork.LoadLevel("GameRoom");
        }
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("OnDisconnected was activated: " + cause);
        if("Launcher" == SceneManager.GetActiveScene().name)
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
        print("Created: " + createRoomName);

        base.OnCreatedRoom();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print(returnCode + message);

        base.OnCreateRoomFailed(returnCode, message);
    }

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    print("OnJoinRandomFailed was activated: " + returnCode + message);

    //    //creates new room after he couldn't join one.
    //    PhotonNetwork.CreateRoom("GameRoom", new RoomOptions { MaxPlayers = maxPlayersInRoom });
    //    base.OnJoinRandomFailed(returnCode, message);
    //}

    #endregion

}
