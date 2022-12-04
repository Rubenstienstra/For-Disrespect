using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListing : MonoBehaviourPunCallbacks
{
    public GameLauncher gameLauncher;
    public void Start()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InLobby)
        {
            print("OnRoomListUpdate Should work");
        }
        else
        {
            print(PhotonNetwork.IsMasterClient + PhotonNetwork.InLobby.ToString());
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("OnRoomListUpdate Is Being Checked!");
        foreach (RoomInfo info in roomList)
        {
            print("OnRoomListUpdate Found A roomlist");

            RoomListing listing = Instantiate(gameLauncher.roomListing, gameLauncher.contentToParent);
            if (listing != null)
                listing.SetRoomInfo(info);



            gameLauncher.crButtonPrefab = Instantiate(gameLauncher.buttonPrefab, gameLauncher.contentToParent);
            gameLauncher.crButtonPrefab.GetComponent<RoomNameButton>().SetRoomInfo(info);

            if (gameLauncher.crButtonPrefab != null)
            {
                print("Sended Info");
            }
            else if (gameLauncher.crButtonPrefab == null)
            {
                print("The're no rooms!");
            }

        }
        base.OnRoomListUpdate(roomList);
    }
    public void SetRoomInfo(RoomInfo info)
    {

    }
}
