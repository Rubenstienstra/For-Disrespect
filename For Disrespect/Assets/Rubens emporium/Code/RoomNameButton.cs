using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomNameButton : MonoBehaviour
{
    public string roomNameString;

    public GameObject inputFieldRoomName;

    public void Start()
    {
        inputFieldRoomName = GameObject.Find("InputField RoomName");
    }
    public void SetInputFieldRoomName()
    {
        inputFieldRoomName.GetComponent<InputField>().text = roomNameString;
    }
    public void SetRoomInfo(RoomInfo info)
    {
        roomNameString = info.Name.ToString();
    }
}
