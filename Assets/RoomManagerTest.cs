using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManagerTest : MonoBehaviourPunCallbacks
{
    private bool isCooldown = false;
    private float cooldownTime = 3.0f; // Cooldown time in seconds
    private bool isSceneBeingLoaded = false;

    [SerializeField]
    GameObject genericVRPlayerPrefab;
    public Vector3 spawnPosition;
    [SerializeField] public int labarotoryNumber;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isCooldown)
        {
            isCooldown = true;
            Debug.Log("Player entered the trigger");
            PhotonNetwork.JoinOrCreateRoom("Labarotory " + labarotoryNumber, new RoomOptions(), TypedLobby.Default);
        }
        
    }
    
    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
    public override void OnJoinedRoom()
    {
        if (!isSceneBeingLoaded)
        {
            isSceneBeingLoaded = true;
            Debug.Log("Joined a room, loading level: ");
            PhotonNetwork.LoadLevel("HospitalScene");
            // // if(PhotonNetwork.IsMasterClient){
            //     PhotonNetwork.Instantiate(genericVRPlayerPrefab.name, spawnPosition, Quaternion.identity);
            // // }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room. Creating a new room named 'lab1'.");
        PhotonNetwork.CreateRoom("lab1"); // Create a new room named 'lab1' if joining failed
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Successfully created a room, loading level: ");
        PhotonNetwork.LoadLevel("HospitalScene"); // Load the level after creating the room
    }
}