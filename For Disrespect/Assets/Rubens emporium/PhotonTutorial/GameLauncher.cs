using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    //GameLauncher is voor wanneer je in de game wil.

    public string gameVersion = "1";
    public byte maxPlayersInRoom = 4;
    public bool isConnected;

    public string crSelectedRoomName;
    public string createRoomName;
    public InputField roomName;
    public InputField displayingRoomNameInput;

    public byte createMaxTotalPlayers;
    public InputField maxTotalPlayers;

    public bool createPrivacySettings;
    public Toggle privacySettings;

    public RoomOptions createRoomSettings;
    public TypedLobby createTypedLobby;

    public List<string> stringOfAllRooms;
    public RoomInfo listOfRoomInfo;

    public GameObject buttonPrefab;


    public GameObject loadingText;
    public GameObject controlWindow;
    public GameObject choosingLobbyOrCreate;
    public GameObject creatingLobby;

    public Transform contentToParent;
    public RoomListing _roomListing;



    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    #region Public Methods

    
    public void Connect()
    {
        loadingText.SetActive(true);
        controlWindow.SetActive(false);

        if (!PhotonNetwork.IsConnected)
        {
            isConnected = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.JoinRandomRoom();
        }
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
        PhotonNetwork.CreateRoom(createRoomName,new RoomOptions {IsVisible = createPrivacySettings, MaxPlayers = createMaxTotalPlayers}, createTypedLobby);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            
            RoomListing roomLister = Instantiate(_roomListing, contentToParent);

            if(roomLister != null) 
            {
                print("Sended Info");
            }
            else if(roomLister == null)
            {
                print("The're no rooms!");
            }
            
        }
        base.OnRoomListUpdate(roomList);
    }

    #endregion

    #region Callbacks Photon

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster was activated");
        loadingText.SetActive(false);
        //controlWindow.SetActive(true);
        choosingLobbyOrCreate.SetActive(true);

        PhotonNetwork.JoinLobby();

        if (isConnected)
        {
            isConnected = false;
        }

        base.OnConnectedToMaster();
    }
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom was activated: ");
        loadingText.SetActive(false);
        controlWindow.SetActive(true);

        print("Loading Room For " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.CurrentRoom.PlayerCount <= maxPlayersInRoom)
        {
            PhotonNetwork.LoadLevel("Room For " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        base.OnJoinedRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("OnDisconnected was activated: " + cause);
        isConnected = false;
        if("Launcher" == SceneManager.GetActiveScene().name)
        {
            loadingText.SetActive(false);
            controlWindow.SetActive(true);
        }
        base.OnDisconnected(cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("OnJoinRandomFailed was activated: " + returnCode + message);

        //creates new room after he couldn't join one.
        PhotonNetwork.CreateRoom("GameRoom", new RoomOptions { MaxPlayers = maxPlayersInRoom });
        base.OnJoinRandomFailed(returnCode, message);
    }

    #endregion

}
