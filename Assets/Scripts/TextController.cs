using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TextController : MonoBehaviour {

    public static TextController instance;

    void Awake() {
        instance = this;
    }


}
