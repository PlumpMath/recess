using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour {
    private float TimePassed;
    private float TimeTotal;
    private Image SpeedBoostIcon;
    public bool CoolDown;

    public void ActivatePower(float PowerUpTime) {
        float TimeTotal = PowerUpTime;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(PowerUpIcon(TimeTotal));
    }

    public IEnumerator PowerUpIcon(float TimeTotal) {
        GameObject SpeedBoost = GameObject.Find("Speed Boost");
        if (SpeedBoost != null) {
            SpeedBoostIcon = SpeedBoost.GetComponent<Image>();
            SpeedBoostIcon.fillAmount = 100;
            CoolDown = true;
        }

        while (TimePassed < TimeTotal) {
            TimePassed += Time.deltaTime;

            if(CoolDown == true ) {
                SpeedBoostIcon.fillAmount -= 1.0f / TimeTotal * Time.deltaTime;
            }

            if(SpeedBoostIcon.fillAmount <= 0) {
                CoolDown = false;
            }

            yield return null;
        }
    }

}