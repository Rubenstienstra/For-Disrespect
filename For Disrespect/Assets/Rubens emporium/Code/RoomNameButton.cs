using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class RoomNameButton : MonoBehaviour // deze script was voor de button list zodat je een lobby kon vinden en alleen maar erop hoeft te klikken.
{
    public string roomNameString;

    public TMP_InputField inputFieldRoomName;

    public void Start()
    {
        //inputFieldRoomName = GameObject.Find("InputField RoomName");
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
