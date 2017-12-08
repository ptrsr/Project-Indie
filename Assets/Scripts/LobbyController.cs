using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Players;
using System.Linq;

public class LobbyController : SubMenu {

	private enum Status
	{
        OnClaw,
		joined,
		ready
	}



	[HideInInspector] public bool selectingPlayers = false;
	private bool activateMainCanvas = false;

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

    [SerializeField]
    private GameObject clawPrefab;

    [SerializeField]
    private float 
        _clawSpeed,
        _clawMulti = 2;

	private GameController gameController;
	private PPController ppController;

    private bool[]
        slots = new bool[4],
        claws = new bool[4];

	void Start ()
	{
		Cursor.visible = false;

		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent <GameController> ();
		ppController = gameController.ppController;
		DeactivateLobbyCanvas ();
		exitConfirmPanel.SetActive (false);

		#if UNITY_EDITOR
		Debug ();
		#endif

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = false;
            claws[i] = false;
        }
	}

	void Update ()
	{
		if (selectingPlayers)
			CheckForPlayerCancel ();

		if (InputHandler.GetButtonDown (Player.P1, Players.Button.Settings) && selectingPlayers)
		{
			settingsCanvas.SetActive (true);
			ServiceLocator.Locate <Menu> ().SendCommand (2);
			settingsCanvas.GetComponent <CameraObjectsPositioning> ().EnableThis ();
			ppController.ChangePP (PPController.PP.game);

			if (exitConfirmPanel.activeInHierarchy)
				exitConfirmPanel.SetActive (false);

			Invoke ("DisableThis", 0.01f);
		}

		if (controlsCanvas.activeInHierarchy)
		{
			if (InputHandler.GetButtonDown (Players.Player.P1, Players.Button.Cancel))
			{
				controlsCanvas.SetActive (false);
				ActivateBlur (false);
			}
		}

		if (activateMainCanvas)
			ActivateMainCanvas ();

		if (selectingPlayers)
		{
			for (int i = 0; i < players.childCount; i++)
			{
				Transform laserHolder = players.GetChild (i).GetComponent <PlayerController> ().laserHolder;

				if (laserHolder.childCount != 0)
				{
					for (int j = 0; j < laserHolder.childCount; j++)
						Destroy (laserHolder.GetChild (j).gameObject);
				}
			}
		}
	}

	public void ActivateBlur (bool status)
	{
		if (ppController == null)
			return;
		
		if (status)
			ppController.ChangePP (PPController.PP.blur);
		else
			ppController.ChangePP (PPController.PP.menu);
	}

	public void DeactivateBlurInSettings ()
	{
		ppController.ChangePP (PPController.PP.game);
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
            Status value;

			if (playerStatus.TryGetValue (player, out value) && value == Status.joined)
				BecomeReady (player);
			else if (!playerStatus.ContainsKey (player))
			{
				if (player == Player.P1 && exitConfirmPanel.activeInHierarchy)
					return;
				else
					JoinLobby (player);
			}
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
        if (playerStatus.ContainsKey(player))
            playerStatus[player] = Status.OnClaw;
        else
        playerStatus.Add(player, Status.OnClaw);

        GameObject newPlayer = Instantiate (playerPrefab, Vector3.zero, Quaternion.Euler (new Vector3 (0.0f, 90.0f, 0.0f)), players);
		newPlayer.name = ((int) player).ToString ();
		PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
		playerController.playerNumber = player;
		playerController.AssignColor ();
		playerController.inLobby = true;

		gameController.musicManager.GetComponent <MusicChanger> ().PlayLobbySound ();

		SetPlayerPosition(newPlayer);
	}

	void SetPlayerPosition (GameObject player)
	{
		for (int i = 0; i < slots.Length; i++)
		{
            if (slots[i])
                continue;

            slots[i] = true;

            PlayerController pc = player.GetComponent<PlayerController>();
            pc.enabled = false;
            pc.slot = i;
            StartCoroutine(PlacePlayer(player, players.transform.position + i * new Vector3(8f, 0, 0)));
            return;
		}
	}

    IEnumerator PlacePlayer(GameObject player, Vector3 worldPos)
    {
        float height = 15;

        float playerHeight = 7;

        PlayerController pc = player.GetComponent<PlayerController>();


        claws[pc.slot] = true;
        Vector3 clawStartPos = worldPos + new Vector3(0, height, 0);

        GameObject claw = Instantiate(clawPrefab, clawStartPos, Quaternion.Euler(0, 90, 0));
        Animator anim = claw.GetComponent<Animator>();
        anim.speed = 10;

        player.transform.parent = claw.transform;
        player.transform.localPosition = new Vector3(0, -playerHeight, 0);

        float distance = Vector3.Distance(claw.transform.position, worldPos);
        float startDist = distance;

        while (distance > 0.1f)
        {
            distance = Vector3.Distance(claw.transform.position, worldPos);
            claw.transform.position = Vector3.Lerp(claw.transform.position, worldPos, Mathf.Max(_clawSpeed, 1 - (distance / startDist)) * Time.deltaTime * _clawMulti);
            yield return null;
        }

        anim.speed = 1;
        anim.SetBool("Open", true);
        player.transform.parent = players.transform;
        distance = Vector3.Distance(claw.transform.position, clawStartPos);

        pc.enabled = true;

        playerStatus[pc.playerNumber] = Status.joined;

        yield return new WaitForSeconds(0.6f);

        while (distance > 2f)
        {
            distance = Vector3.Distance(claw.transform.position, clawStartPos);
            claw.transform.position = Vector3.Lerp(claw.transform.position, clawStartPos, Mathf.Max(_clawSpeed, 1 - (distance / startDist)) * Time.deltaTime * _clawMulti);
            yield return null;
        }

        claws[pc.slot] = false;
        Destroy(claw);
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
		Invoke ("DeactivateLobbyCanvas", 1.0f);
		transform.root.GetComponent <Menu> ().SendCommand (0);

		gameController.SetupGame (players);

		//Colors
		for (int i = 0; i < players.childCount; i++)
		{
			PlayerController playerController = players.GetChild (i).GetComponent <PlayerController> ();
			
			playerController.AssignColor ();
			playerController.inLobby = false;
			playerController.anim.SetBool ("Moving", false);
			playerController.anim.speed = 8;
			playerController.enabled = false;
		}

		yield return new WaitForSeconds (1.0f);

		if (gameController.gameStarted)
		{
			int playerCount = players.childCount;

			List <int> startPos = new List <int> ();

			for (int j = 0; j < playerCount; j++)
				startPos.Add (j);

			for (int i = 0; i < playerCount; i++)
			{
				Transform tempPlayer = players.GetChild (i);

				int randomNumber = Random.Range (0, startPos.Count);

				if (maps.GetChild (0).gameObject.activeInHierarchy)
				{
					tempPlayer.position = startPositions.GetChild (0).GetChild (startPos [randomNumber]).position;
					tempPlayer.rotation = startPositions.GetChild (0).GetChild (startPos [randomNumber]).rotation;
				}
				else if (maps.GetChild (1).gameObject.activeInHierarchy)
				{
					tempPlayer.position = startPositions.GetChild (1).GetChild (startPos [randomNumber]).position;
					tempPlayer.rotation = startPositions.GetChild (1).GetChild (startPos [randomNumber]).rotation;
				}
				else if (maps.GetChild (2).gameObject.activeInHierarchy)
				{
					tempPlayer.position = startPositions.GetChild (2).GetChild (startPos [randomNumber]).position;
					tempPlayer.rotation = startPositions.GetChild (2).GetChild (startPos [randomNumber]).rotation;
				}

				PlayerController playerController = tempPlayer.GetComponent <PlayerController> ();

				playerController.aim.localRotation = Quaternion.Euler (Vector3.zero);
				
				startPos.Remove (startPos [randomNumber]);

				playerController.bodyAnim.SetInteger ("playerClip", 0);

				tempPlayer.GetChild (1).GetChild (2).gameObject.SetActive (true);

				playerController.arrow.SetActive (true);

				//Not random
				//tempPlayer.position = startPositions.GetChild ((int) tempPlayer.GetComponent <PlayerController> ().playerNumber - 1).position;
			}

            for (int i = 0; i < slots.Length; i++)
                slots[i] = false;
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
			{
				newPlayer.transform.position = startPositions.GetChild (0).GetChild (startPos [randomNumber]).position;
				newPlayer.transform.rotation = startPositions.GetChild (0).GetChild (startPos [randomNumber]).rotation;
			}
			else if (maps.GetChild (1).gameObject.activeInHierarchy)
			{
				newPlayer.transform.position = startPositions.GetChild (1).GetChild (startPos [randomNumber]).position;
				newPlayer.transform.rotation = startPositions.GetChild (1).GetChild (startPos [randomNumber]).rotation;
			}
			else if (maps.GetChild (2).gameObject.activeInHierarchy)
			{
				newPlayer.transform.position = startPositions.GetChild (2).GetChild (startPos [randomNumber]).position;
				newPlayer.transform.rotation = startPositions.GetChild (2).GetChild (startPos [randomNumber]).rotation;
			}

			startPos.Remove (startPos [randomNumber]);

			//Not random
			//newPlayer.transform.position = startPositions.GetChild ((int) playerStatus.Keys.ElementAt (i) - 1).position;

			newPlayer.name = ((int) playerStatus.Keys.ElementAt (i)).ToString ();

			PlayerController playerController = newPlayer.GetComponent <PlayerController> ();
			playerController.playerNumber = playerStatus.Keys.ElementAt (i);
			playerController.AssignColor ();
			playerController.inLobby = false;
			playerController.anim.SetBool ("Moving", false);
			playerController.anim.speed = 8;
			playerController.enabled = false;
			playerController.arrow.SetActive (true);

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
			{
				players.GetChild (i).GetComponent <PlayerController> ().StopAllSounds ();
				
				Destroy (players.GetChild (i).gameObject);
			}
		}

        for (int i = 0; i < slots.Length; i++)
            slots[i] = false;
    }

	public void FirstInitializeLobby ()
	{
		ResetLobby (true, false);
		lobbyCanvas.SetActive (true);
	}

	//True when starting lobby and false when exiting - second bool true when going back to lobby from game, otherwise false
	public void ResetLobby (bool status, bool backToLobby)
	{
        for (int i = 0; i < slots.Length; i++)
            slots[i] = false;

        if (status)
		{
			if (!backToLobby)
				playerStatus = new Dictionary <Player, Status> ();
			gameCanvas.SetActive (false);
			ppController.ChangePP (PPController.PP.menu);
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
				else if (playerStatus[player] != Status.OnClaw && !claws[playerController.slot])
                    StartCoroutine(RemovePlayer(playerController));

				break;
			}
		}
	}

    IEnumerator RemovePlayer(PlayerController player)
    {
        float height = 15;
        float playerHeight = 7;

        playerStatus[player.playerNumber] = Status.OnClaw;

        player.enabled = false;
        Vector3 clawStartPos = player.transform.position + new Vector3(0, height, 0);

        claws[player.slot] = true;
        GameObject claw = Instantiate(clawPrefab, clawStartPos, Quaternion.Euler(0, 90, 0));
        Animator anim = claw.GetComponent<Animator>();

        anim.speed = 10;
        anim.SetBool("Open", true);

        float distance = Vector3.Distance(claw.transform.position, player.transform.position + new Vector3(0, playerHeight, 0));
        float startDist = distance;

        while (distance > 0.1f)
        {
            distance = Vector3.Distance(claw.transform.position, player.transform.position + new Vector3(0, playerHeight, 0));
            claw.transform.position = Vector3.Lerp(claw.transform.position, player.transform.position + new Vector3(0, playerHeight, 0), Mathf.Max(_clawSpeed, 1 - (distance / startDist)) * Time.deltaTime * _clawMulti);
            yield return null;
        }

        player.transform.parent = claw.transform;
        anim.speed = 2;
        anim.SetBool("Open", false);

        distance = Vector3.Distance(claw.transform.position, clawStartPos);

        yield return new WaitForSeconds(0.6f);

        clawStartPos += new Vector3(0, playerHeight, 0);

        while (distance > 2f)
        {
            distance = Vector3.Distance(claw.transform.position, clawStartPos);
            claw.transform.position = Vector3.Lerp(claw.transform.position, clawStartPos, Mathf.Max(_clawSpeed, 1 - (distance / startDist)) * Time.deltaTime * _clawMulti);
            yield return null;
        }

        playerStatus.Remove(player.playerNumber);
        slots[player.slot] = false;
        claws[player.slot] = false;
        Destroy(claw);

		if (ReadyCheck () && selectingPlayers)
			StartCoroutine (StartGame ());
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
		activateMainCanvas = true;
		exitConfirmPanel.SetActive (false);
		ppController.ChangePP (PPController.PP.menu);

		//Fail-safe
		foreach (UnityEngine.UI.Button button in mainCanvas.GetComponentsInChildren <UnityEngine.UI.Button> ())
			button.interactable = true;
	}

	void ActivateMainCanvas ()
	{
		if (Vector3.Distance (Camera.main.transform.position, transform.root.GetChild (0).position) < 0.2f)
		{
			mainCanvas.SetActive (true);
			activateMainCanvas = false;
		}
	}

	public void DeactivateMainCanvas ()
	{
		mainCanvas.SetActive (false);
		activateMainCanvas = false;
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
