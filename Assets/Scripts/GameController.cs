using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[Header ("Prefabs")]
	[SerializeField] private GameObject cooldownBar;

	[Header ("Game Objects")]
	[SerializeField] private Transform cooldownBars;

	public void InitializeGame ()
	{
		InitializeCooldownBars ();
	}

	void InitializeCooldownBars ()
	{
		foreach (GameObject player in GameObject.FindGameObjectsWithTag ("Player"))
		{
			GameObject cooldown = Instantiate (cooldownBar, cooldownBars);
			cooldown.transform.GetComponentInChildren <Text> ().text = player.name;

			player.GetComponent <PlayerController> ().cooldownBar = cooldown.GetComponent <Image> ();
		}
	}
}
