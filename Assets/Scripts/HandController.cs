using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {
	public GameObject Hand;
	public GameObject HeldObject;

	public void GrabItem(GameObject itemToGrab){
		HeldObject = itemToGrab;

		itemToGrab.transform.parent = Hand.transform;
		itemToGrab.transform.localPosition = new Vector3(0, 0, 0);
		itemToGrab.transform.localRotation = Quaternion.Euler(0,0,0);

		itemToGrab.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void ReleaseItem(){
		HeldObject.transform.parent = transform.parent;
		Vector3 TossDirection =
			(HeldObject.transform.forward * 8.0f) +
			(HeldObject.transform.up * 3.0f);
		Rigidbody rb = HeldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
		rb.AddForce(TossDirection, ForceMode.Impulse);
		HeldObject = null;
	}
}
