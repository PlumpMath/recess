using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : NetworkBehaviour {

    [SyncVar (hook="OnVisibleChanged")]
    private bool visible;

    private bool _active = false;
    public bool Active{
        get { return _active; }
        set {
            Rigidbody rb = GetComponent<Rigidbody>();

            _active = value;
            rb.isKinematic = _active;
            rb.useGravity = !_active;
            GetComponent<Collider>().isTrigger = _active;
        }
    }

    public int PointValue;

    public override void OnStartServer(){
        visible = true;
    }

    public override void OnStartClient(){
        OnVisibleChanged(visible);
    }

    void OnVisibleChanged(bool isVisible){
        GetComponent<Collider>().enabled = isVisible;
        GetComponentInChildren<MeshRenderer>().enabled = isVisible;
        if(isVisible){
            GetComponentInChildren<ParticleSystem>().Play();
        } else {
            GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    [Command]
    public void CmdPickMeUp(){
        visible = false;
	}

    public void PickMeUp(){
        if(!Active){
            return;
        }
        CmdPickMeUp();
    }

    void OnCollisionEnter(Collision other){
        Debug.LogFormat("Collision with star and {0}", other.gameObject.name);
        if(!Active){
            Debug.Log("ACTIVATE IT!");
            Active = true;
        }
    }
}
