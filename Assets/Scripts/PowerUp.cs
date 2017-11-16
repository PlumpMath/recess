using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;


public class PowerUp : MonoBehaviour {

    public void ActivatePower(GameObject other, PlayerController player) {
        GameObject p = other.gameObject;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Debug.LogFormat("Obtained the {0}!", p.name);

        if (p.name == "Pumps") {
            player.movementSettings.WalkSpeed = 20.0f;
        }
    }

}