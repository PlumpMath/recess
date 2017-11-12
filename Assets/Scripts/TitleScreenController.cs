using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class TitleScreenController : MonoBehaviour {
	public NetworkManager networkManager;

	public void OnClickHostGame(){
		Debug.Log("Host a Game!");
		networkManager.StartHost();
		SceneManager.LoadScene("Main");
	}

	public void OnClickJoinGame(){
		Debug.Log("Join a Game!");
	}

	public void OnClickEditCharacter(){
		Debug.Log("Edit Character!");
	}

	public void OnClickQuit(){
		Debug.Log("Quit!");
		Application.Quit();
	}
}
