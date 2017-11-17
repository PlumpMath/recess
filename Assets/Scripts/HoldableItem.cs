using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HoldableItem : NetworkBehaviour {
    public GameObject Owner;
	private NetworkIdentity networkIdentity;
	private NetworkTransform networkTransform;
	private Transform NativeParent;
	private Rigidbody rb;
	private Collider collider;

	void Awake(){
		NativeParent = transform.parent;

		rb = GetComponent<Rigidbody>();
        networkIdentity = GetComponent<NetworkIdentity>();
		networkTransform = GetComponent<NetworkTransform>();
		collider = GetComponent<Collider>();
	}

	void Update(){
		if(Owner != null && hasAuthority){
			Debug.LogFormat("UPDATE THE POSITION OF {0}", gameObject.name);
			transform.position = transform.parent.position;
			CmdMoveWithOwner();
		}
	}

	public HoldableItem PickUp(GameObject owner, GameObject hand){
		Owner = owner;
		Debug.LogFormat("{0} was picked up by {1}", gameObject.name, Owner.name);

		transform.SetParent(hand.transform);
		transform.localPosition = Vector3.zero;
		networkTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
		rb.isKinematic = true;

		CmdPickUp(owner);

		return this;
	}

	[Command]
	void CmdMoveWithOwner(){
        transform.position = transform.parent.position;
	}

    [Command]
    void CmdPickUp(GameObject owner)
    {
        NetworkIdentity ownerIdentity = owner.GetComponent<NetworkIdentity>();
        networkIdentity.localPlayerAuthority = true;
        networkIdentity.AssignClientAuthority(ownerIdentity.clientAuthorityOwner);
		RpcPickUp(owner);
    }

	[ClientRpc]
	void RpcPickUp(GameObject owner){
		Debug.LogFormat("RPC: Picked up by {0}", owner.name);

        NetworkIdentity ownerIdentity = owner.GetComponent<NetworkIdentity>();
        networkIdentity.localPlayerAuthority = true;
        networkIdentity.AssignClientAuthority(ownerIdentity.clientAuthorityOwner);
	}

	public HoldableItem Release(){
		Debug.LogFormat("{0} was dropped by {1}", gameObject.name, Owner.name);
		rb.isKinematic = false;
		transform.SetParent(NativeParent);
        networkTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody3D;

		CmdReleaseOwnership(Owner);
		Owner = null;

		return this;
	}

	[Command]
	void CmdReleaseOwnership(GameObject owner){
		networkIdentity.localPlayerAuthority = false;
	}
}
