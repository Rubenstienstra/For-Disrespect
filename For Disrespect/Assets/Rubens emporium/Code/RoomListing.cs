using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    public Button roomListButton; 

    public void SetRoomInfo(RoomInfo info)
    {
        roomListButton.GetComponentInChildren<Text>().text = info.Name.ToString();
    }
}
