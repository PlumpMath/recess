using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour {
	private Vital.PlayerController _player;

	public Vital.PlayerController Player{
        get { return _player; }
        set
        {
            _player = value;

            NameField.text = _player.PlayerName;
            ColorChip.color = _player.PlayerColor;
            ScoreField.text = _player.StarCount.ToString();
        }
    }

	private Text NameField;
	private Text ScoreField;
	private Image ColorChip;

	void Awake(){
		NameField = transform.Find("Player Name").GetComponent<Text>();
		ScoreField = transform.Find("Player Score").GetComponent<Text>();
		ColorChip = transform.Find("Player Color").GetComponent<Image>();
	}
}
