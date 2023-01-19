using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class ConnectorToRoom : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();  
    }
    public override void OnConnectedToMaster()
    {
        print("connected");
        PhotonNetwork.JoinRandomRoom();
        base.OnConnectedToMaster();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
