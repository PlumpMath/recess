using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerSingleton : NetworkBehaviour {
	void Awake(){
		if(NetworkManager.singleton){
			Destroy(gameObject);
		}
	}

	public override void OnStartLocalPlayer(){
		Debug.Log("START LOCAL PLAYER!");
	}
}
