using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
	Transform mainCamera;

	void Awake(){
		mainCamera = Camera.main.transform;
	}

	void LateUpdate(){
		if(mainCamera == null){ return; }

		// transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);
		transform.rotation = Quaternion.Inverse(mainCamera.rotation);
	}
}
