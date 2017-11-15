using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour {
	public float Speed = 1.0f;
	public float Lifetime = 10.0f;

	private Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		Destroy(gameObject, Lifetime);
	}

	// Update is called once per frame
	void Update () {
		rigidbody.AddForce(transform.forward * Speed);
	}
}
