using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectSpawn : NetworkBehaviour {
	public GameObject prefab;

	void Start(){
		if(isServer){
			Debug.LogFormat("Spawn a {0}!", prefab.name);
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            NetworkServer.Spawn(o);
		}

		Destroy(gameObject);
	}
}
