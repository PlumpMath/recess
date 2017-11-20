using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectSpawn : NetworkBehaviour {
	public GameObject prefab;

	void Start(){
		if(isServer){
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            NetworkServer.SpawnWithClientAuthority(o, connectionToServer);
		}

		Destroy(gameObject);
	}
}
