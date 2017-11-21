using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class HandController : NetworkBehaviour
{
    public static float interactSphereRadius = 1.0f;
    private const int INTERACTIVE = 1 << 8;
    public HoldableItem HeldObject;
    public float ThrowSpeed;
    private float ChargeLevel;
    private float ChargeMax = 100.0f;
    private NetworkIdentity networkIdentity;

    private Image PowerFill;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward, interactSphereRadius);
    }

    void Awake()
    {
        networkIdentity = GetComponent<NetworkIdentity>();

        GameObject pbf = GameObject.Find("Power Bar");
        if (pbf != null)
        {
            PowerFill = pbf.GetComponent<Image>();
        }
    }

    private GameObject ClosestInteractableObject(){
        GameObject ret = null;
        Vector3 C = transform.position + transform.forward;
        if(Physics.CheckSphere(C, interactSphereRadius, INTERACTIVE)){
            Collider[] found = Physics.OverlapSphere(C, interactSphereRadius, INTERACTIVE);
            var orderedFound = found.OrderBy(c =>
            {
                return (c.gameObject.transform.position - C).sqrMagnitude;
            });

            ret = orderedFound.ElementAt(0).gameObject;
        }

        return ret;
    }

    [Command]
    public void CmdGrab()
    {
        GameObject obj = ClosestInteractableObject();
        if(obj){

            Debug.LogFormat("Grab the {0}!", obj.name);

            NetworkIdentity objectIdentity = obj.GetComponent<NetworkIdentity>();
            NetworkConnection ownerConnection = objectIdentity.clientAuthorityOwner;
            NetworkConnection myConnection = networkIdentity.connectionToClient;
            if(myConnection == null){
                myConnection = networkIdentity.connectionToServer;
            }

            if(ownerConnection != null){
                if(ownerConnection == myConnection){
                    Debug.Log("Already the owner");
                }
            } else {
                Debug.Log("NO OWNER, GIMMIE DAT");
                objectIdentity.AssignClientAuthority(myConnection);

                HeldObject = obj.GetComponent<HoldableItem>().PickUp(gameObject);
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

    [Command]
    public void CmdRelease()
    {
        Vector3 TossDirection =
            (transform.forward * ChargeLevel) +
            (transform.up * 3.0f);

        HoldableItem releasedItem = HeldObject.Release(TossDirection);
        NetworkConnection myConnection = networkIdentity.connectionToClient;
        if (myConnection == null)
        {
            myConnection = networkIdentity.connectionToServer;
        }

        ChargeLevel = 0;
        PowerFill.fillAmount = 0;
        HeldObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(myConnection);
        HeldObject = null;
    }
}
