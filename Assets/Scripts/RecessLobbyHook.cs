using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using Vital;

public class RecessLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(
		NetworkManager manager,
		GameObject lobbyPlayer,
		GameObject gamePlayer
	){
		LobbyPlayer lplayer = lobbyPlayer.GetComponent<LobbyPlayer>();
		Vital.PlayerController player = gamePlayer.GetComponent<Vital.PlayerController>();

		player.PlayerName = lplayer.playerName;
		player.PlayerColor = lplayer.playerColor;
	}
}
