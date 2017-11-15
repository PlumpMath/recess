using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	public GameObject Prefab;
	public Transform SpawnPoint;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
		}
	}
}
