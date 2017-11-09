using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

    public static TextController instance;

    void Awake() {
        instance = this;
    }

    public Text ballText;

}
