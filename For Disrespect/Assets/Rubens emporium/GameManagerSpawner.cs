using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManagerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject gameManagerPrefab;

    void Start()
    {
        PhotonNetwork.InstantiateRoomObject(gameManagerPrefab.name, default, Quaternion.identity);
    }
}
