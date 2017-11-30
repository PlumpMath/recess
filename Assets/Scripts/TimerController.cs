using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimerController : NetworkBehaviour {
     float TimeLeft = 120.0f;

     public Text Timer;
     private float Minutes;
     private float Seconds;

     void Update() {

         TimeLeft -= Time.deltaTime;

         Minutes = Mathf.Floor(TimeLeft / 60);
         Seconds = TimeLeft % 60;

         Timer.text = string.Format("{0:0}:{1:00}", Minutes, Seconds);

         if(TimeLeft < 0) {
             GameOver();
         }

     }

    public void GameOver() {
        Timer.text = "GameOver";
        Invoke("BackToLobby", 6.0f);
    }

    private void BackToLobby(){
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }

 }
