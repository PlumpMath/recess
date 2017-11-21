using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : MonoBehaviour {
    private Image StarIcon;
    private Text StarCountLabl;
    private float StarCount;
    private float TotalStars;

    public void PickMeUp(){
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
        GetComponent<ParticleSystem>().Stop();
	}

    public void AddOneStar() {
        StarCount += StarCount + 1;
        ActivateStar(StarCount);
    }

    public void ActivateStar(float StarCount) {
        float TotalStars = StarCount;
        GameObject Star = GameObject.Find("Star Full");
        GameObject StarCountLabel = GameObject.Find("Star Count");
        StarIcon = Star.GetComponent<Image>();
        StarIcon.fillAmount = 100;
        StarCountLabel.GetComponent<Text>().enabled = true;
        StarCountLabel.GetComponent<Text>().text = TotalStars.ToString();
    }
}
