using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshDeformer : MonoBehaviourPunCallbacks
{
    ObjectDeformable deformable;
    bool isSet = false;
    // PhotonView photonView;

    public InputActionReference deform;
    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            deformable = FindAnyObjectByType<ObjectDeformable>();
            // StartDeform();
        }
    }


    private void StartDeform()
    {

        // photonView = GetComponentInChildren<PhotonView>();
        if (photonView.IsMine)
        {
            deformable.StartDeform();
        }
    }


    void Update()
    {
        if (PhotonNetwork.InRoom && !isSet)
        {
            isSet = true;
            Invoke("StartDeform", 4f);
        }
        if(deformable){
            photonView.RPC("Call", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Call(){
        deformable.CallDeform();

    }
}