using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(InputField))]
public class PlayerNameField : MonoBehaviour
{
    const string crPlayerName = "PlayerName";
    private string defaultName;
    public InputField playerNameInput;

    public void Start()
    {
        if (playerNameInput != null)
        {
            if (PlayerPrefs.HasKey(crPlayerName))
            {
                defaultName = PlayerPrefs.GetString(crPlayerName);
                playerNameInput.text = defaultName;
            }
        }
        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string valueString)
    {
        if (string.IsNullOrEmpty(valueString))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = valueString;


        PlayerPrefs.SetString(crPlayerName, valueString);
    }
}
