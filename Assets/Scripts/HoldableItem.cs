using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HoldableItem : NetworkBehaviour
{
    private Rigidbody rb;
    private NetworkIdentity networkIdentity;

    public delegate void Thrown(GameObject thrower);
    public event Thrown OnThrown;

    public delegate void Grabbed(GameObject owner);
    public event Grabbed OnGrabbed;

    [SyncVar]
    GameObject Owner;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        networkIdentity = GetComponent<NetworkIdentity>();
    }

    void Start()
    {
        networkIdentity.localPlayerAuthority = true;
    }

    public HoldableItem PickUp(GameObject owner)
    {
        CmdPickUp(owner);

        if(OnGrabbed != null) {
            OnGrabbed(owner);
        }

		return this;
    }

    [Command]
    public void CmdPickUp(GameObject owner)
    {
        RpcPickUp(owner);
        NetworkIdentity networkIdentity = owner.GetComponent<NetworkIdentity>();
        if(networkIdentity != null){
            ServerSetParent(networkIdentity);
        }
    }

    [ClientRpc]
    public void RpcPickUp(GameObject owner){
        Owner = owner;
        rb.isKinematic = true;
    }

    public HoldableItem Release(Vector3 TossDirection)
    {
        CmdRelease(TossDirection);

        if(OnThrown != null) {
            OnThrown(Owner);
        }
        
        return this;
    }

    [Command]
    public void CmdRelease(Vector3 TossDirection)
    {
        ServerSetParent(null);
        RpcClientRelease(TossDirection);
    }

    [ClientRpc]
    public void RpcClientRelease(Vector3 TossDirection){
        Owner = null;
        rb.isKinematic = false;
        rb.AddForce(TossDirection, ForceMode.Impulse);
    }

    public void ServerSetParent(NetworkIdentity parentNetworkIdentity){
        if(parentNetworkIdentity != null){
            transform.SetParent(parentNetworkIdentity.transform);
            RpcClientSetParent(parentNetworkIdentity.netId);
        } else {
            transform.SetParent(null);
            RpcClientSetParent(NetworkInstanceId.Invalid);
        }
    }

    [ClientRpc]
    void RpcClientSetParent(NetworkInstanceId newParentNetId){
        Transform parentTransform = null;
        if(newParentNetId != NetworkInstanceId.Invalid){
            var parentGameObj = ClientScene.FindLocalObject(newParentNetId);
            if(parentGameObj != null){
                parentTransform = parentGameObj.transform;
            }
        }

        transform.SetParent(parentTransform);
    }
}
