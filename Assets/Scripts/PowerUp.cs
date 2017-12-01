using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour {
    private float TimePassed;
    private float TimeTotal;
    private Image BoostIconActive;
    public bool CoolDown;

    public void ActivatePower(GameObject BoostIcon, float PowerUpTime) {
        BoostIcon = BoostIcon.transform.Find("Boost Active").gameObject;
        float TimeTotal = PowerUpTime;

        foreach (Transform child in transform) {
            child.GetComponent<MeshRenderer>().enabled = false;
        }
        
        GetComponent<Collider>().enabled = false;
        StartCoroutine(PowerUpIcon(BoostIcon, TimeTotal));
    }

    public IEnumerator PowerUpIcon(GameObject BoostIcon, float TimeTotal) {
        if (BoostIcon != null) {
            BoostIconActive = BoostIcon.GetComponent<Image>();
            BoostIconActive.fillAmount = 100;
            CoolDown = true;
        }

        while (TimePassed < TimeTotal) {
            TimePassed += Time.deltaTime;

            if(CoolDown == true ) {
                BoostIconActive.fillAmount -= 1.0f / TimeTotal * Time.deltaTime;
            }

            if(BoostIconActive.fillAmount <= 0) {
                CoolDown = false;
            }

            yield return null;
        }
    }

}