using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour {

	public void OnClickNewGame(){
		Debug.Log("New Game");
		SceneManager.LoadScene("Main");
	}

	public void OnClickQuit(){
		Debug.Log("Quit");
		Application.Quit();
	}
}
