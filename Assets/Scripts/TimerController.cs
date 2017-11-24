using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour {
     float TimeLeft = 300.0f;
     
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
    }

 }
