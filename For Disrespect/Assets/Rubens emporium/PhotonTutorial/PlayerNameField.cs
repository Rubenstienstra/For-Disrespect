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

    #region PlayerName

    void Start()
    {
        if(playerNameInput != null)
        {
            if (PlayerPrefs.HasKey(crPlayerName))
            {
                defaultName = PlayerPrefs.GetString(crPlayerName);
                playerNameInput.text = defaultName;
            }
        }
        PhotonNetwork.NickName = defaultName;
    }
    public void SetPlayerName(string stringValue)
    {
        if (string.IsNullOrEmpty(stringValue))
        {
            print(stringValue);
        }
        PhotonNetwork.NickName = stringValue;

        PlayerPrefs.SetString(crPlayerName, stringValue);
    }

    #endregion


}
