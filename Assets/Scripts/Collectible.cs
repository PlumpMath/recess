using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : NetworkBehaviour {

    [SyncVar (hook="OnVisibleChanged")]
    private bool visible;

    public bool Active = false;

    public void SetActive(bool a){
        Rigidbody rb = GetComponent<Rigidbody>();

        Active = a;
        rb.isKinematic = Active;
        rb.useGravity = !Active;
        GetComponent<Collider>().isTrigger = Active;
    }

    public int PointValue;

    void Start(){
        SetActive(Active);
    }

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
        if(!Active && other.gameObject.tag != "Player" && other.gameObject.tag != "Star"){
            Debug.Log("ACTIVATE IT!");
            SetActive(true);
        }
    }
}
