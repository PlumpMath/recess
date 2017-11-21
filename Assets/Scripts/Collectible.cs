using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : MonoBehaviour {
    private Image StarIcon;
    private Text StarCountLabel;
    private int StarCount;
    private float TotalStars;

    public void PickMeUp(){
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
        GetComponent<ParticleSystem>().Stop();
	}

    public void AddStars(int StarCount) {
        GameObject Star = GameObject.Find("Star Full");
        GameObject StarCountLabel = GameObject.Find("Star Count");
        StarIcon = Star.GetComponent<Image>();
        StarIcon.fillAmount = 100;
        StarCountLabel.GetComponent<Text>().enabled = true;
        StarCountLabel.GetComponent<Text>().text = StarCount.ToString();
        Debug.Log(StarCount);
    }
}
