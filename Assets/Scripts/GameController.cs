using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private bool selectingPlayers = false;

	public string [] joysticks;
	public int [] joysticksNumber;
	public int playerOne;

	[Header ("Prefabs")]
	[SerializeField] private GameObject player;

	[Header ("Game Objects")]
	[SerializeField] private Button playButton;
	[SerializeField] private GameObject playerSelection;
	[SerializeField] private GameObject players;

	[Header ("Colors")]
	[SerializeField] private Color playerJoined;
	[SerializeField] private Color playerReady;

	void Start ()
	{
		playButton.Select ();
		
		joysticks = null;
		
		Debug ();
	}

	void Update ()
	{
		if (selectingPlayers)
			PlayerSelection ();
	}

	void Debug ()
	{
		print (Input.GetJoystickNames ().Length + " controllers connected");

		for (int i = 0; i < Input.GetJoystickNames ().Length; i++)
			print (Input.GetJoystickNames () [i]);
	}

	public void ActivatePlayerSelection ()
	{
		if (playerSelection.activeInHierarchy)
		{
			playerSelection.SetActive (false);
			selectingPlayers = false;
			joysticks = null;
			joysticksNumber = null;
		}
		else
		{
			playerSelection.SetActive (true);
			Invoke ("InitializePlayerSelection", 0.1f);
			joysticks = new string [4];
			joysticksNumber = new int [4];
		}
	}

	void InitializePlayerSelection ()
	{
		selectingPlayers = true;
	}

	void PlayerSelection ()
	{
		for (int i = 1; i < 5; i++)
		{
			if (Input.GetButtonDown ("SubmitP" + i))
			{
				print (i);

				for (int j = 0; j < playerSelection.transform.childCount; j++)
				{
					Transform playerPanel = playerSelection.transform.GetChild (j);

					if (playerPanel.GetChild (0).gameObject.activeInHierarchy && joysticks [i - 1] == null)
					{
						playerPanel.GetChild (0).gameObject.SetActive (false);
						playerPanel.GetChild (1).gameObject.SetActive (true);

						playerPanel.GetComponent <Image> ().color = playerJoined;

						joysticks [i - 1] = "Joined";
						joysticksNumber [i - 1] = j;

						if (j == 0)
							playerOne = i;

						break;
					}
					else if (playerPanel.GetChild (1).gameObject.activeInHierarchy && joysticks [i - 1] == "Joined" && joysticksNumber [i - 1] == j)
					{
						playerPanel.GetChild (1).gameObject.SetActive (false);
						playerPanel.GetChild (2).gameObject.SetActive (true);

						playerPanel.GetComponent <Image> ().color = playerReady;

						joysticks [i - 1] = "Ready";

						break;
					}
				}
			}
		}

		if (joysticks [0] == "Ready" && joysticks [1] == "Ready")
		{
			int joinedPlayers = 0;
			int readyPlayers = 0;

			foreach (string status in joysticks)
			{
				if (status == "Joined")
					joinedPlayers++;
				
				if (status == "Ready")
					readyPlayers++;
			}

			if (readyPlayers >= 2 && joinedPlayers == 0)
				StartGame (readyPlayers);
		}
	}

	void StartGame (int playerAmount)
	{
		selectingPlayers = false;
		playerSelection.transform.parent.gameObject.SetActive (false);

		for (int i = 0; i < playerAmount; i++)
		{
			GameObject newPlayer = Instantiate (player, players.transform.GetChild (i).position, Quaternion.identity, players.transform);
			newPlayer.name = (joysticksNumber [i] + 1).ToString ();
		}

		for (int i = 0; i < 4; i++)
			Destroy (players.transform.GetChild (i).gameObject);
	}

	public void ExitGame ()
	{
		Application.Quit ();
	}
}
