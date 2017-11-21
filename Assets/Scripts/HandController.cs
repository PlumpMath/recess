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
    private float _chargeLevel;
    private float ChargeLevel {
        get { return _chargeLevel; }
        set {
            _chargeLevel = Mathf.Clamp(value, 0f, 100.0f);
            Debug.LogFormat("Charge set to {0}/100", _chargeLevel);
        }
    }
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
    void CmdSetAuthority(NetworkIdentity itemID)
    {
        itemID.AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CmdRemoveAuthority(NetworkIdentity itemID)
    {
        itemID.RemoveClientAuthority(connectionToClient);
    }

    [Command]
    public void CmdGrab()
    {
        GameObject obj = ClosestInteractableObject();
        if(obj){
            Debug.LogFormat("Grab the {0}!", obj.name);

            NetworkIdentity objectIdentity = obj.GetComponent<NetworkIdentity>();
            NetworkConnection ownerConnection = objectIdentity.clientAuthorityOwner;

            if(ownerConnection == null){
                CmdSetAuthority(objectIdentity);
                HeldObject = obj.GetComponent<HoldableItem>().PickUp(gameObject);
                RpcGrab(objectIdentity);
            }
        }
    }

    [ClientRpc]
    public void RpcGrab(NetworkIdentity item){
        GameObject obj = ClientScene.FindLocalObject(item.netId);
        if(obj){
            HeldObject = obj.GetComponent<HoldableItem>();
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

    public void Release(){
        Debug.LogFormat("Release with charge level {0}", ChargeLevel);
        Vector3 TossDirection =
            (transform.forward * ChargeLevel) +
            (transform.up * 3.0f);

        CmdRelease(TossDirection);

        ChargeLevel = 0;
        PowerFill.fillAmount = 0;
        HeldObject = null;
    }

    [Command]
    public void CmdRelease(Vector3 TossDirection)
    {
        HeldObject.Release(TossDirection);
        CmdRemoveAuthority(HeldObject.GetComponent<NetworkIdentity>());
    }
}
