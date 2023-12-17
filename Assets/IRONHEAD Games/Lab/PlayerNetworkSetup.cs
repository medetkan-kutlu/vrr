using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject localXROriginGameObject;

    
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            //the player is local
            localXROriginGameObject.SetActive(true);
        }
        else
        {
            //the player is remote 
            //If it is not local player, we will disable the XR origin gameobject under the generic VR player.
            localXROriginGameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
