﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour {
	public GameObject Hand;
	public GameObject HeldObject;
    private List<GameObject> HoldableItems;
	public float ThrowSpeed;
    private float ChargeLevel;
    private float ChargeMax = 100.0f;

    private Image PowerFill;

	void Awake(){
        HoldableItems = new List<GameObject>();
        GameObject pbf = GameObject.Find("Power Bar Fill");
        if(pbf != null){
            PowerFill = pbf.GetComponent<Image>();
        }
	}

	public void Grab(){
        int count = HoldableItems.Count;
        if (count > 0)
        {
            GameObject itemToGrab = HoldableItems[count - 1];
            GrabItem(itemToGrab);
            HoldableItems.Remove(itemToGrab);
        }
	}

	public void GrabItem(GameObject itemToGrab){
		HeldObject = itemToGrab;

		itemToGrab.transform.parent = Hand.transform;
		Vector3 pos = itemToGrab.transform.position;
		pos.y += 1.0f;

		itemToGrab.transform.position = pos;

		itemToGrab.GetComponent<Rigidbody>().isKinematic = true;

    }

    public void Charge() {
        if(ChargeLevel <= ChargeMax) {
            ChargeLevel += Time.deltaTime * ThrowSpeed;
        }

        PowerFill.fillAmount = ChargeLevel / 100;
    }


    public void Release() {
        HeldObject.transform.parent = transform.parent;
        Vector3 TossDirection =
            (Hand.transform.forward * ChargeLevel +
            (Hand.transform.up * 3.0f));
        Rigidbody rb = HeldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(TossDirection, ForceMode.Impulse);
        HeldObject = null;
        ChargeLevel = 0;
        // TextController.instance.ThrowPower.text = null;
        PowerFill.fillAmount = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HoldableItem>())
        {
            HoldableItems.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (HoldableItems.Contains(other.gameObject))
        {
            HoldableItems.Remove(other.gameObject);
        }
    }
}
