using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TextController : MonoBehaviour {
    private bool ShowScoreboard;
    public GameObject Scoreboard;

    void Update(){
        if(Input.GetKeyDown(KeyCode.Tab)){
            ShowScoreboard = !ShowScoreboard;
        }
    }

    void OnGUI(){
        Scoreboard.SetActive(ShowScoreboard);
    }


}
