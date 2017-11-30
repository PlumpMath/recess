using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
	public GameObject PlayerListItemPrefab;
	private Transform PlayerList;

	void Awake()
	{
		PlayerList = transform.Find("Player List");
	}

    void OnEnable()
    {
		Vital.PlayerController[] players = GameObject.FindObjectsOfType<Vital.PlayerController>();
		foreach(Vital.PlayerController p in players){
			GameObject listItem = Instantiate(PlayerListItemPrefab);
			listItem.transform.SetParent(PlayerList);

			listItem.GetComponent<PlayerListItem>().Player = p;
		}

		Canvas.ForceUpdateCanvases();
    }

	void OnDisable(){
		int l = PlayerList.childCount;
		if(l > 0){
            for (int i = l - 1; i >= 0; i--)
            {
                Destroy(PlayerList.GetChild(i).gameObject);
            }
		}
	}
}
