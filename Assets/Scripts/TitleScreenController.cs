using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class TitleScreenController : MonoBehaviour {
	public NetworkManager networkManager;

	public void OnClickStartGame(){
		SceneManager.LoadScene("Game Lobby");
	}

	public void OnClickQuit(){
		Debug.Log("Quit!");
		Application.Quit();
	}
}
