using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour {

	public void OnClickStartGame(){
		SceneManager.LoadScene("Game Lobby");
	}

	public void OnClickQuit(){
		Debug.Log("Quit!");
		Application.Quit();
	}
}
