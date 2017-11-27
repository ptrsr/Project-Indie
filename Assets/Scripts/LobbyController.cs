using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour {

	private bool selectingPlayers = false;

	private Dictionary <string, string> playerStatus;
	private int playerOne;
	private List <Color> playerColors;

	[Header ("Prefabs")]
	[SerializeField] private GameObject player;

	[Header ("Game Objects")]
	[SerializeField] private Transform players;
	[SerializeField] private GameObject readyText;
	[SerializeField] private Transform startPositions;

	[Header ("Textures/Materials")]

	[SerializeField] private Texture blue;
	[SerializeField] private Texture red, green, yellow;
	[SerializeField] private Material player1Mat, player2Mat, player3Mat, player4Mat;

	private GameController gameController;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();

		#if UNITY_EDITOR
		Debug ();
		#endif
	}

	void Update ()
	{
		if (selectingPlayers)
			PlayerSelection ();
		
		if (!selectingPlayers && !gameController.gameStarted && playerOne != 0)
		{
			if (Input.GetButtonDown ("SubmitP" + playerOne))
				RestartGame ();
		}
	}

	void Debug ()
	{
		print (Input.GetJoystickNames ().Length + " controllers connected");

		for (int i = 0; i < Input.GetJoystickNames ().Length; i++)
			print (Input.GetJoystickNames () [i]);
	}

	void PlayerSelection ()
	{
		JoinLobby ();
		BecomeReady ();
		ReadyCheck ();
	}

	IEnumerator InitiatePlayerStatus (string playerNumber, string status)
	{
		yield return new WaitForSeconds (0.01f);

		if (playerStatus.ContainsKey (playerNumber))
			playerStatus [playerNumber] = status;
		else
			playerStatus.Add (playerNumber, status);
	}

	void JoinLobby ()
	{
		for (int i = 1; i < 5; i++)
		{
			if (Input.GetButtonDown ("SubmitP" + i))
			{
				string playerNumber = "Player " + i;

				if (!playerStatus.ContainsKey (playerNumber))
				{
					print (playerNumber + " joined");

					StartCoroutine (InitiatePlayerStatus (playerNumber, "Joined"));

					if (players.childCount == 0)
						playerOne = i;

					Vector3 spawnPosition;
					if (players.childCount > 0)
						spawnPosition = players.GetChild (players.childCount - 1).position + new Vector3 (5.0f, 0.0f, 0.0f);
					else
						spawnPosition = new Vector3 (-14.0f, 2.2f, 5.9f);

					GameObject newPlayer = Instantiate (player, spawnPosition, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);
					newPlayer.name = i.ToString ();
					PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
					playerController.enabled = false;

					//Colors
					int randomNumber = Random.Range (0, playerColors.Count);
					playerController.playerColor = playerColors [randomNumber];
					playerColors.Remove (playerColors [randomNumber]);

					AssignColors (playerController, i);
				}
			}
		}
	}

	void AssignColors (PlayerController playerController, int playerNumber)
	{
		Renderer bodyRenderer = playerController.transform.GetChild (0).GetChild (0).GetComponent <Renderer> ();

		foreach (Renderer mat in bodyRenderer.GetComponentsInChildren <Renderer> ())
		{
			switch (playerNumber)
			{
			case 1:
				mat.material = player1Mat;
				break;
			case 2:
				mat.material = player2Mat;
				break;
			case 3:
				mat.material = player3Mat;
				break;
			case 4:
				mat.material = player4Mat;
				break;
			}
		}

		if (playerController.playerColor == Color.blue)
			bodyRenderer.sharedMaterial.mainTexture = blue;
		else if (playerController.playerColor == Color.red)
			bodyRenderer.sharedMaterial.mainTexture = red;
		else if (playerController.playerColor == Color.green)
			bodyRenderer.sharedMaterial.mainTexture = green;
		else if (playerController.playerColor == Color.yellow)
			bodyRenderer.sharedMaterial.mainTexture = yellow;
	}

	void BecomeReady ()
	{
		for (int i = 1; i < 5; i++)
		{
			string playerNumber = "Player " + i;

			if (playerStatus.ContainsKey (playerNumber) && Input.GetButtonDown ("SubmitP" + i))
			{
				if (playerStatus [playerNumber] == "Joined")
				{
					print (playerNumber + " is ready");

					players.GetChild (i - 1).GetChild (0).GetComponent <Animator> ().SetInteger ("playerClip", 3);

					StartCoroutine (InitiatePlayerStatus (playerNumber, "Ready"));
				}
			}
		}
	}

	void ReadyCheck ()
	{
		int joinedPlayers = 0;
		int readyPlayers = 0;

		for (int i = 1; i < 5; i++)
		{
			string playerNumber = "Player " + i;

			if (playerStatus.ContainsKey (playerNumber))
			{
				if (playerStatus [playerNumber] == "Joined")
					joinedPlayers++;
				else if (playerStatus [playerNumber] == "Ready")
					readyPlayers++;
			}
		}

		if (readyPlayers >= 1 && joinedPlayers == 0)
			FinishLobby (true);
		else
			FinishLobby (false);
	}

	//True if it's ready, false if it's not
	void FinishLobby (bool status)
	{
		if (status)
		{
			if (!readyText.activeInHierarchy)
			{
				print ("Game is ready to start");

				readyText.SetActive (true);
			}

			if (Input.GetButtonDown ("SubmitP" + playerOne))
				StartCoroutine (StartGame ());
		}
		else
		{
			if (readyText.activeInHierarchy)
				readyText.SetActive (false);
		}
	}

	IEnumerator StartGame ()
	{
		transform.root.GetComponent <Menu> ().SendCommand (0);

		gameController.SetupGame (players);

		//Colors
		playerColors.Clear ();
		for (int i = 0; i < players.childCount; i++)
			playerColors.Add (players.GetChild (i).GetComponent <PlayerController> ().playerColor);

		yield return new WaitForSeconds (1.0f);

		selectingPlayers = false;

		for (int i = 0; i < players.childCount; i++)
		{
			Transform tempPlayer = players.GetChild (i);
			tempPlayer.position = startPositions.GetChild (i).position;
		}
	}

	void RestartGame ()
	{
		int childCount = players.childCount;

		if (childCount > 0)
		{
			for (int i = 0; i < childCount; i++)
				Destroy (players.GetChild (i).gameObject);
		}

		for (int i = 1; i < gameController.playerAmount + 1; i++)
		{
			GameObject newPlayer = Instantiate (player, startPositions.GetChild (i - 1).position, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);
			newPlayer.name = i.ToString ();

			PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
			playerController.playerColor = playerColors [i - 1];
			playerController.enabled = false;

			AssignColors (playerController, i);
		}

		gameController.Restart ();

		gameController.InitializeGame (0.0f);
	}

	//True when starting lobby and false when exiting
	public void ResetLobby (bool status)
	{
		if (status)
		{
			playerStatus = new Dictionary <string, string> ();
			playerColors = new List <Color> {Color.blue, Color.red, Color.green, Color.yellow};
			Invoke ("ActivateLobbyFunctionality", 0.1f);
		}
		else
		{
			selectingPlayers = false;
			playerStatus = null;
			playerColors = null;
			playerOne = 0;
		}
	}

	void ActivateLobbyFunctionality ()
	{
		selectingPlayers = true;
	}
}
