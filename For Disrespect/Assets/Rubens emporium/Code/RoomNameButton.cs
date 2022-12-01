using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}
