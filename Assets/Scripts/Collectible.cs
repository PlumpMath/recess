using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

	public void PickMeUp(){
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
        GetComponent<ParticleSystem>().Stop();
	}
}
