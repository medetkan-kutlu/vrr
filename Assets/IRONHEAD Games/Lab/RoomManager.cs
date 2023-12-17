using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI occupancyRateText_ForSchool;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed: " + message);
        CreateAndJoinRoom();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with the name: "+PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("The local player: " + PhotonNetwork.NickName + " joined to "+ PhotonNetwork.CurrentRoom.Name + 
            " Player count: "+ PhotonNetwork.CurrentRoom.PlayerCount);
 
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("map"))
        {
            object mapType;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out mapType))
            {
                Debug.Log("Joined room with the map: " + (string)mapType);
                if ((string)mapType == "school")
                {
                    Debug.Log("LoadLevel World_School: ");
                    PhotonNetwork.LoadLevel("World_School");
                }
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName+ " joined to: Player count: "+PhotonNetwork.CurrentRoom.PlayerCount);
    }
 
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate: " + roomList.Count);
        //roomInfo class contains lots of data about the room. 
        foreach (RoomInfo room in roomList)
        {
           Debug.Log(room.Name);
           object mapType;
           room.CustomProperties.TryGetValue("map", out mapType);
           print("AAAAAAA" + mapType);
           if ((string)mapType == "school")
           {
            //    update the school room occupancy field
            print(room.PlayerCount);
              occupancyRateText_ForSchool.text = room.PlayerCount + " / " + 20;
           }
        }
    }
 
   
 
    public void OnEnterJoinRoomBasedOnMapButton()
    {
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties
            = new ExitGames.Client.Photon.Hashtable() { { "map", "school" } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + Random.Range(0,10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
 
        ////this string array called roomPropsInLobby 
        ////will hold the room properties that we will 
        ////introduce to the lobby. 
        string[] roomPropsInLobby = {"map" };
        ////we have 2 different maps
        ////1. outdoor = "outdoor"
        ////2. school = "school"
 
        ExitGames.Client.Photon.Hashtable customRoomProperties
            = new ExitGames.Client.Photon.Hashtable() { { "map", "school" } };
 
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
}