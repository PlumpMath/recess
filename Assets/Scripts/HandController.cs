using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    public static float interactSphereRadius = 1.0f;
    private const int INTERACTIVE = 1 << 8;
    private NetworkTransformChild networkTransformChild;
    public HoldableItem HeldObject;
    public GameObject Hand;
    public float ThrowSpeed;
    private float ChargeLevel;
    private float ChargeMax = 100.0f;

    private Image PowerFill;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward, interactSphereRadius);
    }

    void Awake()
    {
        GameObject pbf = GameObject.Find("Power Bar");
        if (pbf != null)
        {
            PowerFill = pbf.GetComponent<Image>();
        }

        networkTransformChild = GetComponent<NetworkTransformChild>();
    }

    public void Grab()
    {
        Vector3 C = transform.position + transform.forward;

        if (Physics.CheckSphere(C, interactSphereRadius, INTERACTIVE))
        {
            Collider[] found = Physics.OverlapSphere(C, interactSphereRadius, INTERACTIVE);
            var orderedFound = found.OrderBy(c =>
            {
                return (c.gameObject.transform.position - C).sqrMagnitude;
            });

            GameObject obj = orderedFound.ElementAt(0).gameObject;
            HoldableItem item = obj.GetComponent<HoldableItem>();
            if (item)
            {
                HeldObject = item.PickUp(this.gameObject);
            }
        }
    }

    public void Charge()
    {
        if (ChargeLevel <= ChargeMax)
        {
            ChargeLevel += Time.deltaTime * ThrowSpeed;
        }

        PowerFill.fillAmount = ChargeLevel / 100;
    }


    public void Release()
    {
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
}
