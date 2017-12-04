using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Players;
using System.Linq;

public class LobbyController : SubMenu {

	private enum Status
	{
		joined,
		ready
	}

	[HideInInspector] public bool selectingPlayers = false;

	private Dictionary <Player, Status> playerStatus;

	[Header ("Prefabs")]
	[SerializeField] private GameObject playerPrefab;

	[Header ("Game Objects")]
	[SerializeField] private Transform players;
	[SerializeField] private Transform startPositions;
	[SerializeField] private GameObject gameCanvas;
	[SerializeField] private GameObject lobbyCanvas;
	[SerializeField] private GameObject mainCanvas;
	public GameObject settingsCanvas;
	[SerializeField] private GameObject exitConfirmPanel;
	[SerializeField] private Transform maps;
	[SerializeField] private GameObject controlsCanvas;

	private GameController gameController;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();
		DeactivateLobbyCanvas ();
		exitConfirmPanel.SetActive (false);

		#if UNITY_EDITOR
		Debug ();
		#endif
	}

	void Update ()
	{
		if (selectingPlayers)
			CheckForPlayerCancel ();

		if (InputHandler.GetButtonDown (Player.P1, Players.Button.Settings) && selectingPlayers)
		{
			settingsCanvas.SetActive (true);
			ServiceLocator.Locate <Menu> ().SendCommand (2);

			if (exitConfirmPanel.activeInHierarchy)
				exitConfirmPanel.SetActive (false);

			Invoke ("DisableThis", 0.01f);
		}

		if (controlsCanvas.activeInHierarchy)
		{
			if (InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Cancel))
				controlsCanvas.SetActive (false);
		}
	}

	void DisableThis ()
	{
		this.enabled = false;
	}

	public override GameObject EnableMenu ()
	{
		InputHandler.ready += ProcessInput;
		Input.ResetInputAxes ();

		return base.EnableMenu ();
	}

	public override void DisableMenu ()
	{
		InputHandler.ready -= ProcessInput;

		base.DisableMenu ();
	}

	void Debug ()
	{
		print (Input.GetJoystickNames ().Length + " controllers connected");

		for (int i = 0; i < Input.GetJoystickNames ().Length; i++)
			print (Input.GetJoystickNames () [i]);
	}

	void ProcessInput (Player player)
	{
		if (!gameController.gameStarted && selectingPlayers)
		{
			if (playerStatus.ContainsKey (player))
				BecomeReady (player);
			else if (!exitConfirmPanel.activeInHierarchy)
				JoinLobby (player);
		}
	}

	void CheckForPlayerCancel ()
	{
		if (!playerStatus.ContainsKey (Player.P1))
		{
			if (InputHandler.GetButtonDown (Player.P1, Players.Button.Cancel))
			{
				if (exitConfirmPanel.activeInHierarchy)
					exitConfirmPanel.SetActive (false);
				else
					exitConfirmPanel.SetActive (true);

				return;
			}

			if (InputHandler.GetButtonDown (Player.P1, Players.Button.Submit) && exitConfirmPanel.activeInHierarchy)
			{
				BackToMainMeny ();
				return;
			}
		}

		Player player = Players.Player.P1;

		for (int i = 0; i < 5; i++)
		{
			switch (i)
			{
			case 0:
				player = Players.Player.P1;
				break;
			case 1:
				player = Players.Player.P2;
				break;
			case 2:
				player = Players.Player.P3;
				break;
			case 3:
				player = Players.Player.P4;
				break;
			case 4:
				player = Players.Player.P5;
				break;
			}

			if (playerStatus.ContainsKey (player))
			{
				if (InputHandler.GetButtonDown (player, Players.Button.Cancel))
					LeaveLobby (player);
			}
		}
	}

	void JoinLobby (Player player)
	{
		if (playerStatus.ContainsKey (player))
			playerStatus.Remove (player);

		playerStatus.Add (player, Status.joined);

		GameObject newPlayer = Instantiate (playerPrefab, Vector3.zero, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);
		newPlayer.name = ((int) player).ToString ();
		PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
		playerController.playerNumber = player;
		playerController.AssignColor ();
		playerController.enabled = false;

		SetPlayersPosition ();
	}

	void SetPlayersPosition ()
	{
		for (int i = 0; i < players.childCount; i++)
		{
			Transform newPlayer = players.GetChild (i);
			newPlayer.position = new Vector3 (78.8f + (i * 5), 2.24f, 2.5f);
			newPlayer.rotation = Quaternion.Euler (new Vector3 (newPlayer.rotation.x, 90.0f, newPlayer.rotation.z));
		}
		
			//players.GetChild (i).position = new Vector3 (10.8f + (i * 5), 0.9f, 14.0f);
	}

	void BecomeReady (Player player)
	{
		playerStatus [player] = Status.ready;

		for (int i = 0; i < players.childCount; i++)
		{
			if (players.GetChild (i).name == ((int)player).ToString ())
				players.GetChild (i).GetChild (0).GetComponent <Animator> ().SetInteger ("playerClip", 3);
		}

		if (ReadyCheck () && selectingPlayers)
			StartCoroutine (StartGame ());
	}

	bool ReadyCheck ()
	{
//		if (playerStatus.Count < 2)
//			return false;

		if (!playerStatus.ContainsKey (Player.P1))
			return false;

		for (int i = 0; i < playerStatus.Count; i++)
		{
			if (playerStatus.Values.ElementAt (i) != Status.ready)
				return false;
		}

		return true;
	}

	IEnumerator StartGame ()
	{
		selectingPlayers = false;

		yield return new WaitForSeconds (1.0f);

		gameCanvas.SetActive (true);
		lobbyCanvas.SetActive (false);
		mainCanvas.SetActive (false);
		Invoke ("DeactivateLobbyCanvas", 1.0f);
		transform.root.GetComponent <Menu> ().SendCommand (0);

		gameController.SetupGame (players);

		//Colors
		for (int i = 0; i < players.childCount; i++)
		{
			PlayerController playerController = players.GetChild (i).GetComponent <PlayerController> ();
			
			playerController.AssignColor ();
			playerController.anim.SetBool ("Moving", false);
		}

		yield return new WaitForSeconds (1.0f);

		if (gameController.gameStarted)
		{
			int playerCount = players.childCount;

			print ("PlayerCount: " + playerCount);

			List <int> startPos = new List <int> ();

			for (int j = 0; j < playerCount; j++)
				startPos.Add (j);

			print ("StartposCount: " + startPos.Count);

			for (int i = 0; i < playerCount; i++)
			{
				Transform tempPlayer = players.GetChild (i);

				int randomNumber = Random.Range (0, startPos.Count);

				print ("Random: " + randomNumber);

				if (maps.GetChild (0).gameObject.activeInHierarchy)
					tempPlayer.position = startPositions.GetChild (0).GetChild (startPos [randomNumber]).position;
				else if (maps.GetChild (1).gameObject.activeInHierarchy)
					tempPlayer.position = startPositions.GetChild (1).GetChild (startPos [randomNumber]).position;
				
				startPos.Remove (startPos [randomNumber]);

				tempPlayer.GetComponent <PlayerController> ().bodyAnim.SetInteger ("playerClip", 0);

				tempPlayer.GetChild (1).GetChild (2).gameObject.SetActive (true);

				//Not random
				//tempPlayer.position = startPositions.GetChild ((int) tempPlayer.GetComponent <PlayerController> ().playerNumber - 1).position;
			}
		}
	}

	public void NewRound ()
	{
		StartCoroutine (NewRoundTransition ());
	}

	IEnumerator NewRoundTransition ()
	{
		yield return new WaitForSeconds (3.0f);

		RestartGame ();
	}

	void RestartGame ()
	{
		RemoveAllPlayers ();

		int playerCount = gameController.playerAmount;

		List <int> startPos = new List <int> ();

		for (int j = 0; j < playerCount; j++)
			startPos.Add (j);

		for (int i = 0; i < playerCount; i++)
		{
			GameObject newPlayer = Instantiate (playerPrefab, Vector3.zero, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);

			int randomNumber = Random.Range (0, startPos.Count);

			if (maps.GetChild (0).gameObject.activeInHierarchy)
				newPlayer.transform.position = startPositions.GetChild (0).GetChild (startPos [randomNumber]).position;
			else if (maps.GetChild (1).gameObject.activeInHierarchy)
				newPlayer.transform.position = startPositions.GetChild (1).GetChild (startPos [randomNumber]).position;

			startPos.Remove (startPos [randomNumber]);

			//Not random
			//newPlayer.transform.position = startPositions.GetChild ((int) playerStatus.Keys.ElementAt (i) - 1).position;

			newPlayer.name = ((int) playerStatus.Keys.ElementAt (i)).ToString ();

			PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
			playerController.playerNumber = playerStatus.Keys.ElementAt (i);
			playerController.AssignColor ();
			playerController.enabled = false;

			newPlayer.transform.GetChild (1).GetChild (2).gameObject.SetActive (true);
		}

		gameController.Restart ();
		gameController.InitializeGame (0.0f);
	}

	public void RemoveAllPlayers ()
	{
		int childCount = players.childCount;
		if (childCount > 0)
		{
			for (int i = 0; i < childCount; i++)
				Destroy (players.GetChild (i).gameObject);
		}
	}

	public void FirstInitializeLobby ()
	{
		ResetLobby (true, false);
		lobbyCanvas.SetActive (true);
	}

	//True when starting lobby and false when exiting - second bool true when going back to lobby from game, otherwise false
	public void ResetLobby (bool status, bool backToLobby)
	{
		if (status)
		{
			if (!backToLobby)
				playerStatus = new Dictionary <Player, Status> ();
			gameCanvas.SetActive (false);
			Invoke ("ActivateLobbyFunctionality", 0.01f);
		}
		else
		{
			selectingPlayers = false;
			playerStatus = null;
			RemoveAllPlayers ();

			Invoke ("DeactivateLobbyCanvas", 1.0f);
		}
	}

	public void BackToLobby ()
	{
		Invoke ("LobbyJoinInvoke", 0.01f);
		lobbyCanvas.SetActive (true);
	}

	void LobbyJoinInvoke ()
	{
		for (int i = 0; i < playerStatus.Count; i++)
		{
			print (playerStatus.Keys.ElementAt (i));
			JoinLobby (playerStatus.Keys.ElementAt (i));
		}
	}

	void LeaveLobby (Player player)
	{
		for (int i = 0; i < players.childCount; i++)
		{
			PlayerController playerController = players.GetChild (i).GetComponent <PlayerController> ();
			if (playerController.playerNumber == player)
			{
				if (playerStatus [player] == Status.ready)
				{
					playerController.bodyAnim.SetInteger ("playerClip", 0);
					playerStatus [player] = Status.joined;
				}
				else
				{
					playerStatus.Remove (player);
					Destroy (playerController.gameObject);
					Invoke ("SetPlayersPosition", 0.01f);
				}

				break;
			}
		}
	}

	void DeactivateLobbyCanvas ()
	{
		if (gameController.gameStarted || selectingPlayers)
			return;
		
		lobbyCanvas.SetActive (false);
	}

	public void BackToMainMeny ()
	{
		transform.root.GetComponent <Menu> ().SendCommand (1);
		ResetLobby (false, false);
		mainCanvas.SetActive (true);
		exitConfirmPanel.SetActive (false);

		//Fail-safe
		foreach (UnityEngine.UI.Button button in mainCanvas.GetComponentsInChildren <UnityEngine.UI.Button> ())
			button.interactable = true;
	}

	void ActivateLobbyFunctionality ()
	{
		selectingPlayers = true;
	}

	public void ExitGame ()
	{
		Application.Quit ();
	}
}
