using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HoldableItem : NetworkBehaviour
{
    private Rigidbody rb;
    private NetworkIdentity networkIdentity;

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
        Debug.LogFormat("{0} was picked up by {1}", gameObject.name, owner.name);
        Owner = owner;
        rb.isKinematic = true;
    }

    public HoldableItem Release(Vector3 TossDirection)
    {
        Debug.Log("Release()");
        CmdRelease(TossDirection);
        return this;
    }

    [Command]
    public void CmdRelease(Vector3 TossDirection)
    {
        Debug.Log("CmdRelease()");
        ServerSetParent(null);
        RpcClientRelease(TossDirection);
    }

    [ClientRpc]
    public void RpcClientRelease(Vector3 TossDirection){
        Debug.Log("RpcClientRelease()");
        Debug.LogFormat("{0} was dropped by {1}", gameObject.name, Owner.name);
        Owner = null;
        rb.isKinematic = false;
        Debug.LogFormat("Add force {0}", TossDirection);
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
