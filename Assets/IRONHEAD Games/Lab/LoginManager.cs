using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class LoginManager : MonoBehaviourPunCallbacks
{
    
    // Start is called before the first frame update
    void Start()
    {
    }
    
    // Public method to start the connection process
    public void OnConnectButtonPressed()
    {
        ConnectToPhoton();
    }

    public void ConnectToPhoton()
    {
        PhotonNetwork.NickName = " Deneme" + Random.Range(0, 10000);
        PhotonNetwork.ConnectUsingSettings();
    }
    // # Photon Callback Methods
    public override void OnConnected()
    {
        Debug.Log("OnConnected is called. The server is available!");
    }
    // # this callback method is called when the user is 
    // successfully connected to the Photon server. 
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server- Nickname: " + PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby ");
    }


}
