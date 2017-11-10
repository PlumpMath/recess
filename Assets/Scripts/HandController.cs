using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {
	public GameObject Hand;
	public GameObject HeldObject;
    private List<GameObject> HoldableItems;
	public float ThrowSpeed;

	void Awake(){
        HoldableItems = new List<GameObject>();
	}

	public void Use(){
        if (HeldObject != null){
			ReleaseItem();
        } else {
            int count = HoldableItems.Count;
            if (count > 0)
            {
                GameObject itemToGrab = HoldableItems[count - 1];
                GrabItem(itemToGrab);
                HoldableItems.Remove(itemToGrab);
            }
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
