using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : MonoBehaviour {
	private AudioSource bounce;
	private HoldableItem holdable;

	void Awake(){
		bounce = GetComponent<AudioSource>();
		holdable = GetComponent<HoldableItem>();

		holdable.OnThrown += HandleThrow;
	}

	void HandleThrow(){
		Debug.LogFormat("{0} was thrown!", gameObject.name);
	}

	void OnCollisionEnter(Collision other){
		bounce.Play();
	}
}
