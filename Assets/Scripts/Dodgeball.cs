using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : MonoBehaviour {
	private AudioSource bounce;
	private HoldableItem holdable;

	private bool IsHot;

	void Awake(){
		bounce = GetComponent<AudioSource>();
		holdable = GetComponent<HoldableItem>();
		IsHot = false;

		holdable.OnThrown += HandleThrow;
		holdable.OnGrabbed += HandleGrab;
	}

	void HandleGrab(GameObject grabber){
		Debug.LogFormat("{0} was picked up by {1}!", gameObject.name, grabber.name);
	}

	void HandleThrow(GameObject thrower){
		Debug.LogFormat("{0} was thrown by {1}!", gameObject.name, thrower.name);
		IsHot = true;
	}

	void OnCollisionEnter(Collision other){
		bounce.Play();
		if(IsHot){
            if (other.gameObject.tag == "Player") {
                HitPlayer(other.gameObject);
            }

			if(other.gameObject.name == "Broken Board") {
				other.collider.attachedRigidbody.constraints = RigidbodyConstraints.None;
            }

            Debug.LogFormat("Ball hit {0}, no longer hot", other.gameObject.name);
            IsHot = false;
		}
	}

	void HitPlayer(GameObject player){
		Debug.LogFormat("{0} was hit by a dodgeball!", player.name);
	}
}
