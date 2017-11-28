using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : NetworkBehaviour {

    private int _gameTimeElapsed;
	public int GameLength; //In seconds

	private Dictionary<Vital.PlayerController, int> Scoreboard;

	void Awake(){
		Debug.Log("AWAKE!");
	}

	override public void OnStartServer(){
		Scoreboard = new Dictionary<Vital.PlayerController, int>();
		Debug.Log("OnStartServer()");

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject p in players){
			Debug.Log(p.gameObject.name);
			Vital.PlayerController pc = p.GetComponent<Vital.PlayerController>();
			if(pc){
				Debug.LogFormat("Add {0} to Scoreboard with a score of 0", pc.PlayerName);
				Scoreboard.Add(pc, 0);
			}
		}
	}

	void OnPlayerConnected(NetworkPlayer player){
		Debug.Log("Player connected!");
	}

	void CmdGivePlayerStar(){

	}

	void CmdTakePlayerStar(){

	}
}
