using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : NetworkBehaviour {

    [SyncVar (hook="OnVisibleChanged")]
    private bool visible;

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
        CmdPickMeUp();
    }
}
