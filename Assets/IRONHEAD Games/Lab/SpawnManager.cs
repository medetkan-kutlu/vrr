using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnManager :  MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject genericVRPlayerPrefab;
    public Vector3 spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        //this means we are connected photon and do some operations like join or leave 
        //now we can instantiate the players. 

        // PhotonNetwork.Instantiate(genericVRPlayerPrefab.name, spawnPosition, Quaternion.identity);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient){
            Debug.Log("OnPlayerEnteredRoom() " + newPlayer.NickName);
            if(PhotonNetwork.IsMasterClient){
                PhotonNetwork.Instantiate(genericVRPlayerPrefab.name, spawnPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
