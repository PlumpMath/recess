using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class RoadController : MonoBehaviour {

    public GameObject truckPrefab;

    public void SpawnTruck() {
        Debug.Log("spawn truck");
        GameObject truck = Instantiate(truckPrefab, new Vector3(40, 2, -120), Quaternion.Euler(0, 0, 0));

        Vector3 pos = new Vector3(0, 0, 0);
        pos.z += 200;

        truck.GetComponent<Rigidbody>().velocity = pos;

        NetworkServer.Spawn(truck);
    }

}