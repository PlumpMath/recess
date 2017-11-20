using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour {
    private float TimePassed;
    private float TimeTotal;
    private Image SpeedBoostIcon;


    public void ActivatePower(float PowerUpTime) {
        float TimeTotal = PowerUpTime;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(PowerUpIcon(TimeTotal));
    }

    IEnumerator PowerUpIcon(float TimeTotal) {

        GameObject SpeedBoost = GameObject.Find("Speed Boost");
        if (SpeedBoost != null) {
            SpeedBoostIcon = SpeedBoost.GetComponent<Image>();
            SpeedBoostIcon.fillAmount = 100;
        }

        while (TimePassed < TimeTotal) {
            TimePassed += Time.deltaTime;
            SpeedBoostIcon.fillAmount -= (TimePassed / 100) / TimeTotal;
            yield return null;
        }
    }

}