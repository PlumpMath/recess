using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {
	public GameObject Hand;
	public GameObject HeldObject;
	public float ThrowSpeed;

	public void GrabItem(GameObject itemToGrab){
		HeldObject = itemToGrab;
		
		itemToGrab.transform.parent = Hand.transform;
		Vector3 pos = itemToGrab.transform.position;
		pos.y += 1.0f;

		itemToGrab.transform.position = pos;

		itemToGrab.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void ReleaseItem(){
		HeldObject.transform.parent = transform.parent;
		Vector3 TossDirection =
			(Hand.transform.forward * ThrowSpeed) +
			(Hand.transform.up * 3.0f);
		Rigidbody rb = HeldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
		rb.AddForce(TossDirection, ForceMode.Impulse);
		HeldObject = null;
	}
}
