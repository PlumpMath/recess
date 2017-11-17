using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour {
	public GameObject Prefab;
	public Transform SpawnPoint;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			CmdSpawn();
		}
	}

	[Command]
	void CmdSpawn(){
        GameObject o = Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
        NetworkServer.Spawn(o);
	}
}
