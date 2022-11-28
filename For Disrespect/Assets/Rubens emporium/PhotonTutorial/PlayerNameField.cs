using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//[RequireComponent(typeof(InputField))]
public class PlayerNameField : MonoBehaviour
{
    public string crPlayerName;
    public string defaultSavedName;
    public InputField playerNameInput;

    //Zorgt er voor dat de default name gedisplayed word
    public void Start()
    {
        if (playerNameInput != null)
        {
            if (PlayerPrefs.HasKey(crPlayerName))
            {
                defaultSavedName = PlayerPrefs.GetString(crPlayerName);
                playerNameInput.text = defaultSavedName;
            }
        }
        PhotonNetwork.NickName = defaultSavedName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            print("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(crPlayerName, value);
    }
}
