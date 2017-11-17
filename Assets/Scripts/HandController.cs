using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour {
	public GameObject Hand;
	public HoldableItem HeldObject;
    private List<HoldableItem> HoldableItems;
	public float ThrowSpeed;
    private float ChargeLevel;
    private float ChargeMax = 100.0f;

    private Image PowerFill;

	void Awake(){
        HoldableItems = new List<HoldableItem>();
        GameObject pbf = GameObject.Find("Power Bar Fill");
        if(pbf != null){
            PowerFill = pbf.GetComponent<Image>();
        }
	}

	public void Grab(){
        int count = HoldableItems.Count;
        if (count > 0)
        {
            HoldableItem itemToGrab = HoldableItems[count - 1];
            GrabItem(itemToGrab);
            HoldableItems.Remove(itemToGrab);
        }
	}

	public void GrabItem(HoldableItem itemToGrab){
		HeldObject = itemToGrab.PickUp(this.gameObject, Hand);
    }

    public void Charge() {
        if(ChargeLevel <= ChargeMax) {
            ChargeLevel += Time.deltaTime * ThrowSpeed;
        }

        PowerFill.fillAmount = ChargeLevel / 100;
    }


    public void Release() {
        HoldableItem releasedItem = HeldObject.Release();
        Vector3 TossDirection =
            (Hand.transform.forward * ChargeLevel +
            (Hand.transform.up * 3.0f));
        Rigidbody rb = releasedItem.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(TossDirection, ForceMode.Impulse);
        ChargeLevel = 0;
        PowerFill.fillAmount = 0;

        HeldObject = null;
    }

    void OnTriggerEnter(Collider other)
    {
        HoldableItem h = other.GetComponent<HoldableItem>();
        if (h)
        {
            HoldableItems.Add(h);
        }
    }

    void OnTriggerExit(Collider other)
    {
        HoldableItem h = other.GetComponent<HoldableItem>();
        if(h == null){
            return;
        }

        if (HoldableItems.Contains(h))
        {
            HoldableItems.Remove(h);
        }
    }
}
