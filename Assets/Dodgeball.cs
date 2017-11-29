using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : MonoBehaviour {
	private AudioSource bounce;

	void Awake(){
		bounce = GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision other){
		bounce.Play();
	}
}
