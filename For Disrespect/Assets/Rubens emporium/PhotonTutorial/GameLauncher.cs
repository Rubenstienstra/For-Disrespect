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

    public List<string> stringOfAllRooms;
    public RoomInfo listOfRoomInfo;
    public GameObject buttonPrefab;
    public GameObject crButtonPrefab;


    public GameObject loadingText;
    public GameObject mainMenuWindow;
    public GameObject choosingLobbyOrCreate;
    public GameObject creatingLobby;

    public Transform contentToParent;
    public RoomListing roomListing;


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
        loadingText.SetActive(true);
        mainMenuWindow.SetActive(false);

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
        PhotonNetwork.CreateRoom(createRoomName,new RoomOptions {IsVisible = createPrivacySettings, MaxPlayers = createMaxTotalPlayers}, TypedLobby.Default);
    }
    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    print("OnRoomListUpdate Is Being Checked!");
    //    foreach (RoomInfo info in roomList)
    //    {
    //        RoomListing listing = Instantiate(roomListing, contentToParent);
    //        if (listing != null)
    //            listing.SetRoomInfo(info);



    //        crButtonPrefab = Instantiate(buttonPrefab, contentToParent);
    //        crButtonPrefab.GetComponent<RoomNameButton>().SetRoomInfo(info);

    //        if (crButtonPrefab != null)
    //        {
    //            print("Sended Info");
    //        }
    //        else if (crButtonPrefab == null)
    //        {
    //            print("The're no rooms!");
    //        }

    //    }
    //    base.OnRoomListUpdate(roomList);
    //}

    #endregion

    #region Callbacks Photon

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster was activated");
        loadingText.SetActive(false);
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
        isConnected = false;
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("OnJoinRandomFailed was activated: " + returnCode + message);

        //creates new room after he couldn't join one.
        PhotonNetwork.CreateRoom("GameRoom", new RoomOptions { MaxPlayers = maxPlayersInRoom });
        base.OnJoinRandomFailed(returnCode, message);
    }

    #endregion

}
