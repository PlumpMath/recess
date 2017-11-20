using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HoldableItem : NetworkBehaviour
{
    private Rigidbody rb;
    private Vector3 OwnerOffset;

    [SyncVar]
    GameObject Owner;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public HoldableItem PickUp(GameObject owner)
    {
        CmdPickUp(owner);
		return this;
    }

    [Command]
    public void CmdPickUp(GameObject owner)
    {
        Debug.LogFormat("{0} was picked up by {1}", gameObject.name, owner.name);
        Owner = owner;
        OwnerOffset = transform.position - Owner.transform.position;
        //rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        NetworkIdentity networkIdentity = owner.GetComponent<NetworkIdentity>();
        if(networkIdentity != null){
            ServerSetParent(networkIdentity);
        }
    }

    public HoldableItem Release()
    {
        CmdRelease();
        return this;
    }

    [Command]
    public void CmdRelease()
    {
        Debug.LogFormat("{0} was dropped by {1}", gameObject.name, Owner.name);
        Owner = null;
        // rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        ServerSetParent(null);
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
