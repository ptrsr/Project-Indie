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
	private List <Color> playerColors;

	[Header ("Prefabs")]
	[SerializeField] private GameObject playerPrefab;

	[Header ("Game Objects")]
	[SerializeField] private Transform players;
	[SerializeField] private GameObject readyText;
	[SerializeField] private Transform startPositions;

	private GameController gameController;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();

		#if UNITY_EDITOR
		Debug ();
		#endif
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

	void Update ()
	{
		if (selectingPlayers)
			PlayerSelection ();
		
		if (!selectingPlayers && !gameController.gameStarted && gameController.gameFinished) //&& playerOne != 0)
		{
			if (InputHandler.GetButtonDown (Player.P1, Players.Button.Fire)) //Submit
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
		ReadyCheck ();
	}

	void ProcessInput (Player player)
	{
		if (playerStatus.ContainsKey (player))
			BecomeReady (player);
		else
			JoinLobby (player);
	}

	void JoinLobby (Player player)
	{
		playerStatus.Add (player, Status.joined);

		print ((int) player + " joined");

		Vector3 spawnPosition;
		if (players.childCount > 0)
			spawnPosition = players.GetChild (players.childCount - 1).position + new Vector3 (5.0f, 0.0f, 0.0f);
		else
			spawnPosition = new Vector3 (-14.0f, 2.2f, 5.9f);

		GameObject newPlayer = Instantiate (playerPrefab, spawnPosition, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);
		newPlayer.name = ((int) player).ToString ();
		PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
		playerController.playerNumber = player;
		playerController.AssignColor ();
		playerController.enabled = false;
	}

	void BecomeReady (Player player)
	{
		playerStatus [player] = Status.ready;

		//players.GetChild (i - 1).GetChild (0).GetComponent <Animator> ().SetInteger ("playerClip", 3);

		if (ReadyCheck ())
			StartCoroutine (StartGame ());
	}

	bool ReadyCheck ()
	{
//		if (playerStatus.Count < 2)
//			return false;
		
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

			//if (Input.GetButtonDown ("P" + playerOne + "Submit"))
			//	StartCoroutine (StartGame ());
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
			GameObject newPlayer = Instantiate (playerPrefab, startPositions.GetChild (i - 1).position, Quaternion.Euler (new Vector3 (0.0f, 180.0f, 0.0f)), players);
			newPlayer.name = i.ToString ();

			PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
			playerController.enabled = false;
		}

		gameController.Restart ();

		gameController.InitializeGame (0.0f);
	}

	//True when starting lobby and false when exiting
	public void ResetLobby (bool status)
	{
		if (status)
		{
			playerStatus = new Dictionary <Player, Status> ();
			playerColors = new List <Color> {Color.blue, Color.red, Color.green, Color.yellow};
			Invoke ("ActivateLobbyFunctionality", 0.1f);
		}
		else
		{
			selectingPlayers = false;
			playerStatus = null;
			playerColors = null;
		}
	}

	void ActivateLobbyFunctionality ()
	{
		selectingPlayers = true;
	}
}
