using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[Header ("Prefabs")]
	[SerializeField] private GameObject cooldownBar;

	[Header ("Game Objects")]
	[SerializeField] private Transform cooldownBars;
	[SerializeField] private Text countdown;
	[SerializeField] private Text winText;

	private Transform players;

	[HideInInspector] public int playerAmount;
	[HideInInspector] public bool gameStarted;

	public Dictionary <string, int> victories;

	public void SetupGame (Transform _players)
	{
		players = _players;
		playerAmount = players.childCount;

		victories = new Dictionary <string, int> ();

		for (int i = 0; i < playerAmount; i++)
			victories.Add (players.GetChild (i).name, 0);

		winText.gameObject.SetActive (false);

		InitializeGame (1.0f);
	}

	public void InitializeGame (float waitTime)
	{
		InitializeCooldownBars ();
		gameStarted = true;
		StartCoroutine (StartCountdown (waitTime));
	}

	void InitializeCooldownBars ()
	{
		if (cooldownBars.childCount > 0)
		{
			for (int i = 0; i < cooldownBars.childCount; i++)
				Destroy (cooldownBars.GetChild (i).gameObject);
		}

		Invoke ("InstantiateCooldownBars", 0.01f);
	}

	void InstantiateCooldownBars ()
	{
		for (int i = 0; i < playerAmount; i++)
		{
			GameObject cooldown = Instantiate (cooldownBar, cooldownBars);
			cooldown.transform.GetComponentInChildren <Text> ().text = players.GetChild (i).name;

			players.GetChild (i).GetComponent <PlayerController> ().cooldownBar = cooldown.GetComponent <Image> ();
		}
	}

	IEnumerator StartCountdown (float waitTime)
	{
		yield return new WaitForSeconds (waitTime);

		for (int i = 3; i > 0; i--)
		{
			countdown.text = i.ToString ();
			yield return new WaitForSeconds (0.5f);
		}

		countdown.text = "GO";

		for (int i = 0; i < players.childCount; i++)
			players.GetChild (i).GetComponent <PlayerController> ().enabled = true;

		yield return new WaitForSeconds (1.0f);

		countdown.text = "";
	}

	void Update ()
	{
		if (gameStarted)
			CheckForVictory ();
	}

	void CheckForVictory ()
	{
		if (playerAmount == 1)
			return;
		
		if (players.childCount == 1)
		{
			gameStarted = false;

			foreach (GameObject bullet in GameObject.FindGameObjectsWithTag ("Bullet"))
				Destroy (bullet);

			Transform player = players.GetChild (0);
			player.GetComponent <PlayerController> ().enabled = false;

			victories [player.name]++;

			winText.text = "..." + " player wins this round!";
			winText.gameObject.SetActive (true);
			print ("Player " + player.name + " has " + victories [player.name] + " wins!");

			CheckForTotalVictory (player.name);
		}
	}

	void CheckForTotalVictory (string playerName)
	{
		if (victories [playerName] == Modifiers.pointsToVictory)
		{
			victories = null;
			print ("Player " + playerName + " wins the set!");
		}
	}

	public void Restart ()
	{
		winText.gameObject.SetActive (false);
	}
}
