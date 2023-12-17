using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class RoomManagerTest : MonoBehaviourPunCallbacks
{
    private bool isCooldown = false;
    private float cooldownTime = 3.0f; // Cooldown time in seconds
    public string levelToLoad;
    private static bool roomCreated = false; // Flag to track room creation
    private bool isSceneBeingLoaded = false;
    [SerializeField] public int labarotoryNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (!isCooldown)
        {
            isCooldown = true;
            Debug.Log("Player entered the trigger");
            if (!roomCreated)
            {
                CreateRoomAndLoadLevel();
            }
            else
            {
                JoinRoomAndLoadLevel();
            }
            StartCoroutine(ResetCooldown());
        }
        
    }
    
    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }

    void CreateRoomAndLoadLevel()
    {
        Debug.Log("Creating a new room");
        PhotonNetwork.CreateRoom("Labarotory " + labarotoryNumber); // Create a new room
        roomCreated = true; // Set the flag to true
    }

    void JoinRoomAndLoadLevel()
    {
        Debug.Log("Attempting to join a random room");
        PhotonNetwork.JoinRandomRoom(); // Attempt to join a random room
    }

    public override void OnJoinedRoom()
    {
        if (!isSceneBeingLoaded)
        {
            isSceneBeingLoaded = true;
            Debug.Log("Joined a room, loading level: " + levelToLoad);
            PhotonNetwork.LoadLevel("HospitalScene");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room. Creating a new room named 'lab1'.");
        PhotonNetwork.CreateRoom("lab1"); // Create a new room named 'lab1' if joining failed
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Successfully created a room, loading level: " + levelToLoad);
        PhotonNetwork.LoadLevel("HospitalScene"); // Load the level after creating the room
    }
}