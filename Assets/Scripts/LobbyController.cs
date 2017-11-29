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

	private bool selectingPlayers = false;

	private Dictionary <Player, Status> playerStatus;

	[Header ("Prefabs")]
	[SerializeField] private GameObject playerPrefab;

	[Header ("Game Objects")]
	[SerializeField] private Transform players;
	[SerializeField] private GameObject readyText;
	[SerializeField] private Transform startPositions;
	[SerializeField] private GameObject gameCanvas;
	[SerializeField] private GameObject lobbyCanvas;
	[SerializeField] private GameObject mainCanvas;
	[SerializeField] private GameObject exitConfirmPanel;

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
		print (player);

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

		print (player + " joined");

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
			players.GetChild (i).position = new Vector3 (4.8f + (i * 5), 0.9f, 14.0f);
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
		}
		else
		{
			if (readyText.activeInHierarchy)
				readyText.SetActive (false);
		}
	}

	IEnumerator StartGame ()
	{
		gameCanvas.SetActive (true);
		lobbyCanvas.SetActive (false);
		mainCanvas.SetActive (false);
		Invoke ("DeactivateLobbyCanvas", 1.0f);
		transform.root.GetComponent <Menu> ().SendCommand (0);

		gameController.SetupGame (players);

		//Colors
		for (int i = 0; i < players.childCount; i++)
			players.GetChild (i).GetComponent <PlayerController> ().AssignColor ();

		selectingPlayers = false;

		yield return new WaitForSeconds (1.0f);

		if (gameController.gameStarted)
		{
			for (int i = 0; i < players.childCount; i++)
			{
				Transform tempPlayer = players.GetChild (i);
				tempPlayer.position = startPositions.GetChild (i).position;

				players.GetChild (i).GetComponent <PlayerController> ().bodyAnim.SetInteger ("playerClip", 0);
			}
		}
	}

	public void NewRound ()
	{
		StartCoroutine (NewRoundTransition ());
	}

	IEnumerator NewRoundTransition ()
	{
		yield return new WaitForSeconds (4.0f);

		RestartGame ();
	}

	void RestartGame ()
	{
		RemoveAllPlayers ();

		for (int i = 0; i < gameController.playerAmount; i++)
		{
			GameObject newPlayer = Instantiate (playerPrefab, startPositions.GetChild (i).position, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);
			newPlayer.name = ((int) playerStatus.Keys.ElementAt (i)).ToString ();

			PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
			playerController.playerNumber = playerStatus.Keys.ElementAt (i);
			playerController.AssignColor ();
			playerController.enabled = false;
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
