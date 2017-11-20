using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectSpawn : MonoBehaviour {
	public GameObject prefab;

	void Start(){
		GameObject o = Instantiate(prefab, transform.position, transform.rotation);
		NetworkServer.Spawn(o);

		Destroy(gameObject);
	}
}
